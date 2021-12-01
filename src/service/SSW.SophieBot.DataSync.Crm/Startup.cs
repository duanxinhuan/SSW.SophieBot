﻿using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SSW.SophieBot.AzureFunction.DependencyInjection;
using SSW.SophieBot.DataSync.Crm.Config;
using SSW.SophieBot.DataSync.Crm.HttpClients;

[assembly: FunctionsStartup(typeof(SSW.SophieBot.DataSync.Crm.Startup))]
namespace SSW.SophieBot.DataSync.Crm
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.AddSerilog();

            ConfigureCrm(builder);
            ConfigureSyncFunctions(builder);
            ConfigureAzureServices(builder);
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            builder.AddAppsettings();
        }

        private static void ConfigureCrm(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<CrmOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Crm").Bind(settings);
                });

            builder.Services.AddHttpClient<AuthClient>();
            builder.Services.AddHttpClient<CrmClient>();
        }

        private static void ConfigureSyncFunctions(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<SyncOptions>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    settings.OrganizationId = configuration["OrganizationId"];
                    configuration.GetSection("EmployeeSync").Bind(settings.EmployeeSync);
                });
        }

        private static void ConfigureAzureServices(IFunctionsHostBuilder builder)
        {
            var serviceBusConString = builder.GetContext().Configuration.GetConnectionString("ServiceBus");
            var cosmosConString = builder.GetContext().Configuration.GetConnectionString("CosmosDb");
            builder.Services.AddAzureClients(builder =>
            {
                builder.AddClient((SyncOptions _) => new CosmosClientBuilder(cosmosConString)
                    .WithSerializerOptions(new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    })
                );
                builder.AddServiceBusClient(serviceBusConString);
            });
        }
    }
}
