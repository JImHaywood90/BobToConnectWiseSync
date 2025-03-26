using System.Text;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Dto.Bob;
using AzureApiPoc.Services;
using Abstractions.Services;

namespace AzureApiPoc
{
    public class QueueConsumer
    {
        private readonly ILogger<QueueConsumer> _logger;
        private readonly IBobService _bobService;
        private readonly IConnectWiseService _connectWiseService;

        public QueueConsumer(ILogger<QueueConsumer> logger, IBobService bobService, IConnectWiseService connectWiseService)
        {
            _logger = logger;
            _bobService = bobService;
            _connectWiseService = connectWiseService;
        }

        [Function(nameof(QueueConsumer))]
        public async Task Run(
            [ServiceBusTrigger("zz-sbq-bob-cwpsa-api", Connection = "QueueConnectionString")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Processing Message ID: {id}", message.MessageId);
            var messageBody = Encoding.UTF8.GetString(message.Body.ToArray());
            _logger.LogInformation("Message Body: {body}", messageBody);

            // Deserialize the message into a TimeOffCreatedEvent
            var timeOffEvent = JsonConvert.DeserializeObject<TimeOffCreatedEvent>(messageBody);
            if (timeOffEvent == null)
            {
                _logger.LogError("Failed to deserialize the message.");
                // Optionally abandon or dead-letter the message
                return;
            }

            // Retrieve full leave details from Bob using the provided getApi URL
            var bobDetails = await _bobService.GetLeaveDetailsAsync(timeOffEvent.Data.GetApi);
            if (bobDetails == null)
            {
                _logger.LogError("Failed to retrieve Bob leave details.");
                return;
            }

            // Lookup ConnectWise user using the employee email from the leave details
            var cwUser = await _connectWiseService.GetUserByEmailAsync(bobDetails.employeeEmail);
            if (cwUser == null)
            {
                _logger.LogError("No ConnectWise user found for email {email}", bobDetails.employeeEmail);
                return;
            }

            // Create a new schedule entry in ConnectWise Manage
            bool created = await _connectWiseService.CreateScheduleEntryAsync(bobDetails, cwUser);
            if (created)
            {
                _logger.LogInformation("Schedule entry created successfully for user {email}", bobDetails.employeeEmail);
                await messageActions.CompleteMessageAsync(message);
            }
            else
            {
                _logger.LogError("Failed to create schedule entry in ConnectWise for user {email}", bobDetails.employeeEmail);
                // Optionally handle message abandonment or dead-lettering here
            }
        }
    }
}
