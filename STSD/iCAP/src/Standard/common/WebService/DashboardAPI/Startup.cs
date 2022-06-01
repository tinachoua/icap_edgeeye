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
using ShareLibrary;
using ShareLibrary.Interface;
using DashboardAPI.Models;
using Microsoft.AspNetCore.Cors;

namespace DashboardAPI
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
            
            //CORS
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("CorsPolicy", policy =>
            //    {
            //        policy.AllowAnyOrigin()
            //              .AllowAnyHeader()
            //              .AllowAnyMethod()
            //              .AllowCredentials();
            //    });
            //});
            //

            services.AddMvc();
            services.AddScoped<IWidgetRepository,AdminDBDispatcher._widget>();
            services.AddScoped<IRedisCacheDispatcher, RedisCacheDispatcher>();
            services.AddScoped<IDataDBDispatcher, DataDBDispatcher>();
            services.AddScoped<IDevice, AdminDBDispatcher._device>();
            services.AddScoped<IEmail, AdminDBDispatcher._email>();
            services.AddScoped<IEmployee, AdminDBDispatcher._employee>();
            services.AddScoped<IMultipleDashboard, AdminDBDispatcher._multipleDashboard>();
            services.AddScoped<IBranch, AdminDBDispatcher._branch>();
            services.AddScoped<IThreshold, AdminDBDispatcher._threshold>();
            services.AddScoped<IData, AdminDBDispatcher._data>();
            services.AddScoped<IPermission, AdminDBDispatcher._permission>();
            services.AddScoped<ICustomizedMap, AdminDBDispatcher._customizedMap>();
            //services.AddScoped<ISocket, SocketDispatcher>();
#if DEBUG
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "DashboardAPI",
                    Version = "v1",
                    Description = "iCAP Dashboard API"
                });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "DashboardAPI.xml");
                c.IncludeXmlComments(xmlPath);
            });
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));// crate the log
            loggerFactory.AddDebug();
#if DEBUG
            app.UseSwagger(); 

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "iCAP Dashboard API V1");
            });

            //CORS
            //app.UseCors("CorsPolicy");
            //
#endif
            //app.UseCors("CorsPolicy");
            app.UseMvc();
        }
    }
}
