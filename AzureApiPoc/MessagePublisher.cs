using Abstractions;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace AzureApiPoc
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly ILogger<MessagePublisher> _logger;
        private readonly IQueueClient _queueClient;

        public MessagePublisher(IQueueClient queueClient, ILogger<MessagePublisher> logger)
        {
            _queueClient = queueClient;
            _logger = logger;
        }

        public Task PublishAsync<T>(T model)
        {
            var message = JsonConvert.SerializeObject(model);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            return _queueClient.SendAsync(new Message(messageBytes));
        }

        public Task PublishAsync(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);

            return _queueClient.SendAsync(new Message(messageBytes));
        }
    }
}
