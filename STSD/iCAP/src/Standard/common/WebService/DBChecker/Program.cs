using System;
using ShareLibrary;
using StackExchange.Redis;
using System.IO;
using Newtonsoft.Json;
using ShareLibrary.AdminDB;

namespace DBChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            DefaultSQLData defaultSQLData;
            if (File.Exists("DefaultData.json"))
            {
                var serializer = new JsonSerializer();
                using (StreamReader r = File.OpenText("DefaultData.json"))
                using (var jsonTextReader = new JsonTextReader(r))
                {
                    defaultSQLData = serializer.Deserialize<DefaultSQLData>(jsonTextReader);
                }
            }
            else
            {
                string path = Path.Combine("DefaultData", "AETINA.json");
                var serializer = new JsonSerializer();
                using (StreamReader r = File.OpenText(path))
                using (var jsonTextReader = new JsonTextReader(r))
                {
                    defaultSQLData = serializer.Deserialize<DefaultSQLData>(jsonTextReader);
                }
            }

            AdminDBDispatcher adb = new AdminDBDispatcher(defaultSQLData);
            DataDBDispatcher ddb = new DataDBDispatcher();
            adb.InitialCheck();
            ddb.Init();
        }
    }
}
