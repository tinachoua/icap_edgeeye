using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using ShareLibrary.DataTemplate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ShareLibrary.Interface;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ShareLibrary.Contsants;
using ShareLibrary.Mongo.Template;
using ShareLibrary.Mongo;

namespace ShareLibrary
{
    public class DataDBDispatcher : IDataDBDispatcher
    {
        protected static string mongoDBConnectionString;
        protected string DBName = "iCAP";
        protected MongoClient _mongoclient;
        protected IMongoDatabase _database;

        public DataDBDispatcher()
        {
            GetConnectionString();
            _mongoclient = new MongoClient(mongoDBConnectionString);
            _database = _mongoclient.GetDatabase(DBName);
        }
        public void GetConnectionString ()
        {
            if (File.Exists("DBSetting.json"))
            {
                using (StreamReader sr = new StreamReader(new FileStream("DBSetting.json", FileMode.Open)))
                {
                    dynamic str = JsonConvert.DeserializeObject(sr.ReadLine());
                    mongoDBConnectionString = str.DataDBConnectionString;
                }
            }
            else
            {
                mongoDBConnectionString = "mongodb://icap-admin:icap-admin-pwd@172.30.0.2:27017/?authSource=iCAP";
                var json = new
                {
                    AdminDBConnectionString = "server=172.30.0.3;userid=root;pwd=admin;port=3306;database=icap;persistsecurityinfo=True",
                    DataDBConnectionString = mongoDBConnectionString,
                    RedisConnectionString = "172.30.0.5"
                };

                using (StreamWriter sw = new StreamWriter(new FileStream("DBSetting.json", FileMode.CreateNew)))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(json));
                }
            }
        }

        public BsonDocument GetLastRawData (string CollectionName)
        {
            var mongoclient = new MongoClient(mongoDBConnectionString);
            var database = mongoclient.GetDatabase(DBName);
            var collection = database.GetCollection<BsonDocument>(CollectionName);

            List<BsonDocument> retList = collection.Find(new BsonDocument()).Sort("{_id:-1}").Limit(1).ToList();

            if (retList.Count == 0)
            {
                return null;
            }
            return retList.Last();
        }

        public BsonDocument GetLastRawData(string CollectionName, string QueryStr)
        {
            var mongoclient = new MongoClient(mongoDBConnectionString);
            var database = mongoclient.GetDatabase(DBName);
            var collection = database.GetCollection<BsonDocument>(CollectionName);

            try
            {
                BsonDocument bson = BsonSerializer.Deserialize<BsonDocument>(QueryStr);
                return collection.Find(bson).First().AsBsonDocument;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        public List<BsonDocument> GetRawData (string CollectionName, string QueryStr, int Limit)
        {
            var mongoclient = new MongoClient(mongoDBConnectionString);
            var database = mongoclient.GetDatabase(DBName);
            var collection = database.GetCollection<BsonDocument>(CollectionName);
            List<BsonDocument> retList;


            if (QueryStr == null)
            {
                if (Limit > 0)
                {
                    retList = collection.Find(new BsonDocument()).Sort("{_id:-1}").Limit(Limit).ToList();
                }
                else
                {
                    retList = collection.Find(new BsonDocument()).Sort("{_id:-1}").ToList();
                }
            }
            else
            {
                try
                {
                    BsonDocument bson = BsonSerializer.Deserialize<BsonDocument>(QueryStr);
                    if (Limit > 0)
                    {
                        retList = collection.Find(bson).Sort("{_id:-1}").Limit(Limit).ToList();
                    }
                    else
                    {
                        retList = collection.Find(bson).Sort("{_id:-1}").ToList();
                    }
                }
                catch (Exception)
                {
                    retList = new List<BsonDocument>();
                }

            }

            /*if (retList.Count == 0)
            {
                return null;
            }*/
            return retList;
        }

        public long GetRawDataCount(string CollectionName, string QueryStr)
        {
            var mongoclient = new MongoClient(mongoDBConnectionString);
            var database = mongoclient.GetDatabase(DBName);
            var collection = database.GetCollection<BsonDocument>(CollectionName);
            long count = 0;

            try
            {
                if (QueryStr == null)
                {
                    count = collection.Count(new BsonDocument());
                }
                else
                {
                    BsonDocument bson = BsonSerializer.Deserialize<BsonDocument>(QueryStr);
                    count = collection.Find(bson).Count();
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            return count;
        }

        public void InsertData (string CollectionName, BsonDocument Value)
        {
            var mongoclient = new MongoClient(mongoDBConnectionString);
            var database = mongoclient.GetDatabase(DBName);
            var collection = database.GetCollection<BsonDocument>(CollectionName);

            collection.InsertOne(Value);
        }

        public BsonDocument GetAnalyzerData(string SerialNumber)
        {
            var mongoclient = new MongoClient(mongoDBConnectionString);
            var database = mongoclient.GetDatabase(DBName);
            var collection = database.GetCollection<BsonDocument>("StorageAnalyzer");
            var builder = Builders<BsonDocument>.Filter;
            var filter = builder.Eq("SN", SerialNumber);

            List<BsonDocument> retList = collection.Find(filter).Sort("{_id:-1}").Limit(1).ToList();

            if (retList.Count == 0)
            {
                return null;
            }
            return retList.Last();
        }

        public bool UpdateEventlog (EventDataTemplate Value)
        {
            var mongoclient = new MongoClient(mongoDBConnectionString);
            var database = mongoclient.GetDatabase(DBName);
            var collection = database.GetCollection<BsonDocument>("EventLog");
    
            var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(Value.EventId));

            var update = Builders<BsonDocument>.Update.Set("Checked", true);

            var result = collection.UpdateOne(filter, update);

            if(result.ModifiedCount == 1)
            {
                return true;
            }

            return false;
        }

        public string[] GetStorageList (string devName)
        {
            BsonDocument bdoc = GetLastRawData(string.Format("{0}-static", devName));
            List<string> storageSNList = new List<string>();
            foreach(BsonDocument bd in bdoc["Storage"].AsBsonArray)
            {
                storageSNList.Add(bd["SN"].AsString);
            }

            return storageSNList.ToArray();
        }

        public List<BsonDocument> GetRawData(string CollectionName, FilterDefinition<BsonDocument> queryStr)
        {
            var mongoclient = new MongoClient(mongoDBConnectionString);
            var database = mongoclient.GetDatabase(DBName);
            var collection = database.GetCollection<BsonDocument>(CollectionName);
            List<BsonDocument> retList;
            try
            {
                 retList = collection.Find(queryStr).ToList();
            }
            catch (Exception)
            {
                retList = new List<BsonDocument>();
            }
            return retList;
        }

        public Schema.Api_key GetGoogleMapAPIKey()
        {
            var keys = _database.GetCollection<BsonDocument>(MONGO.KEYS_COLLECTION);

            if (keys.Count(new BsonDocument()) == 0)
            {
                return null;
            }
            else
            {
                var bson = keys.Find(new BsonDocument()).First().ToBsonDocument();

                if (!bson.Contains(MONGO.GOOGLE_MAP))
                {
                    return null;
                }
                return BsonSerializer.Deserialize<Schema.Api_key>(bson[MONGO.GOOGLE_MAP].AsBsonDocument);
            }
        }
        public DateTime? UpdateAPIKey(string type, string key)
        {
            var collection = _database.GetCollection<BsonDocument>(MONGO.KEYS_COLLECTION);

            var bsond = collection.Find(new BsonDocument()).First().ToBsonDocument();

            var filter = Builders<BsonDocument>.Filter.Eq("_id", bsond["_id"].AsObjectId);

            var date = DateTime.UtcNow;

            var update = Builders<BsonDocument>.Update
                .Set(b => b[type][MONGO.KEY_ATTR], key)
                .Set(b => b[type][MONGO.UPDATE_TIME_ATTTR], date);


            var result = collection.UpdateOne(filter, update);

            if (result.ModifiedCount > 0)
            {
                return date;
            }

            return null;
        }

        public List<KeyUpdateTime> GetAPIUpdateTime()
        {
            var collection = _database.GetCollection<BsonDocument>(MONGO.KEYS_COLLECTION);

            var result = collection.Find(new BsonDocument());

            List<KeyUpdateTime> ret = new List<KeyUpdateTime>();

            if (result.Count() == 0)
            {
                collection.InsertOne(new BsonDocument() {
                    { MONGO.GOOGLE_MAP, new BsonDocument(){
                        { MONGO.KEY_ATTR, BsonNull.Value},
                        { MONGO.UPDATE_TIME_ATTTR, BsonNull.Value}
                    } }
                });
                return ret;
            }

            var bson = result.First().ToBsonDocument();


            foreach (string type in MONGO.API_TYPE)
            {
                if (!bson.Contains(type) || bson[type][MONGO.UPDATE_TIME_ATTTR].IsBsonNull) continue;

                ret.Add(new KeyUpdateTime()
                {
                    type = type,
                    updateTime = bson[type][MONGO.UPDATE_TIME_ATTTR].ToUniversalTime()
                });
            }

            return ret;
        }

        public void Init()
        {
            var keys = _database.GetCollection<BsonDocument>(MONGO.KEYS_COLLECTION);

            if (keys.Count(new BsonDocument()) == 0)
            {
                keys.InsertOne(new BsonDocument() {
                    { MONGO.GOOGLE_MAP, new BsonDocument(){
                        { MONGO.KEY_ATTR, BsonNull.Value},
                        { MONGO.UPDATE_TIME_ATTTR, BsonNull.Value}
                    } }
                });
            }
        }

        //public string GetSpecificField(string CollectionName, string Key)
        //{
        //    try
        //    {
        //        var mongoclient = new MongoClient(mongoDBConnectionString);
        //        var database = mongoclient.GetDatabase(DBName);
        //        var collection = database.GetCollection<BsonDocument>(CollectionName);

        //        var col = mydb.GetCollection<BsonDocument>("col");

        //        var projection = new BsonDocument() {
        //            { nameof(MyObject.Category), $"$key" },
        //            { nameof(MyObject.Name), $"$value" },
        //            { "_id", 0  }
        //        };

        //        var result = col.Aggregate().Unwind("value")
        //            .Project<MyObject>(projection).ToList();


        //        db.GetCollection("users");

        //        var cursor = Photos.FindAs<DocType>(Query.EQ("age", 33));
        //        cursor.SetFields(Fields.Include("a", "b"));
        //        var items = cursor.ToList()
        //        //var collection = database.GetCollection<BsonDocument>(CollectionName);

        //        //List<BsonDocument> retList = collection.Find(new BsonDocument()).Sort("{_id:-1}").Limit(1).ToList();
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        //public BsonDocument GetDashboardWidgetInfo(int dashboardId)
        //{
        //    var mongoclient = new MongoClient(mongoDBConnectionString);
        //    var database = mongoclient.GetDatabase(DBName);
        //    var collection = database.GetCollection<BsonDocument>("DashboardData");
        //    var builder = Builders<BsonDocument>.Filter;
        //    var filter = builder.Eq("SN", SerialNumber);

        //    List<BsonDocument> retList = collection.Find(filter).Sort("{_id:-1}").Limit(1).ToList();

        //    if (retList.Count == 0)
        //    {
        //        return null;
        //    }
        //    return retList.Last();
        //}
    }
}
