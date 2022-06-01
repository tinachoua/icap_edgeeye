using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using ShareLibrary;

namespace DashboardAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            DataDBDispatcher ddb = new DataDBDispatcher();
           

            ddb.GetConnectionString();
            rcd.GetConnectionString();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls("http://*:51000")
                .Build();

            host.Run();
        }
    }
}
