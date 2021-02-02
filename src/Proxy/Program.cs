using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ReverseProxyPOC.Proxy.Configuration;
using Serilog;
using System;

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
                config.AddCustomConfiguration(() => { });
            })
            .UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration);
            });
    }
}
