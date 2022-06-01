using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockDataCreate.Models
{
    public class MockStorageFactory
    {
        public MockStorageFactory(Random _rnd, MockEvent _MockEvent)
        {
            rnd = _rnd;
            MockEvent = _MockEvent;
        }
        public class MockStorage
        {
            public string StorageSN { get; set; }
            public double[] HealthList { get; set; }
            public int[] AvgECList { get; set; }
            public int[] LifespanData { get; set; }
            public int[] TempData { get; set; }
        }
        const int LIFESPAN_DATA_COUNT = 20;
        const int LIFESPAN_THRESHOLD = 150;
        const int TEMPERATURE_THRESHOLD = 50;
        MockEvent MockEvent;
        Random rnd;
        public MockStorage Mock(int index, Random rnd, List<BsonDocument> MockEventList, string devName)
        {
            string storageSN = "BCADevice" + (index).ToString("D5");
            int[] lifespanData = new int[MockDataDynamic.DYNAMIC_DATA_COUNT];
            double[] health = new double[MockDataDynamic.DYNAMIC_DATA_COUNT];
            int[] avgEC = new int[MockDataDynamic.DYNAMIC_DATA_COUNT];
            int[] tempData = new int[MockDataDynamic.DYNAMIC_DATA_COUNT];

            for (var i = 0; i < MockDataDynamic.DYNAMIC_DATA_COUNT; i++)
            {
                int temp, lifespan;
                if (index % 5 == 0)
                {
                    temp = rnd.Next(51, 80);
                    lifespan = rnd.Next(20, 2450);
                    MockEvent.MockTemperatureEvent(MockEventList, devName, storageSN, temp.ToString());
                }
                else if (index % 2 == 0)
                {
                    temp = rnd.Next(25, 50);
                    lifespan = rnd.Next(1800, 2450);
                }
                else
                {
                    temp = rnd.Next(25, 50);
                    lifespan = rnd.Next(2100, 2450);
                }

                tempData[i] = temp;
                lifespanData[i] = lifespan;
            }

            Array.Sort(lifespanData);
            Array.Reverse(lifespanData);

            Parallel.For(0, LIFESPAN_DATA_COUNT, k =>
            {
                health[k] = lifespanData[k] / 2450.0 * 100.0;
                avgEC[k] = 3000 - (int)Math.Floor(30 * health[k]);
            });

            int lastLifespan = lifespanData[MockDataDynamic.DYNAMIC_DATA_COUNT - 1];
            if (lastLifespan < LIFESPAN_THRESHOLD)
            {
                MockEvent.MockLifespanEvent(MockEventList, devName, storageSN, lastLifespan.ToString());
            }

            return new MockStorage()
            {
                StorageSN = storageSN,
                HealthList = health,
                AvgECList = avgEC,
                LifespanData = lifespanData,
                TempData = tempData
            };
        }
    }
}
