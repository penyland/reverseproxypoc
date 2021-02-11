using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using ReverseProxyPOC.Proxy.Configuration;
using ReverseProxyPOC.Proxy.Proxy;
using ReverseProxyPOC.Proxy.Services;
using System.Threading.Tasks;

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

            services.AddFeatureManagement();

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

            app.Use((context, next) =>
            {
                var endpoint = context.GetEndpoint();
                if (endpoint is null)
                {
                    return Task.CompletedTask;
                }

                logger.LogInformation($"Endpoint: {endpoint.DisplayName}");

                if (endpoint is RouteEndpoint routeEndpoint)
                {
                    logger.LogInformation("Endpoint has route pattern: " +
                        routeEndpoint.RoutePattern.RawText);
                }

                return next();
            });

            app.Use((context, next) =>
            {
                var endpointFeature = context.Features[typeof(IEndpointFeature)] as IEndpointFeature;
                var endpoint = endpointFeature?.Endpoint;

                if (endpoint != null)
                {
                    var routePattern = (endpoint as RouteEndpoint)?.RoutePattern.RawText;
                }

                return next();
            });

            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapControllers();
                endpoints.MapReverseProxy();

                // Map dynamic controller at order 0 so it runs before reverse proxy.
                // endpoints.MapDynamicControllerRoute<RouteValueTransformer>("{controller}/{id:int?}", state: null, order: 0);
                // endpoints.MapDynamicControllerRoute<RouteValueTransformer>("{controller}/{id:int?}");
                endpoints.MapDynamicControllerRoute<RouteValueTransformer>("{controller}/{id:int?}/{action?}", state: null, order: 0);

                // endpoints.MapDynamicControllerRoute<RouteValueTransformer>("{**route}");
                // endpoints.MapDynamicControllerRoute<RouteValueTransformer>("{controller}");
                // endpoints.MapDynamicControllerRoute<RouteValueTransformer>("{controller}/{action}/{id?}");
                // endpoints.MapDynamicControllerRoute<RouteValueTransformer>("{**catch-all}");
            });

            hostApplicationLifetime.ApplicationStarted.Register(OnApplicationStarted);
        }

        private void OnApplicationStarted()
        {
        }
    }
}
