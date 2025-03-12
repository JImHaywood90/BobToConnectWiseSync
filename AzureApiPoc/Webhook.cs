using Abstractions;
using Dto.Bob;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureApiPoc
{
    public class Webhook
    {
        private readonly ILogger<Webhook> _logger;
        private readonly IMessagePublisher _messagePublisher;

        public Webhook(ILogger<Webhook> logger, IMessagePublisher messagePublisher)
        {
            _logger = logger;
            _messagePublisher = messagePublisher;
        }

        [Function("WebHook")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "webhook")] HttpRequest req)
        {
            _logger.LogInformation("Publishing request payload as message");

            var requestContent = req.ReadFromJsonAsync<TimeOffCreatedEvent>().Result;

            _messagePublisher.PublishAsync(requestContent);

            _logger.LogInformation("Publishing request payload as message");

            return new OkResult();
        }
    }
}