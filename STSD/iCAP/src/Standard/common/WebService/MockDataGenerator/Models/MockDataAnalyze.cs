using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace MockDataCreate.Models
{
    public class MockDataAnalyze
    {
        const int LIFESPAN_DATA_COUNT = 20;
        double[] GetLifespanTime(int timestampNow) {
            double[] lifespantime = new double[LIFESPAN_DATA_COUNT];
            lifespantime[0] = timestampNow - 1728000.0;
            for (var i = 1; i < LIFESPAN_DATA_COUNT; i++)
            {
                lifespantime[i] = lifespantime[i - 1] + 86400.0;
            }
            return lifespantime;
        }


        BsonArray GetLifespan(MockStorageFactory.MockStorage mockStorage, int timestampNow) {
            BsonArray ret = new BsonArray();
            var lifespanTime = GetLifespanTime(timestampNow);
            for (var i=0; i < LIFESPAN_DATA_COUNT; i++)
            {
                ret.Add(new BsonDocument() {
                    {"time", lifespanTime[i]},
                    {"health", mockStorage.HealthList[i]},
                    {"data", mockStorage.LifespanData[i]}
                });
            }
            return ret;
        }
        public BsonDocument GetMockData(
            MockStorageFactory.MockStorage mockStorage,
            int timestampNow
        )
        {
            return new BsonDocument()
            {
                {"SN", mockStorage.StorageSN},
                {"Capacity", 238.467908},
                {"InitHealth", 100},
                {"InitTime", 1505260800},
                {"PECycle", 3000},
                {"Lifespan", GetLifespan(mockStorage, timestampNow)}

            };
        }
    }
}
