using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RSSFeedBot.Configurations;
using System;

[assembly: FunctionsStartup(typeof(RSSFeedBot.Startup))]

namespace RSSFeedBot
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);

            var context = builder.GetContext();
            builder.ConfigurationBuilder
                .SetBasePath(context.ApplicationRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{context.EnvironmentName}.json", true);
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<RSSFeedConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection(nameof(RSSFeedConfiguration)).Bind(settings);
                });
        }
    }
}
