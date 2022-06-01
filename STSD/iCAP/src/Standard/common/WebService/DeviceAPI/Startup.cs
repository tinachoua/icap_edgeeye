using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using ShareLibrary.Interface;
using ShareLibrary;
using DeviceAPI.Models.Remote;
using Microsoft.AspNetCore.Http;

namespace DeviceAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();            
            services.AddScoped<IRedisCacheDispatcher, RedisCacheDispatcher>();
            services.AddScoped<IDataDBDispatcher, DataDBDispatcher>();
            services.AddScoped<IDevice, AdminDBDispatcher._device>();
            services.AddScoped<IBranch, AdminDBDispatcher._branch>();
            services.AddScoped<IRemoteCommandSender,RemoteCommandSender>();
            services.AddScoped<IEmployee, AdminDBDispatcher._employee>();
            services.AddScoped<IWidgetRepository, AdminDBDispatcher._widget>();
            services.AddScoped<IPermission, AdminDBDispatcher._permission>();

            // services.AddScoped<IFormCollection,
#if DEBUG
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "DeviceAPI",
                    Version = "v1",
                    Description = "iCAP Device API"
                });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "DeviceAPI.xml");
                c.IncludeXmlComments(xmlPath);
            });
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            //app.UseStaticFiles();
            //app.Map("/ws", WebSocketDispatcher.Map);

#if DEBUG
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "iCAP Device API V1");
            });
#endif
            app.UseMvc();
        }
    }
}
