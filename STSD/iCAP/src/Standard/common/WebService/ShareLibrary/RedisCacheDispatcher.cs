using Newtonsoft.Json;
using ShareLibrary.DataTemplate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using ShareLibrary.Interface;

namespace ShareLibrary
{
    public class RedisCacheDispatcher : IRedisCacheDispatcher
    {
        private static string ConnectionString;

        private static ConnectionMultiplexer redis;

        public void GetConnectionString()
        {
            if (File.Exists("DBSetting.json"))
            {
                using (StreamReader sr = new StreamReader(new FileStream("DBSetting.json", FileMode.Open)))
                {
                    dynamic str = JsonConvert.DeserializeObject(sr.ReadLine());
                    ConnectionString = str.RedisConnectionString;
                }
            }
            else
            {
                ConnectionString = "172.30.0.5";
                var json = new
                {
                    AdminDBConnectionString = "server=172.30.0.3;userid=root;pwd=admin;port=3306;database=icap;persistsecurityinfo=True",
                    DataDBConnectionString = "mongodb://172.30.0.2:27017",
                    RedisConnectionString = ConnectionString
                };

                using (StreamWriter sw = new StreamWriter(new FileStream("DBSetting.json", FileMode.CreateNew)))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(json));
                }
            }
            redis = ConnectionMultiplexer.Connect(ConnectionString);
        }

        public bool ClearCache()
        {
            try
            {
                var server = redis.GetServer(ConnectionString);
                server.FlushDatabase(0);

            }
            catch (Exception ex)
            {
                //Console.ForegroundColor = ConsoleColor.Red;
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "ClearCache",
                    URL = "",
                    ResponseCode = 0,
                    Remark = ex.Message
                });
                //Console.ResetColor();
                return false;
            }
            
            return true;
        }

        public bool SetCache(int dbIndex, string key, string value)
        {
            try
            {
                object AsyncState = null;
                IDatabase db = redis.GetDatabase(dbIndex, AsyncState);
                db.StringSet(key, value);
            }
            catch (Exception ex)
            {
                //Console.ForegroundColor = ConsoleColor.Red;
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "SetCache",
                    URL = "",
                    ResponseCode = 0,
                    Remark = ex.Message
                });
                //Console.ResetColor();
                return false;
            }
            return true;
        }

        public bool SendPWD(string account, string pwd)
        {
            var payload = new
            {
                account = account,
                pwd = pwd
            };

            try
            {
                ISubscriber sub = redis.GetSubscriber();
                sub.Publish("Cmd", JsonConvert.SerializeObject(payload));
            }
            catch (Exception ex)
            {
                //Console.ForegroundColor = ConsoleColor.Red;
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "SendPWD",
                    URL = "",
                    ResponseCode = 0,
                    Remark = ex.Message
                });
                //Console.ResetColor();
                return false;
            }
            return true;
        }

        public void StartSubscribe()
        {
            ISubscriber sub = redis.GetSubscriber();
            sub.Subscribe("Cmd", (channel, message) =>
            {

            });
        }

        public bool AddDevice(string devName)
        {
            try
            {
                object AsyncState = null;
                IDatabase db = redis.GetDatabase(1, AsyncState);
                db.StringSet(devName, 0);
            }
            catch (Exception ex)
            {
                //Console.ForegroundColor = ConsoleColor.Red;
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "AddDevice",
                    URL = "",
                    ResponseCode = 0,
                    Remark = ex.Message
                });
                //Console.ResetColor();
                return false;
            }
            return true;
        }

        public string GetCache(int dbIndex, string key)
        {
            string ret = null;
            try
            {
                object AsyncState = null;
                IDatabase db = redis.GetDatabase(dbIndex, AsyncState);
                ret = db.StringGet(key);
            }
            catch (Exception)
            {
                //ret = "0";
            }
            return ret;
        }

        public bool SetStatus(string key, int value)
        {
            var payload = new
            {
                Device = key,
                Status = value
            };

            try
            {
                object AsyncState = null;
                IDatabase db = redis.GetDatabase(1, AsyncState);
                ISubscriber sub = redis.GetSubscriber();
                db.StringSet(key, value);
                sub.Publish("Status", JsonConvert.SerializeObject(payload));

            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public string GetStatus(string key)
        {
            string ret = null;
            try
            {
                object AsyncState = null;
                IDatabase db = redis.GetDatabase(1, AsyncState);
                ret = db.StringGet(key);
            }
            catch (Exception)
            {

            }
            return ret;
        }

        public bool CleanDeviceStatus()
        {
            try
            {
                var server = redis.GetServer(ConnectionString);
                server.FlushDatabase(1);
            }
            catch (Exception ex)
            {
                //Console.ForegroundColor = ConsoleColor.Red;
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "CleanDeviceStatus",
                    URL = "",
                    ResponseCode = 0,
                    Remark = ex.Message
                });
                //Console.ResetColor();
                return false;
            }
            return true;
        }

        public bool PublishSetThreshold(ThresholdSettingTemplate d)
        {
            var payload = new
            {
                Message = "Set threshold"
            };

            try
            {
                ISubscriber sub = redis.GetSubscriber();
                sub.Publish("Threshold", JsonConvert.SerializeObject(d));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public List<KeyValuePair<string, string>> GetAll(int dbIndex)
        {
            List<KeyValuePair<string, string>> ret = new List<KeyValuePair<string, string>>();
            var server = redis.GetServer(ConnectionString + ":6379");
            IDatabase db = redis.GetDatabase(dbIndex);
            var keys = server.Keys(dbIndex);

            foreach (var key in keys)
            {
                var account = db.StringGet(key);
                ret.Add(new KeyValuePair<string, string>(account, key.ToString()));
            }
            return ret;
        }

        public void KeyDelete(int dbIndex, string key)
        {
            var server = redis.GetServer(ConnectionString + ":6379");
            IDatabase db = redis.GetDatabase(dbIndex);
            db.KeyDelete(key);
        }

        public void KeyDeleteByValue(int dbIndex, string value)
        {
            var server = redis.GetServer(ConnectionString + ":6379");
            IDatabase db = redis.GetDatabase(dbIndex);

            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();
            var keys = server.Keys(dbIndex);

            foreach (var key in keys)
            {
                var account = db.StringGet(key);
                items.Add(new KeyValuePair<string, string>(key.ToString(), account));
            }

            foreach (var item in items)
            {
                if (item.Value == value)
                {
                    db.KeyDelete(item.Key);
                }
            }
        }
    }
}
