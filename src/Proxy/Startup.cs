using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using ReverseProxyPOC.Proxy.Proxy;
using ReverseProxyPOC.Proxy.Proxy.Filters;

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
                .LoadFromConfig(Configuration.GetSection("ReverseProxy"));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Proxy", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy
                        .WithOrigins("https://localhost:44341", "http://localhost:55420")
                        .AllowAnyMethod();
                });
            });

            services.AddHealthChecks();

            services.AddFeatureManagement()
                .AddFeatureFilter<ProxyAllowedFilter>()
                .UseDisabledFeaturesHandler(new DisabledEndpointHandler());

            services.AddHttpProxy();

            services.ConfigureProxy(Configuration.GetSection("DynamicProxyRoutes"));
            services.AddConfig<DynamicRouteSettings>(Configuration.GetSection("DynamicProxyRoutes"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger, IHostApplicationLifetime hostApplicationLifetime)
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors();

            app.UseMiddleware<EndpointSwitchingMiddleware>();
            // app.UseMiddlewareForFeature<EndpointSwitchingMiddleware>(nameof(FeatureFlags.EndpointSwitching));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapReverseProxy();

                endpoints.MapHealthChecks("/api/health");
            });
        }
    }
}
