using Abstractions;
using AzureApiPoc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<IQueueClient>(client => new QueueClient(connectionString: "Endpoint=sb://zz-sb-bob-cwpsa-api.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=secret_key=",
                                                                      entityPath: "zz-sbq-bob-cwpsa-api"));
        services.AddSingleton<IMessagePublisher, MessagePublisher>();
    })
    .Build();

host.Run();
