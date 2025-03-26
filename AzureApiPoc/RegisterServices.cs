using Abstractions;
using Abstractions.Mapping;
using Abstractions.Services;
using AzureApiPoc.Configuration;
using AzureApiPoc.Http;
using AzureApiPoc.Mapping.Bob;
using AzureApiPoc.Services;
using AzureApiPoc.Services.ConnectWise;
using Dto.Bob;
using Dto.ConnectWise;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Bob;

public static class RegisterServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Load all API clients from config
        var apiClientConfigs = configuration.GetSection("ApiClients").Get<ApiClientsConfig>();

        services.AddTransient<ResilienceHandler>();

        foreach (var (name, options) in apiClientConfigs)
        {
            var clientName = name + "Client";

            services.AddHttpClient(clientName)
                .ConfigureHttpClient((sp, client) =>
                {
                    client.BaseAddress = new Uri(options.BaseUri);
                    client.Timeout = Timeout.InfiniteTimeSpan;

                    var config = sp.GetRequiredService<IConfiguration>();
                    var headerPrefix = $"ApiClients:{name}:DefaultHeaders:";
                    var section = config.GetSection($"ApiClients:{name}:DefaultHeaders");

                    foreach (var header in section.GetChildren())
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }

                    // Inject secret headers from Key Vault
                    var secretHeaderKeys = new[] { "Authorization", "X-Api-Key" }; // extend as needed
                    foreach (var key in secretHeaderKeys)
                    {
                        var secretValue = config[$"ApiClients:{name}:{key}"];
                        if (!string.IsNullOrWhiteSpace(secretValue))
                        {
                            client.DefaultRequestHeaders.Remove(key); // just in case
                            client.DefaultRequestHeaders.Add(key, secretValue);
                        }
                    }
                })
                .AddHttpMessageHandler<ResilienceHandler>();
        }

        // Register generic client factory
        services.AddSingleton<IGenericHttpClientFactory, GenericHttpClientFactory>();

        // Register Mapperly mappers
        services.AddSingleton<IModelMapper<HiBobLeaveDetails, ConnectWiseScheduleEntryPayload>, BobToConnectWiseMapper>();

        // Register ConnectWise API client
        services.AddHttpClient<IConnectWiseApiClient, ConnectWiseApiClient>()
            .ConfigureHttpClient((sp, client) =>
            {
                var options = apiClientConfigs["ConnectWise"];
                client.BaseAddress = new Uri(options.BaseUri);
                client.Timeout = Timeout.InfiniteTimeSpan;

                if (options.DefaultHeaders != null)
                {
                    foreach (var header in options.DefaultHeaders)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
            })
            .AddHttpMessageHandler<ResilienceHandler>();

        // Register BobService
        services.AddTransient<IBobService>(sp =>
        {
            var factory = sp.GetRequiredService<IGenericHttpClientFactory>();
            var logger = sp.GetRequiredService<ILogger<BobService>>();
            return new BobService((HttpClient)factory.CreateClient("BobClient"), logger);
        });

        // Register ConnectWiseService
        services.AddTransient<IConnectWiseService, ConnectWiseService>();

        return services;
    }
}
