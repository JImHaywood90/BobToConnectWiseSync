using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
   .ConfigureAppConfiguration((context, config) =>
   {
       var env = context.HostingEnvironment;
       config.AddJsonFile("appsettings.json", optional: true)
             .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
             .AddEnvironmentVariables();

       var builtConfig = config.Build();
       var keyVaultUri = builtConfig["KeyVaultUri"]; // stored in app config

       if (!string.IsNullOrEmpty(keyVaultUri))
       {
           config.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
       }
   })
    .Build();

host.Run();