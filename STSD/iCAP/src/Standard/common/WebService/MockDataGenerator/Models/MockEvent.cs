using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;

namespace MockDataCreate.Models
{
    public class MockEvent
    {
        public MockEvent(Random _rnd)
        {
            rnd = _rnd;
        }

        Random rnd;
        public void MockOfflineEvent(List<BsonDocument> eventList, string devName) {
            eventList.Add(new BsonDocument() {
                {"Dev", devName},
                {"Message", devName + " offline"},
                {"Checked", ((rnd.NextDouble() > 0.5) ? false : true)},
                {"Time", (double)rnd.Next(1506988800, 1506999540)}
            });
        }

        public void MockTemperatureEvent(List<BsonDocument> eventList, string devName, string StorageSN, string temp)
        {
            eventList.Add(new BsonDocument()
            {
                {"Dev", devName},
                {"Message", "Storage " + StorageSN + " " + "temperature over thershold, value : " + temp + " celsius."},
                {"Checked", ((rnd.NextDouble() > 0.5) ? false : true)},
                {"Time", (double)rnd.Next(1506902400, 1506999540)}
            });
        }

        public void MockLifespanEvent(List<BsonDocument> eventList, string devName, string StorageSN, string lifespan)
        {
            eventList.Add(new BsonDocument()
                {
                    {"Dev", devName},
                    {"Message", "Storage " + StorageSN +" "+"lifespan over thershold, value : " + lifespan +" days."},
                    {"Checked", ((rnd.NextDouble() > 0.5) ? false : true)},
                    {"Time", (double)rnd.Next(1506902400, 1506999540)}
                }
            );
        }
    }
}
