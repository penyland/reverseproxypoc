using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ReverseProxyPOC.Proxy.Services;

namespace ReverseProxyPOC.Proxy
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddReverseProxy()
                .LoadFromConfig(Configuration.GetSection("ReverseProxy"))
                .AddProxyConfigFilter<CustomConfigFilter>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "proxy", Version = "v1" });
            });

            services.AddSingleton<IProxyConfigurationService, ProxyConfigurationService>();

            services.AddProxyConfiguration();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "proxy/swagger/{documentname}/swagger.json";
                });
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/proxy/swagger/v1/swagger.json", "Proxy v1");
                    c.RoutePrefix = "proxy/swagger";
                });
            }

            app.UseProxyConfigurationRefresher();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapReverseProxy();
            });
        }
    }
}
