using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MockDataCreate.Models
{
    public class MockDataFactory
    {
        public MockDataFactory(DeviceCount _deviceCount)
        {
            rnd = new Random(Guid.NewGuid().GetHashCode());
            DateTime UtcNow = DateTime.UtcNow;
            mockStartTimestamp = (int)(UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds - 1200;
            MockStatic = new MockDataStatic(rnd);
            MockDataAnalyze = new MockDataAnalyze();
            DeviceCount = _deviceCount;
            MockDynamic = new MockDataDynamic(rnd, mockStartTimestamp);
            MockEvent = new MockEvent(rnd);
            MockStorageFactory = new MockStorageFactory(rnd, MockEvent);
        }
        protected class MockFunParameter
        {
            public TestingData TestingData { get; set; }
            public DeviceCount.MockDevice[] MockDevices { get; set; }
            public int DeviceIndex { get; set; }
            public int StorageIndex { get; set; }
        }
        public class TestingData
        {
            public TestingData() {
                StaticData = new List<BsonDocument>();
                StorageAnalyzerData = new List<BsonDocument>();
                DynamicData = new List<BsonDocument[]>();
                EventData = new List<BsonDocument>();
            }
            public List<BsonDocument> StaticData { get; set; }
            public List<BsonDocument> StorageAnalyzerData { get; set; }
            public List<BsonDocument[]> DynamicData { get; set; }
            public List<BsonDocument> EventData { get; set; }
            public int GetMaxDataCount() {
                int[] array = new int[] {
                    StaticData.Count,
                    StorageAnalyzerData.Count,
                    DynamicData.Count,
                    EventData.Count
                };
                return array.Max();
            }
        }

        Int32 mockStartTimestamp;
        DeviceCount DeviceCount;
        MockDataStatic MockStatic;
        MockDataAnalyze MockDataAnalyze;
        MockDataDynamic MockDynamic;
        MockStorageFactory MockStorageFactory;
        MockEvent MockEvent;
        const int STORAGE_MAX_COUNT = 2;
        Random rnd;

        string[] GetMockStorageSN(int index, int count, out int StorageIndex) {
            StorageIndex = index;
            string[] ret = new string[count];
            for (var i=0; i<ret.Length; i++)
            {
                ret[i] = "BCADevice" + (StorageIndex).ToString("D5");
                StorageIndex++;
            }
            return ret;
        }

        protected void Mock(MockFunParameter mockFunParameter, out int DeviceIndex, out int StorageIndex)
        {
            DeviceIndex = mockFunParameter.DeviceIndex;
            StorageIndex = mockFunParameter.StorageIndex;
 
            foreach (var device in mockFunParameter.MockDevices)
            {
                uint total = device.Total;
                for (var i = 0; i < total; i++)
                {
                    var devName = "Device" + DeviceIndex.ToString("D5");
                    var storageCount = rnd.Next(1, STORAGE_MAX_COUNT +1);

                    MockStorageFactory.MockStorage[] mockStorages = new MockStorageFactory.MockStorage[storageCount];

                    for (var j=0; j < storageCount; j++) {
                        mockStorages[j] = MockStorageFactory.Mock(StorageIndex, rnd, mockFunParameter.TestingData.EventData, devName);
                        mockFunParameter.TestingData.StorageAnalyzerData.Add(MockDataAnalyze.GetMockData(mockStorages[j], mockStartTimestamp));
                        StorageIndex++;
                    }

                    mockFunParameter.TestingData.StaticData.Add(MockStatic.GetMockData(devName, mockStorages, device, DeviceIndex, mockStartTimestamp));
                    mockFunParameter.TestingData.DynamicData.Add(MockDynamic.GetMockData(devName, mockStorages, device, mockStartTimestamp));

                    if (i % 10 == 0)
                    {
                        MockEvent.MockOfflineEvent(mockFunParameter.TestingData.EventData, devName);
                    }

                    DeviceIndex++;
                }
            }
        }
        public void Start(TestingData TestingData)
        {
            int DeviceIndex = 1, StorageIndex = 1;
            Type type = typeof(DeviceCount);
            PropertyInfo[] properties = type.GetProperties();
            
            foreach (PropertyInfo property in properties)
            {
                Mock(new MockFunParameter()
                {
                    TestingData = TestingData,
                    MockDevices = (DeviceCount.MockDevice[])property.GetValue(DeviceCount, null),
                    DeviceIndex = DeviceIndex,
                    StorageIndex = StorageIndex,
                }, out DeviceIndex, out StorageIndex);
            }
        }
    }
}
