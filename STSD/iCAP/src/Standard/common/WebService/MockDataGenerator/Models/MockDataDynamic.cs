using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace MockDataCreate.Models
{
    public class MockDataDynamic
    {
        public MockDataDynamic(Random _rnd, int _mockStartTimestamp)
        {
            rnd = _rnd;
            timestamp = new int[DYNAMIC_DATA_COUNT]; 
            timestamp[0] = _mockStartTimestamp;
            for (var i = 1; i < DYNAMIC_DATA_COUNT; i++)
                timestamp[i] = timestamp[i - 1] + 60;
        }
        Random rnd;
        const int MAX_MEMORY_SIZE = 16777216;
        public const int DYNAMIC_DATA_COUNT = 20;
        BsonDocument[] IANALYZER = new BsonDocument[] {
            new BsonDocument()
            {
                {"Enable", 1},
                {"SRC", 2784002},
                {"RRC", 39870315},
                {"SWC", 1527066},
                {"RWC", 66674392},
                {"SR",new BsonDocument()
                        {
                            {"0", 17629},
                            {"1", 60490},
                            {"2", 732917},
                            {"3", 153909},
                            {"4", 727414},
                            {"5", 1091643},
                        }
                },
                {"SW",new BsonDocument()
                        {
                            {"0", 10397},
                            {"1", 52358},
                            {"2", 1230507},
                            {"3", 41972},
                            {"4", 54111},
                            {"5", 137721},
                        }
                },
                {"RR",new BsonDocument()
                        {
                            {"0",9466989},
                            {"1",7688031},
                            {"2",9023755},
                            {"3",1406997},
                            {"4",12284543}
                        }
                },
                {"RW",new BsonDocument()
                        {
                            {"0", 4381752},
                            {"1", 2522908},
                            {"2", 13497783},
                            {"3", 5396054},
                            {"4", 40875895}
                        }
                },
            },
            new BsonDocument(){
                {"Enable", 1},
                {"SRC", 6219462},
                {"RRC", 59308250},
                {"SWC", 2492221},
                {"RWC", 95052336},
                {"SR", new BsonDocument()
                    {
                        {"0", 58894},
                        {"1", 103215},
                        {"2", 1290914},
                        {"3", 326046},
                        {"4", 1399984},
                        {"5", 3040409},
                    }
                },
                {"SW", new BsonDocument()
                    {
                        {"0", 40656},
                        {"1", 124687},
                        {"2", 2038509},
                        {"3", 49115},
                        {"4", 69337},
                        {"5", 169917},
                    }
                },
                {"RR", new BsonDocument()
                    {
                        {"0",10632528},
                        {"1",13444033},
                        {"2",13309369},
                        {"3",1983239},
                        {"4",19939081}
                    }
                },
                {"RW", new BsonDocument()
                    {
                        {"0", 6450957},
                        {"1", 3623980},
                        {"2", 18445399},
                        {"3", 7694365},
                        {"4", 58837635}
                    }
                },
            },
        };
        int[] timestamp;

        BsonDocument GetCPU() {
            return new BsonDocument()
            {
                {"0",new BsonDocument()
                    {
                        {"Freq", rnd.Next(1000, 1500)},
                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
                        {"Temp", rnd.Next(20, 45)},
                        {"V", rnd.Next(8, 11)}
                    }
                },
                {"1",new BsonDocument()
                    {
                        {"Freq", rnd.Next(1000, 1500)},
                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
                        {"Temp", rnd.Next(20, 45)},
                        {"V", rnd.Next(8, 11)}
                    }
                },
                {"2",new BsonDocument()
                    {
                        {"Freq", rnd.Next(1000, 1500)},
                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
                        {"Temp", rnd.Next(20, 45)},
                        {"V", rnd.Next(8, 11)}
                    }
                },
                {"3",new BsonDocument()
                    {
                        {"Freq", rnd.Next(1000, 1500)},
                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
                        {"Temp", rnd.Next(20, 45)},
                        {"V", rnd.Next(8, 11)}
                    }
                },
                {"Freq", rnd.Next(1500, 3500)},
                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
                {"FanRPM", rnd.Next(1500, 2500)},   
            };
        }

        BsonDocument GetTX2CPU() {
            var CORE_STATUS = new string[2] { "on", "off" };
            var statusIndex = rnd.Next(0, 2);

            return new BsonDocument()
            {
                {"0",new BsonDocument()
                    {
                        {"Freq", rnd.Next(1000, 1500)},
                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
                        {"Temp", rnd.Next(20, 45)},
                        {"V", rnd.Next(8, 11)},
                        {"status", CORE_STATUS[statusIndex]},
                        {"frequency", (CORE_STATUS[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
                        {"loading", (CORE_STATUS[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
                    }
                },
                {"1",new BsonDocument()
                    {
                        {"Freq", rnd.Next(1000, 1500)},
                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
                        {"Temp", rnd.Next(20, 45)},
                        {"V", rnd.Next(8, 11)},
                        {"status", CORE_STATUS[statusIndex]},
                        {"frequency", (CORE_STATUS[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
                        {"loading", (CORE_STATUS[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
                    }
                },
                {"2",new BsonDocument()
                    {
                        {"Freq", rnd.Next(1000, 1500)},
                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
                        {"Temp", rnd.Next(20, 45)},
                        {"V", rnd.Next(8, 11)},
                        {"status", CORE_STATUS[statusIndex]},
                        {"frequency", (CORE_STATUS[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
                        {"loading", (CORE_STATUS[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
                    }
                },
                {"3",new BsonDocument()
                    {
                        {"Freq", rnd.Next(1000, 1500)},
                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
                        {"Temp", rnd.Next(20, 45)},
                        {"V", rnd.Next(8, 11)},
                        {"status", CORE_STATUS[statusIndex]},
                        {"frequency", (CORE_STATUS[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
                        {"loading", (CORE_STATUS[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
                    }
                },
                {"Freq", rnd.Next(1500, 3500)},
                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
                {"FanRPM", rnd.Next(1500, 2500)},
            };
        }
        public BsonDocument GetDynamicMemRawData()
        {
            return new BsonDocument()
            {
                { "memUsed", rnd.Next(0, MAX_MEMORY_SIZE+1)},
                { "temp", rnd.Next(-30, 101)}
            };
        }

        public BsonArray GetStorage(MockStorageFactory.MockStorage[] mockStorages, int dynamicDataIndex) {
            BsonArray ret = new BsonArray(); 

            for (var i = 0; i < mockStorages.Length; i++)
            {
               ret.Add(new BsonDocument() {
                        {"Index", i},
                        {"SN", mockStorages[i].StorageSN},
                        {"smart",new BsonDocument()
                            {
                                {"5", 0},
                                {"9", 4274},
                                {"12", 31},
                                {"163", 14},
                                {"165", 334},
                                {"167", mockStorages[i].AvgECList[dynamicDataIndex]},
                                {"169", 91},
                                {"170", 142},
                                {"171", 0},
                                {"172", 0},
                                {"192", 0},
                                {"194", mockStorages[i].TempData[dynamicDataIndex]},
                                {"229", 0},
                                {"235", 0 },
                                {"241", 85336},
                                {"242", 106513}
                            }
                        },
                        {"Health", mockStorages[i].HealthList[dynamicDataIndex]},
                        {"PECycle",3000},
                        {"iAnalyzer", IANALYZER[i % 2]},

                });
            }
            return ret;
        }
        public BsonDocument[] GetMockData(string deviceName, MockStorageFactory.MockStorage[] mockStorages, DeviceCount.MockDevice mockDevice, int mockStartTimestamp) {
            BsonDocument[] ret = new BsonDocument[DYNAMIC_DATA_COUNT];
            var spec = mockDevice.Spec;
            //var enableOOB = spec.OOB;
            var enableCANBus = spec.CANBus;
            var enableGPU = spec.GPU;

            for (var i=0; i < DYNAMIC_DATA_COUNT; i++)
            {
                ret[i] = new BsonDocument()
                {
                    {"Dev", deviceName},
                    {"CPU", enableGPU? GetTX2CPU() : GetCPU()},
                    {"mbProbe",new BsonDocument()},
                    {"MEM", GetDynamicMemRawData()},
                    {"Storage", GetStorage(mockStorages, i)},
                    {"Ext", enableCANBus ? new BsonDocument(){
                        {"0", 4136 },
                        {"1", 122 },
                        {"2", 87 },
                        {"3", 345 },
                        {"4", 276 },
                        {"5", 45 }
                    } : new BsonDocument()},
                    {"time", timestamp[i]},
                    {"GPU", enableGPU? new BsonDocument()
                        {
                            {"CoreClock", rnd.Next(140, 1122)},
                            {"Temp", Convert.ToDouble(rnd.Next(0, 99))},
                            {"MemUsed", rnd.Next(28, 99)},
                            {"Load", rnd.Next(0, 99)},
                            {"FanTemp", Convert.ToDouble(rnd.Next(0, 99))},
                        } : new BsonDocument()
                    },
                 };
            }

            return ret;
        }
    }
}
