using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.Threading;
using MySql.Data.MySqlClient;
using ShareLibrary;

namespace AuthenticationAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
           
            RedisCacheDispatcher rcd = new RedisCacheDispatcher();
            //AdminDBDispatcher adb = new AdminDBDispatcher();
            DataDBDispatcher ddb = new DataDBDispatcher();

            ddb.GetConnectionString();
            rcd.GetConnectionString();
            //rcd.ClearCache();

#if false
            ddb.InsertFakeData();
#endif
            
           // rcd.StartSubscribe();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
		        .UseUrls("http://*:50000")
                .Build();

            host.Run();
        }
    }
}
