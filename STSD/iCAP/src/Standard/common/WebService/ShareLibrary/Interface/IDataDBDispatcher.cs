using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using ShareLibrary.DataTemplate;
using ShareLibrary.Mongo.Template;
using ShareLibrary.Mongo;
using System;

namespace ShareLibrary.Interface
{
    public interface IDataDBDispatcher
    {
        BsonDocument GetAnalyzerData (string SerialNumber);
        void GetConnectionString ();
        BsonDocument GetLastRawData (string CollectionName);
        BsonDocument GetLastRawData(string CollectionName, string QueryStr);
        List<BsonDocument> GetRawData (string CollectionName, string QueryStr, int Limit);
        void InsertData (string CollectionName, BsonDocument Value);
        bool UpdateEventlog (EventDataTemplate Value);
        string[] GetStorageList (string devName);
        long GetRawDataCount(string CollectionName, string QueryStr);
        List<BsonDocument> GetRawData(string CollectionName, FilterDefinition<BsonDocument> queryStr);
        Schema.Api_key GetGoogleMapAPIKey();
        DateTime? UpdateAPIKey(string type, string key);
        List<KeyUpdateTime> GetAPIUpdateTime();
    }
}