using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ReverseProxyPOC.Proxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // AppContext.SetSwitch("Microsoft.AspNetCore.Routing.UseCorrectCatchAllBehavior", true);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        config.AddUserSecrets<Startup>(optional: false, reloadOnChange: true);
                    }

                    config.AddJsonFile("routes.json");

                    var settings = config.Build();
                    var connection = settings.GetConnectionString("AppConfig");

                    config.AddAzureAppConfiguration(options =>
                    {
                        options
                          .Connect(connection)
                          .UseFeatureFlags()
                          // Load configuration values with no label
                          .Select(KeyFilter.Any, LabelFilter.Null)
                          // Override with any configuration values specific to current hosting env
                          .Select(KeyFilter.Any, context.HostingEnvironment.EnvironmentName)
                          .Select(keyFilter: "ProxyPOC:*", labelFilter: "Development");
                    });
                })
                .UseSerilog((context, config) =>
                {
                    config.ReadFrom.Configuration(context.Configuration);
                });
    }
}
