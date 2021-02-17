using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace ReverseProxyPOC.Proxy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // AppContext.SetSwitch("Microsoft.AspNetCore.Routing.UseCorrectCatchAllBehavior", true);
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                if (Log.Logger == null || Log.Logger.GetType().Name == "SilentLogger")
                {
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .CreateLogger();
                }

                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
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

                    if (!string.IsNullOrEmpty(settings.GetConnectionString("AppConfig")))
                    {
                        config.AddAzureAppConfiguration(options =>
                        {
                            options
                                .Connect(settings.GetConnectionString("AppConfig"))
                                .UseFeatureFlags()
                                // Load configuration values with no label
                                .Select(KeyFilter.Any, LabelFilter.Null)
                                // Override with any configuration values specific to current hosting env
                                .Select(KeyFilter.Any, context.HostingEnvironment.EnvironmentName)
                                .Select(keyFilter: "ProxyPOC:*", labelFilter: "Development");
                        });
                    }
                })
                .UseSerilog((context, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Environment", context.HostingEnvironment);
                });
    }
}
