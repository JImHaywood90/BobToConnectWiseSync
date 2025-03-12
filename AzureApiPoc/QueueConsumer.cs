using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureApiPoc
{
    public class QueueConsumer
    {
        private readonly ILogger<QueueConsumer> _logger;

        public QueueConsumer(ILogger<QueueConsumer> logger)
        {
            _logger = logger;
        }

        [Function(nameof(QueueConsumer))]
        public async Task Run(
            [ServiceBusTrigger("zz-sbq-bob-cwpsa-api", Connection = "QueueConnectionString")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
