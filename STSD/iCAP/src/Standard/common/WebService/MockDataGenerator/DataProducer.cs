using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary;
using ShareLibrary.AdminDB;
using MongoDB.Bson;
using System.Linq;
using System.Threading.Tasks;
using MockDataCreate.MyEventDataModel;
using MockDataCreate.Models;


namespace MockDataCreate
{
    public class DataProducer : IDataProducer
    {
        // AdminDB      
        public void DataMaker(ushort Count, Device[] TestingDevice)
        {

            for (int i = 1; i <= Count; i++)
            {
                Random rnd = new Random(Guid.NewGuid().GetHashCode());

                TestingDevice[i - 1] = new Device()
                {
                    Id = i,
                    Name = "Device" + i.ToString("D5"),
                    OwnerId = 1,
                    DeviceClassId = 1,
                    Latitude = 0,
                    Longitude = 0,
                    DeletedFlag = false,
                    PhotoUrl = "assets/images/devices/" + rnd.Next(1, 7).ToString("D2") + ".png"
                };
            }
        }
        public void DataMaker(DeviceCount deviceCount, Device[] TestingDevice)
        {
            int index = 1;
            uint TWCount = deviceCount.GetCount(deviceCount.TW);
            uint CNCount = deviceCount.GetCount(deviceCount.CN);
            uint JapanCount = deviceCount.GetCount(deviceCount.JP);
            uint USACount = deviceCount.GetCount(deviceCount.USA);
            uint NetherlandsCount = deviceCount.GetCount(deviceCount.NL);

            for (int i = 1; i <= TWCount; i++)
            {
                Random rnd = new Random(Guid.NewGuid().GetHashCode());

                TestingDevice[i - 1] = new Device()
                {
                    Id = i,
                    Name = "Device" + i.ToString("D5"),
                    OwnerId = 1,
                    DeviceClassId = 1,
                    Latitude = 0,
                    Longitude = 0,
                    DeletedFlag = false,
                    PhotoUrl = "assets/images/devices/" + rnd.Next(1, 7).ToString("D2") + ".png",
                    Alias = "TW" + index.ToString("D3")
                };
                index++;
            }

            index = 1;

            for (int i = (int)TWCount + 1; i <= TWCount + USACount; i++)
            {
                Random rnd = new Random(Guid.NewGuid().GetHashCode());

                TestingDevice[i - 1] = new Device()
                {
                    Id = i,
                    Name = "Device" + i.ToString("D5"),
                    OwnerId = 1,
                    DeviceClassId = 1,
                    Latitude = 0,
                    Longitude = 0,
                    DeletedFlag = false,
                    PhotoUrl = "assets/images/devices/" + rnd.Next(1, 7).ToString("D2") + ".png",
                    Alias = "US" + index.ToString("D3")
                };
                index++;
            }

            index = 1;

            for (uint i = TWCount + USACount + 1; i <= TWCount + USACount + JapanCount; i++)
            {
                Random rnd = new Random(Guid.NewGuid().GetHashCode());

                TestingDevice[i - 1] = new Device()
                {
                    Id = (int)i,
                    Name = "Device" + i.ToString("D5"),
                    OwnerId = 1,
                    DeviceClassId = 1,
                    Latitude = 0,
                    Longitude = 0,
                    DeletedFlag = false,
                    PhotoUrl = "assets/images/devices/" + rnd.Next(1, 7).ToString("D2") + ".png",
                    Alias = "JP" + index.ToString("D3")
                };
                index++;
            }

            index = 1;

            for (uint i = TWCount + USACount + JapanCount + 1; i <= TWCount + USACount + JapanCount + NetherlandsCount; i++)
            {
                Random rnd = new Random(Guid.NewGuid().GetHashCode());

                TestingDevice[i - 1] = new Device()
                {
                    Id = (int)i,
                    Name = "Device" + i.ToString("D5"),
                    OwnerId = 1,
                    DeviceClassId = 1,
                    Latitude = 0,
                    Longitude = 0,
                    DeletedFlag = false,
                    PhotoUrl = "assets/images/devices/" + rnd.Next(1, 7).ToString("D2") + ".png",
                    Alias = "NL" + index.ToString("D3")
                };
                index++;
            }

            index = 1;

            for (uint i = TWCount + USACount + JapanCount + NetherlandsCount + 1; i <= TWCount + USACount + JapanCount + NetherlandsCount + CNCount; i++)
            {
                Random rnd = new Random(Guid.NewGuid().GetHashCode());

                TestingDevice[i - 1] = new Device()
                {
                    Id = (int)i,
                    Name = "Device" + i.ToString("D5"),
                    OwnerId = 1,
                    DeviceClassId = 1,
                    Latitude = 0,
                    Longitude = 0,
                    DeletedFlag = false,
                    PhotoUrl = "assets/images/devices/" + rnd.Next(1, 7).ToString("D2") + ".png",
                    Alias = "CN" + index.ToString("D3")
                };
                index++;
            }
        }
        //public void DataMaker(uint Count, BranchDeviceList[] TestBranchDeviceElement)
        //{
        //    for (int i = 1; i <= Count; i++)
        //    {
        //        TestBranchDeviceElement[i - 1] = new BranchDeviceList()
        //        {
        //            Id = i,
        //            BranchId = 1,
        //            DeviceId = i,
        //        };
        //    }
        //}
        public void DataMaker(uint Count, Devicecertificate[] TestingDeviceCertificate)
        {
            for (int i = 1; i <= Count; i++)
            {
                TestingDeviceCertificate[i - 1] = new Devicecertificate
                {
                    Id = i,
                    DeviceId = i,
                    ExpiredDate = DateTime.UtcNow,
                    Password = "123",
                    Thumbprint = "ABCD" + i.ToString("D10"),
                    DeletedFlag = false
                };
            }
        }
        public void DataMaker(
            DeviceCount deviceCount,
            MockDataFactory.TestingData TestingData
        ) {
            var mockDataFactory = new MockDataFactory(deviceCount);

            mockDataFactory.Start(TestingData);
        }



        //public void DataMaker(DeviceCount deviceCount, BsonDocument[] TestingStaticData, BsonDocument[] TestingStorageAnalyzerData, BsonDocument[][] TestingDynamicData, BsonDocument[] TestingEventData, uint EventDeviceOfflineCount)
        //{
        //    //Static Raw Data

        //    //Setting
        //    string[] OSType = new string[] { "Microsoft Windows 10 Enterprise", "Microsoft Windows 10 Enterprise", "Microsoft Windows 10 Enterprise", "Ubuntu 16.04 LTS", "Ubuntu 16.04 LTS", "Ubuntu 16.04 LTS" };
        //    string[] OSVerType = new string[] { "x86_64", "x86_64", "x86_64", "16.04 LTS", "16.04 LTS", "16.04 LTS" };
        //    string[] MountAtType = new string[] { "C:\\", "D:\\", "E:\\", "sda", "sda", "sda" };
        //    uint Total = deviceCount.GetTotal();
        //    uint TWCount = deviceCount.GetCount(deviceCount.Taiwan);
        //    uint CNCount = deviceCount.GetCount(deviceCount.Cn);
        //    uint JapanCount = deviceCount.GetCount(deviceCount.Japan);
        //    uint USACount = deviceCount.GetCount(deviceCount.USA);
        //    uint NetherlandsCount = deviceCount.GetCount(deviceCount.Netherlands);
        //    int[] DeviceStorageCount = new int[Total];
        //    int OS = 0, storageSN = 0;            
        //    byte IP = 1;
        //    byte IP_3 = 1;
        //    double Longitude = 0.0, Latitude = 0.0;
        //    Int32 staticTime = 0, timestampNow = 0;
        //    Random rnd = new Random(Guid.NewGuid().GetHashCode());
        //    byte DynamicDataCount = 20, LifespanDataCount=20;           
        //    DateTime UtcNow = DateTime.UtcNow;                     

        //    timestampNow = (int)(UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        //    staticTime = (int)timestampNow - 1200;


        //    int maxValueTWX, maxValueTWY, maxValueUSAX, maxValueUSAY, maxValueJapanX, maxValueJapanY, maxValueNetherlandsX, maxValueNetherlandsY, maxValueCNX, maxValueCNY;

        //    const int minValueTWX = 121625363;
        //    const int minValueTWY = 25054599;-
        //    const int minValueUSAX = -121949999;
        //    const int minValueUSAY = 37512508;
        //    const int minValueJapanX = 139777546;
        //    const int minValueJapanY = 35681911;
        //    const int minValueNetherlandsX = 5497833;
        //    const int minValueNetherlandsY = 51438771;
        //    const int minValueCNX = 113911457;
        //    const int minValueCNY = 22510755;

        //    if (Total == 100)
        //    {
        //        maxValueTWX = 121643131;
        //        maxValueTWY = 25062997;
        //        maxValueUSAX = -121922533;
        //        maxValueUSAY = 37522958;
        //        maxValueJapanX = 139790292;
        //        maxValueJapanY = 35689231;
        //        maxValueNetherlandsX = 5510899;
        //        maxValueNetherlandsY = 51444945;
        //        maxValueCNX = 113925319;
        //        maxValueCNY = 22518655;
        //    } 
        //    else
        //    {
        //        maxValueTWX = 121667142;
        //        maxValueTWY = 25096113;
        //        maxValueUSAX = -121910731;
        //        maxValueUSAY = 37579675;
        //        maxValueJapanX = 139811342;
        //        maxValueJapanY = 35701990;
        //        maxValueNetherlandsX = 5508980;
        //        maxValueNetherlandsY = 51449105;
        //        maxValueCNX = 113944954;
        //        maxValueCNY = 22523750;
        //    }

        //    Make(deviceCount.Taiwan);

        //    ////xxx(deivceCount.)














        //    int i = 1;

        //    OS = rnd.Next(6);
        //    //DeviceStorageCount[i - 1] = rnd.Next(1, 3);// limit 2 storage
        //    Longitude = 121.634247; // innodisk location
        //    Latitude = 25.058798;
        //    ///first device
        //    DeviceStorageCount[0] = 1;
        //    TestingStaticData[0] = new BsonDocument()
        //                {
        //                    {"Dev", Dev(i.ToString("D5"))},
        //                    {"Sys",new BsonDocument()
        //                            {
        //                                {"OS", OSType[OS]},
        //                                {"OSVer", OSVerType[OS]},
        //                                {"OSArch", "64-bit"},
        //                                {"Name","innotest"},
        //                                {"Longitude",Longitude},
        //                                {"Latitude",Latitude}
        //                            }
        //                    },
        //                    {"CPU",new BsonDocument()
        //                            {
        //                                {"Manu", "GenuineIntel"},
        //                                {"Name", "Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz"},
        //                                {"Numofcore", 4},
        //                                {"L2", 1024 },
        //                                {"L3", 6144 }
        //                            }
        //                    },
        //                    {"MB",new BsonDocument()
        //                            {
        //                                {"Manu","Gigabyte Technology Co., Ltd."},
        //                                {"Product","Z270X-UD3-CF" },
        //                                {"SN","Default string" },
        //                                {"BIOSManu","American Megatrends Inc." },
        //                                {"BIOSVer","ALASKA - 1072009"},
        //                                {"mbTemp", new BsonDocument(){ }}
        //                            }
        //                    },
        //                    {"MEM", GetSPDMemoryMockData()
        //                    },
        //                    {"Storage",new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",0},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","B16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    }
        //                                }
        //                    },
        //                    {"Net", new BsonArray()
        //                            {
        //                                new BsonDocument()
        //                                {
        //                                    {"Name","eth0" },
        //                                    {"Type","Ethernet"},
        //                                    {"MAC","aa:bb:cc:dd:ee:ff"},
        //                                    {"IPv6","" },
        //                                    {"IPaddr","192.168." + (++IP_3).ToString() + "." + (++IP).ToString()},
        //                                    {"Netmask","255.255.255.0"}
        //                                }
        //                            }
        //                    },
        //                    {"Ext",new BsonDocument()
        //                            {
        //                                {"0", new BsonDocument()
        //                                    {
        //                                        {"Name", "Engine Speed"},
        //                                        {"Unit", "rpm"},
        //                                        {"Type", 0 }
        //                                    }
        //                                },
        //                                {"1", new BsonDocument()
        //                                    {
        //                                        {"Name", "Tachograph Vehicle Speed"},
        //                                        {"Unit", "km/h"},
        //                                        {"Type", 0 }
        //                                    }
        //                                },
        //                                {"2", new BsonDocument()
        //                                    {
        //                                        {"Name", "Engine Coolant Temperature"},
        //                                        {"Unit", "℃"},
        //                                        {"Type", 0 }
        //                                    }
        //                                },
        //                                {"3", new BsonDocument()
        //                                    {
        //                                        {"Name", "Engine Total Fuel Used"},
        //                                        {"Unit", "kg"},
        //                                        {"Type", 0 }
        //                                    }
        //                                },
        //                                {"4", new BsonDocument()
        //                                    {
        //                                        {"Name", "Boots Pressure"},
        //                                        {"Unit", "kpg"},
        //                                        {"Type", 0 }
        //                                    }
        //                                },
        //                                {"5", new BsonDocument()
        //                                    {
        //                                        {"Name", "Accelerator Pedal Position"},
        //                                        {"Unit", "%"},
        //                                        {"Type", 0 }
        //                                    }
        //                                }
        //                            }
        //                    },
        //                    {"Remote",new BsonDocument()
        //                                {
        //                                    {"0", new BsonDocument()
        //                                        {
        //                                            {"Name","iCoverX"},
        //                                            {"Unit","Trigger"},
        //                                            {"Type",3}
        //                                        }
        //                                    }
        //                                }
        //                    },
        //                    {"time", staticTime},
        //                };

        //    for ( i = 3; i <= TWCount; i++)
        //    {               
        //        OS = rnd.Next(6);
        //        DeviceStorageCount[i - 1] = rnd.Next(1, 3);// limit 2 storage;

        //        if (i < 4)
        //        {
        //            Longitude = 121.634247; // innodisk location
        //            Latitude = 25.058798;
        //        }
        //        else
        //        {
        //            Longitude = rnd.Next(minValueTWX, maxValueTWX) / 1000000.0;
        //            Latitude = rnd.Next(minValueTWY, maxValueTWY) / 1000000.0;
        //        }

        //        if (i <= 11)
        //        {
        //            //add Tx2 Mock Data
        //            switch (DeviceStorageCount[i - 1])
        //            {
        //                case 1:
        //                    TestingStaticData[i - 1] = new BsonDocument()
        //                    {
        //                        {"Dev", Dev(i.ToString("D5"))},
        //                        {"Sys",new BsonDocument()
        //                                {
        //                                    {"OS", "Ubuntu 16.04.5 LTS"},
        //                                    {"OSVer", "16.04.5 LTS (Xenial Xerus)"},
        //                                    {"OSArch", "Linux 4.4.38-tegra aarch64"},
        //                                    {"Name","tegra-ubuntu"},
        //                                    {"Longitude",Longitude},
        //                                    {"Latitude",Latitude}
        //                                }
        //                        },
        //                        {"CPU",new BsonDocument()
        //                                {
        //                                    {"Manu", ""},
        //                                    {"Name", "ARMv8 Processor rev 3 (v8l)"},
        //                                    {"Numofcore", 4},
        //                                    {"L2", 2048 },
        //                                    {"L3", 0 }
        //                                }
        //                        },
        //                        {"MB",new BsonDocument()
        //                                {
        //                                    {"Manu", "Aetina Corporation."},
        //                                    {"Product", "ACE-N310" },
        //                                    {"SN", "012101705311" + i},
        //                                    {"BIOSManu", ""},
        //                                    {"BIOSVer", ""},
        //                                    {"mbTemp", new BsonDocument(){ }}
        //                                }
        //                        },
        //                        {"MEM", GetSPDMemoryMockData()
        //                        },
        //                        {"Storage",new BsonArray()
        //                                    {
        //                                        new BsonDocument()
        //                                        {
        //                                            {"Index",0},
        //                                            {"Model","2.5\"SATA SSD 3ME3"},
        //                                            {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                            {"FWVer","S16425"},
        //                                            {"Par", new BsonDocument()
        //                                                    {
        //                                                        {"TotalCap", 250051725},
        //                                                        {"NumofPar", 1 },
        //                                                        {"ParInfo",new BsonArray()
        //                                                                    {
        //                                                                        new BsonDocument()
        //                                                                        {
        //                                                                            {"MountAt", ""},
        //                                                                            {"Capacity", 249544704}
        //                                                                        }
        //                                                                    }
        //                                                        }
        //                                                    }
        //                                            }
        //                                        }
        //                                    }
        //                        },
        //                        {"Net", new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Name","eth0" },
        //                                        {"Type","Ethernet"},
        //                                        {"MAC","aa:bb:cc:dd:ee:ff"},
        //                                        {"IPv6","" },
        //                                        {"IPaddr","192.168." + (++IP_3).ToString() + "." + (++IP).ToString()},
        //                                        {"Netmask","255.255.255.0"}
        //                                    }
        //                                }
        //                        },
        //                        {"Ext", new BsonDocument(){ }},
        //                        {"Remote", new BsonDocument(){ }},
        //                        {"time", staticTime},
        //                        {"GPU",new BsonDocument()
        //                                {
        //                                    {"Name", "NVIDIA Tegra X2"},
        //                                    {"Arch", "NVIDIA Pascal™"},
        //                                    {"DriverVer", "9.0"},
        //                                    {"ComputeCap", "6.2"},
        //                                    {"CoreNum", "256"},
        //                                    {"MemType", "LDDR4"},
        //                                    {"MemBusWidth", "128-bit"},
        //                                    {"MemSize", "7846 MB"},
        //                                    {"MemBandWidth", "59.7 GB/s"},
        //                                    {"Clock", "1301 MHz"},
        //                                    {"MemClock", "1600MHz"}
        //                                }
        //                        },
        //                    };
        //                    break;
        //                case 2:
        //                    TestingStaticData[i - 1] = new BsonDocument()
        //                    {
        //                        {"Dev", Dev(i.ToString("D5"))},
        //                        {"Sys",new BsonDocument()
        //                                {
        //                                    {"OS", "Ubuntu 16.04.5 LTS"},
        //                                    {"OSVer", "16.04.5 LTS (Xenial Xerus)"},
        //                                    {"OSArch", "Linux 4.4.38-tegra aarch64"},
        //                                    {"Name","tegra-ubuntu"},
        //                                    {"Longitude",Longitude},
        //                                    {"Latitude",Latitude}
        //                                }
        //                        },
        //                        {"CPU",new BsonDocument()
        //                                {
        //                                    {"Manu", ""},
        //                                    {"Name", "ARMv8 Processor rev 3 (v8l)"},
        //                                    {"Numofcore", 4},
        //                                    {"L2", 2048 },
        //                                    {"L3", 0 }
        //                                }
        //                        },
        //                        {"MB",new BsonDocument()
        //                                {
        //                                    {"Manu", "Aetina Corporation."},
        //                                    {"Product", "ACE-N310" },
        //                                    {"SN", "012101705311" + i},
        //                                    {"BIOSManu", ""},
        //                                    {"BIOSVer", ""},
        //                                    {"mbTemp", new BsonDocument(){ }}
        //                                }
        //                        },
        //                        {"MEM", GetSPDMemoryMockData()
        //                        },
        //                        {"Storage",new BsonArray()
        //                                    {
        //                                        new BsonDocument()
        //                                        {
        //                                            {"Index",0},
        //                                            {"Model","2.5\"SATA SSD 3ME3"},
        //                                            {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                            {"FWVer","S16425"},
        //                                            {"Par", new BsonDocument()
        //                                                    {
        //                                                        {"TotalCap",250051725},
        //                                                        {"NumofPar",1 },
        //                                                        {"ParInfo",new BsonArray()
        //                                                                    {
        //                                                                        new BsonDocument()
        //                                                                        {
        //                                                                            {"MountAt",MountAtType[OS]},
        //                                                                            {"Capacity",249544704}
        //                                                                        }
        //                                                                    }
        //                                                        }
        //                                                    }
        //                                            }
        //                                        },
        //                                        new BsonDocument()
        //                                        {
        //                                            {"Index",1},
        //                                            {"Model","2.5\"SATA SSD 3ME3"},
        //                                            {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                            {"FWVer","S16425"},
        //                                            {"Par", new BsonDocument()
        //                                                    {
        //                                                        {"TotalCap",250051725},
        //                                                        {"NumofPar",1 },
        //                                                        {"ParInfo",new BsonArray()
        //                                                                    {
        //                                                                        new BsonDocument()
        //                                                                        {
        //                                                                            {"MountAt",MountAtType[OS]},
        //                                                                            {"Capacity",249544704}
        //                                                                        }
        //                                                                    }
        //                                                        }
        //                                                    }
        //                                            }
        //                                        }
        //                                    }
        //                        },
        //                        {"Net", new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Name","eth0" },
        //                                        {"Type","Ethernet"},
        //                                        {"MAC","aa:bb:cc:dd:ee:ff"},
        //                                        {"IPv6","" },
        //                                        {"IPaddr","192.168." + (++IP_3).ToString() + "." + (++IP).ToString()},
        //                                        {"Netmask","255.255.255.0"}
        //                                    }
        //                                }
        //                        },
        //                        {"Ext",new BsonDocument(){ }},
        //                        {"Remote",new BsonDocument(){ }},
        //                        {"time",staticTime},
        //                        {"GPU", new BsonDocument()
        //                                {
        //                                    {"Name", "NVIDIA Tegra X2"},
        //                                    {"Arch", "NVIDIA Pascal™"},
        //                                    {"DriverVer", "9.0"},
        //                                    {"ComputeCap", "6.2"},
        //                                    {"CoreNum", "256"},
        //                                    {"MemType", "LDDR4"},
        //                                    {"MemBusWidth", "128-bit"},
        //                                    {"MemSize", "7846 MB"},
        //                                    {"MemBandWidth", "59.7 GB/s"},
        //                                    {"Clock", "1301 MHz"},
        //                                    {"MemClock", "1600MHz"}
        //                                }
        //                        },
        //                    };
        //                    break;
        //            }
        //            continue;
        //        }


        //        switch (DeviceStorageCount[i - 1])
        //        {
        //            case 1:
        //                TestingStaticData[i - 1] = new BsonDocument()
        //                {
        //                    {"Dev", Dev(i.ToString("D5"))},
        //                    {"Sys",new BsonDocument()
        //                            {
        //                                {"OS", OSType[OS]},
        //                                {"OSVer", OSVerType[OS]},
        //                                {"OSArch", "64-bit"},
        //                                {"Name","innotest"},
        //                                {"Longitude",Longitude},
        //                                {"Latitude",Latitude}
        //                            }
        //                    },
        //                    {"CPU",new BsonDocument()
        //                            {
        //                                {"Manu", "GenuineIntel"},
        //                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz"},
        //                                {"Numofcore",4},
        //                                {"L2", 1024 },
        //                                {"L3", 6144 }
        //                            }
        //                    },
        //                    {"MB",new BsonDocument()
        //                            {
        //                                {"Manu","Gigabyte Technology Co., Ltd."},
        //                                {"Product","Z270X-UD3-CF" },
        //                                {"SN","Default string" },
        //                                {"BIOSManu","American Megatrends Inc." },
        //                                {"BIOSVer","ALASKA - 1072009"},
        //                                {"mbTemp", new BsonDocument(){ }}
        //                            }
        //                    },
        //                    {"MEM", GetSPDMemoryMockData()
        //                    },
        //                    {"Storage",new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",0},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    }
        //                                }
        //                    },
        //                    {"Net", new BsonArray()
        //                            {
        //                                new BsonDocument()
        //                                {
        //                                    {"Name","eth0" },
        //                                    {"Type","Ethernet"},
        //                                    {"MAC","aa:bb:cc:dd:ee:ff"},
        //                                    {"IPv6","" },
        //                                    {"IPaddr","192.168." + (++IP_3).ToString() + "." + (++IP).ToString()},
        //                                    {"Netmask","255.255.255.0"}
        //                                }
        //                            }
        //                    },
        //                    {"Ext",new BsonDocument(){ }},
        //                    {"Remote",new BsonDocument(){ }},
        //                    {"time",staticTime}
        //                };
        //                break;
        //            case 2:
        //                TestingStaticData[i - 1] = new BsonDocument()
        //                {
        //                    {"Dev", Dev(i.ToString("D5"))},
        //                    {"Sys",new BsonDocument()
        //                            {
        //                                {"OS", OSType[OS]},
        //                                {"OSVer", OSVerType[OS]},
        //                                {"OSArch", "64-bit"},
        //                                {"Name","innotest"},
        //                                {"Longitude",Longitude},
        //                                {"Latitude",Latitude}
        //                            }
        //                    },
        //                    {"CPU",new BsonDocument()
        //                            {
        //                                {"Manu", "GenuineIntel"},
        //                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz"},
        //                                {"Numofcore",4},
        //                                {"L2",1024 },
        //                                {"L3",6144 }
        //                            }
        //                    },
        //                    {"MB",new BsonDocument()
        //                            {
        //                                {"Manu","Gigabyte Technology Co., Ltd."},
        //                                {"Product","Z270X-UD3-CF" },
        //                                {"SN","Default string" },
        //                                {"BIOSManu","American Megatrends Inc." },
        //                                {"BIOSVer","ALASKA - 1072009"},
        //                                {"mbTemp", new BsonDocument(){ }}
        //                            }
        //                    },
        //                    {"MEM", GetSPDMemoryMockData()
        //                    },
        //                    {"Storage",new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",0},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    },
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",1},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    }
        //                                }
        //                    },
        //                    {"Net", new BsonArray()
        //                            {
        //                                new BsonDocument()
        //                                {
        //                                    {"Name","eth0" },
        //                                    {"Type","Ethernet"},
        //                                    {"MAC","aa:bb:cc:dd:ee:ff"},
        //                                    {"IPv6","" },
        //                                    {"IPaddr","192.168." + (++IP_3).ToString() + "." + (++IP).ToString()},
        //                                    {"Netmask","255.255.255.0"}
        //                                }
        //                            }
        //                    },
        //                    {"Ext",new BsonDocument(){ }},
        //                    {"Remote",new BsonDocument(){ }},
        //                    {"time",staticTime}
        //                };
        //                break;
        //        }
        //    }
        //    //USA Device

        //    for (i = (int)TWCount + 1; i <= TWCount + USACount; i++)
        //    {
        //        OS = rnd.Next(6);
        //        DeviceStorageCount[i - 1] = rnd.Next(1, 3);// limit 2 storage;
        //        Longitude = rnd.Next(minValueUSAX, maxValueUSAX) / 1000000.0;
        //        Latitude = rnd.Next(minValueUSAY, maxValueUSAY) / 1000000.0;

        //        switch (DeviceStorageCount[i - 1])
        //        {
        //            case 1:
        //                TestingStaticData[i - 1] = new BsonDocument()
        //                {
        //                    {"Dev", Dev(i.ToString("D5"))},
        //                    {"Sys",new BsonDocument()
        //                            {
        //                                {"OS", OSType[OS]},
        //                                {"OSVer", OSVerType[OS]},
        //                                {"OSArch", "64-bit"},
        //                                {"Name","innotest"},
        //                                {"Longitude",Longitude},
        //                                {"Latitude",Latitude}
        //                            }
        //                    },
        //                    {"CPU",new BsonDocument()
        //                            {
        //                                {"Manu", "GenuineIntel"},
        //                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz"},
        //                                {"Numofcore",4},
        //                                {"L2",1024 },
        //                                {"L3",6144 }
        //                            }
        //                    },
        //                    {"MB",new BsonDocument()
        //                            {
        //                                {"Manu","Gigabyte Technology Co., Ltd."},
        //                                {"Product","Z270X-UD3-CF" },
        //                                {"SN","Default string" },
        //                                {"BIOSManu","American Megatrends Inc." },
        //                                {"BIOSVer","ALASKA - 1072009"},
        //                                {"mbTemp", new BsonDocument(){ }}
        //                            }
        //                    },
        //                    {"MEM", GetSPDMemoryMockData()
        //                    },
        //                    {"Storage",new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",0},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    }
        //                                }
        //                    },
        //                    {"Net", new BsonArray()
        //                            {
        //                                new BsonDocument()
        //                                {
        //                                    {"Name","eth0" },
        //                                    {"Type","Ethernet"},
        //                                    {"MAC","aa:bb:cc:dd:ee:ff"},
        //                                    {"IPv6","" },
        //                                    {"IPaddr","192.168." + (++IP_3).ToString() + "." + (++IP).ToString()},
        //                                    {"Netmask","255.255.255.0"}
        //                                }
        //                            }
        //                    },
        //                    {"Ext",new BsonDocument(){ }},
        //                    {"Remote",new BsonDocument(){ }},
        //                    {"time",staticTime}
        //                };
        //                break;
        //            case 2:
        //                TestingStaticData[i - 1] = new BsonDocument()
        //                {
        //                    {"Dev", Dev(i.ToString("D5"))},
        //                    {"Sys",new BsonDocument()
        //                            {
        //                                {"OS", OSType[OS]},
        //                                {"OSVer", OSVerType[OS]},
        //                                {"OSArch", "64-bit"},
        //                                {"Name","innotest"},
        //                                {"Longitude",Longitude},
        //                                {"Latitude",Latitude}
        //                            }
        //                    },
        //                    {"CPU",new BsonDocument()
        //                            {
        //                                {"Manu", "GenuineIntel"},
        //                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz"},
        //                                {"Numofcore",4},
        //                                {"L2",1024 },
        //                                {"L3",6144 }
        //                            }
        //                    },
        //                    {"MB",new BsonDocument()
        //                            {
        //                                {"Manu","Gigabyte Technology Co., Ltd."},
        //                                {"Product","Z270X-UD3-CF" },
        //                                {"SN","Default string" },
        //                                {"BIOSManu","American Megatrends Inc." },
        //                                {"BIOSVer","ALASKA - 1072009"},
        //                                {"mbTemp", new BsonDocument(){ }}
        //                            }
        //                    },
        //                    {"MEM", GetSPDMemoryMockData()
        //                    },
        //                    {"Storage",new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",0},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    },
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",1},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    }
        //                                }
        //                    },
        //                    {"Net", new BsonArray()
        //                            {
        //                                new BsonDocument()
        //                                {
        //                                    {"Name","eth0" },
        //                                    {"Type","Ethernet"},
        //                                    {"MAC","aa:bb:cc:dd:ee:ff"},
        //                                    {"IPv6","" },
        //                                    {"IPaddr","192.168." + (++IP_3).ToString() + "." + (++IP).ToString()},
        //                                    {"Netmask","255.255.255.0"}
        //                                }
        //                            }
        //                    },
        //                    {"Ext",new BsonDocument(){ }},
        //                    {"Remote",new BsonDocument(){ }},
        //                    {"time",staticTime}
        //                };
        //                break;
        //        }
        //    }
        //    //Japan Device

        //    for (i = (int)TWCount + (int)USACount + 1; i <= TWCount + USACount + JapanCount; i++)
        //    {
        //        OS = rnd.Next(6);
        //        DeviceStorageCount[i - 1] = rnd.Next(1, 3);// limit 2 storage;
        //        Longitude = rnd.Next(minValueJapanX, maxValueJapanX) / 1000000.0;
        //        Latitude = rnd.Next(minValueJapanY, maxValueJapanY) / 1000000.0;

        //        switch (DeviceStorageCount[i - 1])
        //        {
        //            case 1:
        //                TestingStaticData[i - 1] = new BsonDocument()
        //                {
        //                    {"Dev", Dev(i.ToString("D5"))},
        //                    {"Sys",new BsonDocument()
        //                            {
        //                                {"OS", OSType[OS]},
        //                                {"OSVer", OSVerType[OS]},
        //                                {"OSArch", "64-bit"},
        //                                {"Name","innotest"},
        //                                {"Longitude",Longitude},
        //                                {"Latitude",Latitude}
        //                            }
        //                    },
        //                    {"CPU",new BsonDocument()
        //                            {
        //                                {"Manu", "GenuineIntel"},
        //                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz"},
        //                                {"Numofcore",4},
        //                                {"L2",1024 },
        //                                {"L3",6144 }
        //                            }
        //                    },
        //                    {"MB",new BsonDocument()
        //                            {
        //                                {"Manu","Gigabyte Technology Co., Ltd."},
        //                                {"Product","Z270X-UD3-CF" },
        //                                {"SN","Default string" },
        //                                {"BIOSManu","American Megatrends Inc." },
        //                                {"BIOSVer","ALASKA - 1072009"},
        //                                {"mbTemp", new BsonDocument(){ }}
        //                            }
        //                    },
        //                    {"MEM", GetSPDMemoryMockData()
        //                    },
        //                    {"Storage",new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",0},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    }
        //                                }
        //                    },
        //                    {"Net", new BsonArray()
        //                            {
        //                                new BsonDocument()
        //                                {
        //                                    {"Name","eth0" },
        //                                    {"Type","Ethernet"},
        //                                    {"MAC","aa:bb:cc:dd:ee:ff"},
        //                                    {"IPv6","" },
        //                                    {"IPaddr","192.168." + (++IP_3).ToString() + "." + (++IP).ToString()},
        //                                    {"Netmask","255.255.255.0"}
        //                                }
        //                            }
        //                    },
        //                    {"Ext",new BsonDocument(){ }},
        //                    {"Remote",new BsonDocument(){ }},
        //                    {"time",staticTime}
        //                };
        //                break;
        //            case 2:
        //                TestingStaticData[i - 1] = new BsonDocument()
        //                {
        //                    {"Dev", Dev(i.ToString("D5"))},
        //                    {"Sys",new BsonDocument()
        //                            {
        //                                {"OS", OSType[OS]},
        //                                {"OSVer", OSVerType[OS]},
        //                                {"OSArch", "64-bit"},
        //                                {"Name","innotest"},
        //                                {"Longitude",Longitude},
        //                                {"Latitude",Latitude}
        //                            }
        //                    },
        //                    {"CPU",new BsonDocument()
        //                            {
        //                                {"Manu", "GenuineIntel"},
        //                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz"},
        //                                {"Numofcore",4},
        //                                {"L2",1024 },
        //                                {"L3",6144 }
        //                            }
        //                    },
        //                    {"MB",new BsonDocument()
        //                            {
        //                                {"Manu","Gigabyte Technology Co., Ltd."},
        //                                {"Product","Z270X-UD3-CF" },
        //                                {"SN","Default string" },
        //                                {"BIOSManu","American Megatrends Inc." },
        //                                {"BIOSVer","ALASKA - 1072009"},
        //                                {"mbTemp", new BsonDocument(){ }}
        //                            }
        //                    },
        //                    {"MEM", GetSPDMemoryMockData()
        //                    },
        //                    {"Storage",new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",0},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    },
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",1},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    }
        //                                }
        //                    },
        //                    {"Net", new BsonArray()
        //                            {
        //                                new BsonDocument()
        //                                {
        //                                    {"Name","eth0" },
        //                                    {"Type","Ethernet"},
        //                                    {"MAC","aa:bb:cc:dd:ee:ff"},
        //                                    {"IPv6","" },
        //                                    {"IPaddr","192.168." + (++IP_3).ToString() + "." + (++IP).ToString()},
        //                                    {"Netmask","255.255.255.0"}
        //                                }
        //                            }
        //                    },
        //                    {"Ext",new BsonDocument(){ }},
        //                    {"Remote",new BsonDocument(){ }},
        //                    {"time",staticTime}
        //                };
        //                break;
        //        }
        //    }
        //    //Netherlands Device

        //    for ( i = (int)TWCount + (int)USACount + (int)JapanCount + 1; i <= TWCount + USACount + JapanCount + NetherlandsCount; i++)
        //    {
        //        OS = rnd.Next(6);
        //        DeviceStorageCount[i - 1] = rnd.Next(1, 3);// limit 2 storage;
        //        Longitude = rnd.Next(minValueNetherlandsX, maxValueNetherlandsX) / 1000000.0;
        //        Latitude = rnd.Next(minValueNetherlandsY, maxValueNetherlandsY) / 1000000.0;

        //        switch (DeviceStorageCount[i - 1])
        //        {
        //            case 1:
        //                TestingStaticData[i - 1] = new BsonDocument()
        //                {
        //                    {"Dev", Dev(i.ToString("D5"))},
        //                    {"Sys",new BsonDocument()
        //                            {
        //                                {"OS", OSType[OS]},
        //                                {"OSVer", OSVerType[OS]},
        //                                {"OSArch", "64-bit"},
        //                                {"Name","innotest"},
        //                                {"Longitude",Longitude},
        //                                {"Latitude",Latitude}
        //                            }
        //                    },
        //                    {"CPU",new BsonDocument()
        //                            {
        //                                {"Manu", "GenuineIntel"},
        //                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz"},
        //                                {"Numofcore",4},
        //                                {"L2",1024 },
        //                                {"L3",6144 }
        //                            }
        //                    },
        //                    {"MB",new BsonDocument()
        //                            {
        //                                {"Manu","Gigabyte Technology Co., Ltd."},
        //                                {"Product","Z270X-UD3-CF" },
        //                                {"SN","Default string" },
        //                                {"BIOSManu","American Megatrends Inc." },
        //                                {"BIOSVer","ALASKA - 1072009"},
        //                                {"mbTemp", new BsonDocument(){ }}
        //                            }
        //                    },
        //                    {"MEM", GetSPDMemoryMockData()
        //                    },
        //                    {"Storage",new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",0},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    }
        //                                }
        //                    },
        //                    {"Net", new BsonArray()
        //                            {
        //                                new BsonDocument()
        //                                {
        //                                    {"Name","eth0" },
        //                                    {"Type","Ethernet"},
        //                                    {"MAC","aa:bb:cc:dd:ee:ff"},
        //                                    {"IPv6","" },
        //                                    {"IPaddr","192.168." + (++IP_3).ToString() + "." + (++IP).ToString()},
        //                                    {"Netmask","255.255.255.0"}
        //                                }
        //                            }
        //                    },
        //                    {"Ext",new BsonDocument(){ }},
        //                    {"Remote",new BsonDocument(){ }},
        //                    {"time",staticTime}
        //                };
        //                break;
        //            case 2:
        //                TestingStaticData[i - 1] = new BsonDocument()
        //                {
        //                    { "Dev", Dev(i.ToString("D5"))},
        //                    {"Sys",new BsonDocument()
        //                            {
        //                                {"OS", OSType[OS]},
        //                                {"OSVer", OSVerType[OS]},
        //                                {"OSArch", "64-bit"},
        //                                {"Name","innotest"},
        //                                {"Longitude",Longitude},
        //                                {"Latitude",Latitude}
        //                            }
        //                    },
        //                    {"CPU",new BsonDocument()
        //                            {
        //                                {"Manu", "GenuineIntel"},
        //                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz"},
        //                                {"Numofcore",4},
        //                                {"L2",1024 },
        //                                {"L3",6144 }
        //                            }
        //                    },
        //                    {"MB",new BsonDocument()
        //                            {
        //                                {"Manu","Gigabyte Technology Co., Ltd."},
        //                                {"Product","Z270X-UD3-CF" },
        //                                {"SN","Default string" },
        //                                {"BIOSManu","American Megatrends Inc." },
        //                                {"BIOSVer","ALASKA - 1072009"},
        //                                {"mbTemp", new BsonDocument(){ }}
        //                            }
        //                    },
        //                    {"MEM", GetSPDMemoryMockData()
        //                    },
        //                    {"Storage",new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",0},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    },
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",1},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    }
        //                                }
        //                    },
        //                    {"Net", new BsonArray()
        //                            {
        //                                new BsonDocument()
        //                                {
        //                                    {"Name","eth0" },
        //                                    {"Type","Ethernet"},
        //                                    {"MAC","aa:bb:cc:dd:ee:ff"},
        //                                    {"IPv6","" },
        //                                    {"IPaddr","192.168." + (++IP_3).ToString() + "." + (++IP).ToString()},
        //                                    {"Netmask","255.255.255.0"}
        //                                }
        //                            }
        //                    },
        //                    {"Ext",new BsonDocument(){ }},
        //                    {"Remote",new BsonDocument(){ }},
        //                    {"time",staticTime}
        //                };
        //                break;
        //        }
        //    }

        //    //CN Device

        //    for (i = (int)TWCount + (int)USACount + (int)JapanCount + (int)NetherlandsCount + 1 ; i <= TWCount + USACount + JapanCount + NetherlandsCount + CNCount; i++)
        //    {
        //        OS = rnd.Next(6);
        //        DeviceStorageCount[i - 1] = rnd.Next(1, 3);// limit 2 storage;
        //        Longitude = rnd.Next(minValueCNX, maxValueCNX) / 1000000.0;
        //        Latitude = rnd.Next(minValueCNY, maxValueCNY) / 1000000.0;

        //        switch (DeviceStorageCount[i - 1])
        //        {
        //            case 1:
        //                TestingStaticData[i - 1] = new BsonDocument()
        //                {
        //                    {"Dev", Dev(i.ToString("D5"))},
        //                    {"Sys",new BsonDocument()
        //                            {
        //                                {"OS", OSType[OS]},
        //                                {"OSVer", OSVerType[OS]},
        //                                {"OSArch", "64-bit"},
        //                                {"Name","innotest"},
        //                                {"Longitude",Longitude},
        //                                 {"Latitude",Latitude}
        //                            }
        //                    },
        //                    {"CPU",new BsonDocument()
        //                            {
        //                                {"Manu", "GenuineIntel"},
        //                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz"},
        //                                {"Numofcore",4},
        //                                {"L2",1024 },
        //                                {"L3",6144 }
        //                            }
        //                    },
        //                    {"MB",new BsonDocument()
        //                            {
        //                                {"Manu","Gigabyte Technology Co., Ltd."},
        //                                {"Product","Z270X-UD3-CF" },
        //                                {"SN","Default string" },
        //                                {"BIOSManu","American Megatrends Inc." },
        //                                {"BIOSVer","ALASKA - 1072009"},
        //                                {"mbTemp", new BsonDocument(){ }}
        //                            }
        //                    },
        //                    {"MEM", GetSPDMemoryMockData()
        //                    },

        //                    {"Storage",new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",0},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    }
        //                                }
        //                    },
        //                    {"Net", new BsonArray()
        //                            {
        //                                new BsonDocument()
        //                                {
        //                                    {"Name","eth0" },
        //                                    {"Type","Ethernet"},
        //                                    {"MAC","aa:bb:cc:dd:ee:ff"},
        //                                    {"IPv6","" },
        //                                    {"IPaddr","192.168." + (++IP_3).ToString() + "." + (++IP).ToString()},
        //                                    {"Netmask","255.255.255.0"}
        //                                }
        //                            }
        //                    },
        //                    {"Ext",new BsonDocument(){ }},
        //                    {"Remote",new BsonDocument(){ }},
        //                    {"time",staticTime}
        //                };
        //                break;
        //            case 2:
        //                TestingStaticData[i - 1] = new BsonDocument()
        //                {
        //                    {"Dev", Dev(i.ToString("D5"))},
        //                    {"Sys",new BsonDocument()
        //                            {
        //                                {"OS", OSType[OS]},
        //                                {"OSVer", OSVerType[OS]},
        //                                {"OSArch", "64-bit"},
        //                                {"Name","innotest"},
        //                                {"Longitude",Longitude},
        //                                {"Latitude",Latitude}
        //                            }
        //                    },
        //                    {"CPU",new BsonDocument()
        //                            {
        //                                {"Manu", "GenuineIntel"},
        //                                {"Name","Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz"},
        //                                {"Numofcore",4},
        //                                {"L2",1024 },
        //                                {"L3",6144 }
        //                            }
        //                    },
        //                    {"MB",new BsonDocument()
        //                            {
        //                                {"Manu","Gigabyte Technology Co., Ltd."},
        //                                {"Product","Z270X-UD3-CF" },
        //                                {"SN","Default string" },
        //                                {"BIOSManu","American Megatrends Inc." },
        //                                {"BIOSVer","ALASKA - 1072009"},
        //                                {"mbTemp", new BsonDocument(){ }}
        //                            }
        //                    },
        //                    {"MEM", GetSPDMemoryMockData()
        //                    },
        //                    {"Storage",new BsonArray()
        //                                {
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",0},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    },
        //                                    new BsonDocument()
        //                                    {
        //                                        {"Index",1},
        //                                        {"Model","2.5\"SATA SSD 3ME3"},
        //                                        {"SN","BCADevice"+(++storageSN).ToString("D5")},
        //                                        {"FWVer","S16425"},
        //                                        {"Par", new BsonDocument()
        //                                                {
        //                                                    {"TotalCap",250051725},
        //                                                    {"NumofPar",1 },
        //                                                    {"ParInfo",new BsonArray()
        //                                                                {
        //                                                                    new BsonDocument()
        //                                                                    {
        //                                                                        {"MountAt",MountAtType[OS]},
        //                                                                        {"Capacity",249544704}
        //                                                                    }
        //                                                                }
        //                                                    }
        //                                                }
        //                                        }
        //                                    }
        //                                }
        //                    },
        //                    {"Net", new BsonArray()
        //                            {
        //                                new BsonDocument()
        //                                {
        //                                    {"Name","eth0" },
        //                                    {"Type","Ethernet"},
        //                                    {"MAC","aa:bb:cc:dd:ee:ff"},
        //                                    {"IPv6","" },
        //                                    {"IPaddr","192.168." + (++IP_3).ToString() + "." + (++IP).ToString()},
        //                                    {"Netmask","255.255.255.0"}
        //                                }
        //                            }
        //                    },
        //                    {"Ext",new BsonDocument(){ }},
        //                    {"Remote",new BsonDocument(){ }},
        //                    {"time",staticTime}
        //                };
        //                break;
        //        }
        //    }

        //    //Storage Analyzer Data
        //    //Setting
        //    double[] lifespantime = new double[LifespanDataCount];

        //    lifespantime[0] = timestampNow - 1728000.0;
        //    for ( i=1;i< LifespanDataCount;i++)
        //    {
        //        lifespantime[i] = lifespantime[i - 1] + 86400.0;
        //    }

        //    List<double[]> HealthList = new List<double[]>();
        //    List<int[]> AvgECList = new List<int[]>();
        //    int index = 0,storageAnalyzerIndex=0;
        //    int lifespanThreshold = 150;
            List<EventLife> elList = new List<EventLife>();

        //    //Make
        //    for(int deviceSN=0; deviceSN < Total; deviceSN++)
        //        for (i = 1; i <= DeviceStorageCount[deviceSN]; i++)//i = storage count
        //        {
        //            int[] data;

        //            index = 0;
        //            if (deviceSN % 5 == 0)
        //            {
        //                data = Enumerable.Repeat(0, 20).Select(j => rnd.Next(20, 2450)).ToArray();
        //            }
        //            else if (deviceSN % 2 == 0)
        //            {
        //                data = Enumerable.Repeat(0, 20).Select(j => rnd.Next(1800, 2450)).ToArray();
        //            }
        //            else
        //            {
        //                data = Enumerable.Repeat(0, 20).Select(j => rnd.Next(2100, 2450)).ToArray();
        //            }

        //            Array.Sort(data);
        //            Array.Reverse(data);                   

        //            double[] health = new double[20];
        //            int[] AvgEC = new int[20];
        //            Parallel.For(0, 20, k =>
        //            {
        //                health[k] = data[k] / 2450.0 * 100.0;
        //                AvgEC[k] = 3000 - (int)Math.Floor(30 * health[k]);//1% = 30
        //            });

        //            HealthList.Add(health);
        //            AvgECList.Add(AvgEC);

        //            //Make StorageAnalyzerData
        //            TestingStorageAnalyzerData[storageAnalyzerIndex] = new BsonDocument()
        //            {
        //                {"SN","BCADevice"+(storageAnalyzerIndex+++1).ToString("D5")},
        //                {"Capacity",238.467908},
        //                {"InitHealth",100},
        //                {"InitTime",1505260800},
        //                {"PECycle",3000},
        //                {"Lifespan",new BsonArray()
        //                            {
        //                                new BsonDocument()
        //                                {//1
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//2
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//3
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//4
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//5
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//6
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//7
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//8
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//9
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//10
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//11
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//12
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//13
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//14
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//15
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//16
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//17
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//18
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//19
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                },
        //                                new BsonDocument()
        //                                {//20
        //                                        {"time",lifespantime[index]},
        //                                        {"health",health[index]},
        //                                        {"data",data[index++]}
        //                                }
        //                            }
        //                }
        //            };
        //            //Catch Event Lifespan
        //            if(data[19]< lifespanThreshold)
        //            {
        //                elList.Add(new EventLife()
        //                {
        //                    deviceName = "Device" + (deviceSN+1).ToString("D5"),
        //                    storageSN = (i-1).ToString(),
        //                    Lifespan = data[19].ToString()
        //                });
        //            }
        //        }

        //    //Dynamic Data

        //    //Setting
        //    int MaxMemorySize = 16777216;
        //    int StorageIndex = 0;
        //    int TempData1 = 0, TempData2 = 0;           
        //    int[] timestamp = new int[DynamicDataCount];

        //    timestamp[0] = (int)(UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds-1200;

        //    for ( i = 1; i < DynamicDataCount; i++)
        //        timestamp[i] = timestamp[i - 1] + 60;

        //    List<EventTemp> etList = new List<EventTemp>();

        //    //Make
        //    i = 1;
        //    storageSN = 1;
        //    switch (DeviceStorageCount[i-1])
        //    {
        //        case 1:TestingDynamicData
        //            for (int j = 0; j < 20; j++)
        //            {
        //                TempData1 = rnd.Next(51, 80);
        //                TestingDynamicData[i - 1][j] = new BsonDocument()
        //                        {
        //                            {"Dev", "Device" + i.ToString("D5")},
        //                            {"CPU",new BsonDocument()
        //                                    {
        //                                        {"0",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"1",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"2",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"3",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"Freq", rnd.Next(1500, 3500)},
        //                                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                        {"FanRPM", rnd.Next(1500, 2500)},
        //                                    }
        //                            },
        //                            {"mbProbe",new BsonDocument()},
        //                            {"MEM", GetDynamicMemRawData()},
        //                            {"Storage",new BsonArray()
        //                                        {
        //                                            new BsonDocument()
        //                                            {//1
        //                                                {"Index",0},
        //                                                {"SN", "BCADevice"+(storageSN).ToString("D5")},
        //                                                {"smart",new BsonDocument()
        //                                                        {
        //                                                            {"5",0},
        //                                                            {"9",4274},
        //                                                            {"12",31},
        //                                                            {"163",14},
        //                                                            {"165",334},
        //                                                            {"167",AvgECList[StorageIndex][j]},
        //                                                            {"169",91},
        //                                                            {"170",142},
        //                                                            {"171",0},
        //                                                            {"172",0},
        //                                                            {"192",0},
        //                                                            {"194",TempData1},
        //                                                            {"229",0},
        //                                                            {"235",0 },
        //                                                            {"241",85336},
        //                                                            {"242",106513}
        //                                                        }
        //                                                },
        //                                                {"Health",HealthList[StorageIndex][j]},
        //                                                {"PECycle",3000},
        //                                                {"iAnalyzer",new BsonDocument()
        //                                                            {
        //                                                                {"Enable", 1},
        //                                                                {"SRC", 2784002},
        //                                                                {"RRC", 39870315},
        //                                                                {"SWC", 1527066},
        //                                                                {"RWC", 66674392},
        //                                                                {"SR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 17629},
        //                                                                            {"1", 60490},
        //                                                                            {"2", 732917},
        //                                                                            {"3", 153909},
        //                                                                            {"4", 727414},
        //                                                                            {"5", 1091643},
        //                                                                        }
        //                                                                },
        //                                                                {"SW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 10397},
        //                                                                            {"1", 52358},
        //                                                                            {"2", 1230507},
        //                                                                            {"3", 41972},
        //                                                                            {"4", 54111},
        //                                                                            {"5", 137721},
        //                                                                        }
        //                                                                },
        //                                                                {"RR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0",9466989},
        //                                                                            {"1",7688031},
        //                                                                            {"2",9023755},
        //                                                                            {"3",1406997},
        //                                                                            {"4",12284543}
        //                                                                        }
        //                                                                },
        //                                                                {"RW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 4381752},
        //                                                                            {"1", 2522908},
        //                                                                            {"2", 13497783},
        //                                                                            {"3", 5396054},
        //                                                                            {"4", 40875895}
        //                                                                        }
        //                                                                },
        //                                                            }
        //                                                }
        //                                            }
        //                                        }
        //                            },
        //                            {"Ext",new BsonDocument()
        //                                    {
        //                                        {"0", 4136 },
        //                                        {"1", 122 },
        //                                        {"2", 87 },
        //                                        {"3", 345 },
        //                                        {"4", 276 },
        //                                        {"5", 45 }
        //                                    }
        //                            },
        //                            {"time", timestamp[j]}
        //                        };
        //                if (TempData1 > 50)
        //                {
        //                    etList.Add(new EventTemp()
        //                    {
        //                        deviceName = "Device" + i.ToString("D5"),
        //                        storageSN = "0",
        //                        Temp = TempData1.ToString(),
        //                    });
        //                }
        //            }
        //            StorageIndex += 1;
        //            storageSN++;
        //            break;
        //        case 2:
        //            for (int j = 0; j < 20; j++)
        //            {
        //                TempData1 = rnd.Next(51, 80);
        //                TempData2 = rnd.Next(51, 80);
        //                TestingDynamicData[i - 1][j] = new BsonDocument()
        //                        {
        //                            {"Dev", "Device" + i.ToString("D5")},
        //                            {"CPU",new BsonDocument()
        //                                    {
        //                                        {"0",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"1",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"2",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"3",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"Freq", rnd.Next(1500, 3500)},
        //                                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                        {"FanRPM", rnd.Next(1500, 2500)},
        //                                    }
        //                            },
        //                            {"mbProbe",new BsonDocument()},
        //                            {"MEM", GetDynamicMemRawData()},
        //                            {"Storage",new BsonArray()
        //                                        {
        //                                            new BsonDocument()
        //                                            {//1
        //                                                {"Index",0},
        //                                                {"SN", "BCADevice"+(storageSN).ToString("D5")},
        //                                                {"smart",new BsonDocument()
        //                                                        {
        //                                                            {"5",0},
        //                                                            {"9",4274},
        //                                                            {"12",31},
        //                                                            {"163",14},
        //                                                            {"165",334},
        //                                                            {"167",AvgECList[StorageIndex][j]},
        //                                                            {"169",91},
        //                                                            {"170",142},
        //                                                            {"171",0},
        //                                                            {"172",0},
        //                                                            {"192",0},
        //                                                            {"194",TempData1},
        //                                                            {"229",0},
        //                                                            {"235",0 },
        //                                                            {"241",85336},
        //                                                            {"242",106513}
        //                                                        }
        //                                                },
        //                                                {"Health",HealthList[StorageIndex][j]},
        //                                                {"PECycle",3000},
        //                                                {"iAnalyzer",new BsonDocument()
        //                                                            {
        //                                                                {"Enable", 1},
        //                                                                {"SRC", 2784002},
        //                                                                {"RRC", 39870315},
        //                                                                {"SWC", 1527066},
        //                                                                {"RWC", 66674392},
        //                                                                {"SR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 17629},
        //                                                                            {"1", 60490},
        //                                                                            {"2", 732917},
        //                                                                            {"3", 153909},
        //                                                                            {"4", 727414},
        //                                                                            {"5", 1091643},
        //                                                                        }
        //                                                                },
        //                                                                {"SW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 10397},
        //                                                                            {"1", 52358},
        //                                                                            {"2", 1230507},
        //                                                                            {"3", 41972},
        //                                                                            {"4", 54111},
        //                                                                            {"5", 137721},
        //                                                                        }
        //                                                                },
        //                                                                {"RR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0",9466989},
        //                                                                            {"1",7688031},
        //                                                                            {"2",9023755},
        //                                                                            {"3",1406997},
        //                                                                            {"4",12284543}
        //                                                                        }
        //                                                                },
        //                                                                {"RW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 4381752},
        //                                                                            {"1", 2522908},
        //                                                                            {"2", 13497783},
        //                                                                            {"3", 5396054},
        //                                                                            {"4", 40875895}
        //                                                                        }
        //                                                                },
        //                                                            }
        //                                                }
        //                                            },
        //                                            new BsonDocument()
        //                                            {//2
        //                                                {"Index",1},
        //                                                {"SN", "BCADevice"+(storageSN+1).ToString("D5")},
        //                                                {"smart",new BsonDocument()
        //                                                        {
        //                                                            {"5",0},
        //                                                            {"9",4274},
        //                                                            {"12",31},
        //                                                            {"163",14},
        //                                                            {"165",334},
        //                                                            {"167",AvgECList[StorageIndex+1][j]},
        //                                                            {"169",91},
        //                                                            {"170",142},
        //                                                            {"171",0},
        //                                                            {"172",0},
        //                                                            {"192",0},
        //                                                            {"194",TempData2},
        //                                                            {"229",0},
        //                                                            {"235",0 },
        //                                                            {"241",85336},
        //                                                            {"242",106513}
        //                                                        }
        //                                                },
        //                                                {"Health",HealthList[StorageIndex+1][j]},
        //                                                {"PECycle",3000},
        //                                                {"iAnalyzer",new BsonDocument()
        //                                                            {
        //                                                                {"Enable", 1},
        //                                                                {"SRC", 6219462},
        //                                                                {"RRC", 59308250},
        //                                                                {"SWC", 2492221},
        //                                                                {"RWC", 95052336},
        //                                                                {"SR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 58894},
        //                                                                            {"1", 103215},
        //                                                                            {"2", 1290914},
        //                                                                            {"3", 326046},
        //                                                                            {"4", 1399984},
        //                                                                            {"5", 3040409},
        //                                                                        }
        //                                                                },
        //                                                                {"SW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 40656},
        //                                                                            {"1", 124687},
        //                                                                            {"2", 2038509},
        //                                                                            {"3", 49115},
        //                                                                            {"4", 69337},
        //                                                                            {"5", 169917},
        //                                                                        }
        //                                                                },
        //                                                                {"RR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0",10632528},
        //                                                                            {"1",13444033},
        //                                                                            {"2",13309369},
        //                                                                            {"3",1983239},
        //                                                                            {"4",19939081}
        //                                                                        }
        //                                                                },
        //                                                                {"RW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 6450957},
        //                                                                            {"1", 3623980},
        //                                                                            {"2", 18445399},
        //                                                                            {"3", 7694365},
        //                                                                            {"4", 58837635}
        //                                                                        }
        //                                                                },
        //                                                            }
        //                                                }
        //                                            }
        //                                        }
        //                            },
        //                            {"Ext",new BsonDocument()
        //                                    {
        //                                        {"0", 4136 },
        //                                        {"1", 122 },
        //                                        {"2", 87 },
        //                                        {"3", 345 },
        //                                        {"4", 276 },
        //                                        {"5", 45 }
        //                                    }
        //                            },
        //                            {"time", timestamp[j]}
        //                        };
        //                if (TempData1 > 50)
        //                {
        //                    etList.Add(new EventTemp()
        //                    {
        //                        deviceName = "Device" + i.ToString("D5"),
        //                        storageSN = "0",
        //                        Temp = TempData1.ToString(),
        //                    });
        //                }
        //                if (TempData2 > 50)
        //                {
        //                    etList.Add(new EventTemp()
        //                    {
        //                        deviceName = "Device" + i.ToString("D5"),
        //                        storageSN = "1",
        //                        Temp = TempData2.ToString(),
        //                    });
        //                }
        //            }
        //            StorageIndex += 2;
        //            storageSN += 2;
        //            break;
        //    }

        //    var coreStatus = new string[2] { "on", "off"};

        //    for ( i = 2; i <= Total; i++)//i = device
        //    {
        //        var statusIndex = rnd.Next(0, 2);

        //        if (i % (Total / 2) == 0)
        //        {
        //            if (i <= 11)
        //            {
        //                switch (DeviceStorageCount[i - 1])
        //                {
        //                    case 1:
        //                        for (int j = 0; j < 20; j++)
        //                        {
        //                            TempData1 = rnd.Next(51, 80);
        //                            TestingDynamicData[i - 1][j] = new BsonDocument()
        //                            {
        //                            {"Dev", "Device" + i.ToString("D5")},
        //                            {"CPU",new BsonDocument()
        //                                    {
        //                                        {"0",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)},
        //                                                {"status", coreStatus[statusIndex]},
        //                                                {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                            }
        //                                        },
        //                                        {"1",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)},
        //                                                {"status", coreStatus[statusIndex]},
        //                                                {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                            }
        //                                        },
        //                                        {"2",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)},
        //                                                {"status", coreStatus[statusIndex]},
        //                                                {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                            }
        //                                        },
        //                                        {"3",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)},
        //                                                {"status", coreStatus[statusIndex]},
        //                                                {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                            }
        //                                        },
        //                                        {"Freq", rnd.Next(1500, 3500)},
        //                                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                        {"FanRPM", rnd.Next(1500, 2500)},
        //                                    }
        //                            },
        //                            {"mbProbe",new BsonDocument()},
        //                            {"MEM", GetDynamicMemRawData()},
        //                            {"Storage",new BsonArray()
        //                                        {
        //                                            new BsonDocument()
        //                                            {//1
        //                                                {"Index",0},
        //                                                {"SN", "BCADevice"+(storageSN).ToString("D5")},
        //                                                {"smart",new BsonDocument()
        //                                                        {
        //                                                            {"5",0},
        //                                                            {"9",4274},
        //                                                            {"12",31},
        //                                                            {"163",14},
        //                                                            {"165",334},
        //                                                            {"167",AvgECList[StorageIndex][j]},
        //                                                            {"169",91},
        //                                                            {"170",142},
        //                                                            {"171",0},
        //                                                            {"172",0},
        //                                                            {"192",0},
        //                                                            {"194",TempData1},
        //                                                            {"229",0},
        //                                                            {"235",0 },
        //                                                            {"241",85336},
        //                                                            {"242",106513}
        //                                                        }
        //                                                },
        //                                                {"Health",HealthList[StorageIndex][j]},
        //                                                {"PECycle",3000},
        //                                                {"iAnalyzer",new BsonDocument()
        //                                                            {
        //                                                                {"Enable", 1},
        //                                                                {"SRC", 2784002},
        //                                                                {"RRC", 39870315},
        //                                                                {"SWC", 1527066},
        //                                                                {"RWC", 66674392},
        //                                                                {"SR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 17629},
        //                                                                            {"1", 60490},
        //                                                                            {"2", 732917},
        //                                                                            {"3", 153909},
        //                                                                            {"4", 727414},
        //                                                                            {"5", 1091643},
        //                                                                        }
        //                                                                },
        //                                                                {"SW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 10397},
        //                                                                            {"1", 52358},
        //                                                                            {"2", 1230507},
        //                                                                            {"3", 41972},
        //                                                                            {"4", 54111},
        //                                                                            {"5", 137721},
        //                                                                        }
        //                                                                },
        //                                                                {"RR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0",9466989},
        //                                                                            {"1",7688031},
        //                                                                            {"2",9023755},
        //                                                                            {"3",1406997},
        //                                                                            {"4",12284543}
        //                                                                        }
        //                                                                },
        //                                                                {"RW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 4381752},
        //                                                                            {"1", 2522908},
        //                                                                            {"2", 13497783},
        //                                                                            {"3", 5396054},
        //                                                                            {"4", 40875895}
        //                                                                        }
        //                                                                },
        //                                                            }
        //                                                }
        //                                            }
        //                                        }
        //                            },
        //                            {"Ext",new BsonDocument()},
        //                            {"time", timestamp[j]},
        //                            {"GPU",new BsonDocument()
        //                                {
        //                                    {"CoreClock", rnd.Next(140, 1122)},
        //                                    {"Temp", Convert.ToDouble(rnd.Next(0, 99))},
        //                                    {"MemUsed", rnd.Next(28, 99)},
        //                                    {"Load", rnd.Next(0, 99)},
        //                                    {"FanTemp", Convert.ToDouble(rnd.Next(0, 99))},
        //                                }
        //                            },
        //                        };
        //                            if (TempData1 > 50)
        //                            {
        //                                etList.Add(new EventTemp()
        //                                {
        //                                    deviceName = "Device" + i.ToString("D5"),
        //                                    storageSN = "0",
        //                                    Temp = TempData1.ToString(),
        //                                });
        //                            }
        //                        }
        //                        StorageIndex += 1;
        //                        storageSN++;
        //                        break;
        //                    case 2:
        //                        for (int j = 0; j < 20; j++)
        //                        {
        //                            TempData1 = rnd.Next(51, 80);
        //                            TempData2 = rnd.Next(51, 80);
        //                            TestingDynamicData[i - 1][j] = new BsonDocument()
        //                        {
        //                            {"Dev", "Device" + i.ToString("D5")},
        //                            {"CPU",new BsonDocument()
        //                                    {
        //                                        {"0",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)},
        //                                                {"status", coreStatus[statusIndex]},
        //                                                {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                            }
        //                                        },
        //                                        {"1",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)},
        //                                                {"status", coreStatus[statusIndex]},
        //                                                {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                            }
        //                                        },
        //                                        {"2",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)},
        //                                                {"status", coreStatus[statusIndex]},
        //                                                {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                            }
        //                                        },
        //                                        {"3",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)},
        //                                                {"status", coreStatus[statusIndex]},
        //                                                {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                            }
        //                                        },
        //                                        {"Freq", rnd.Next(1500, 3500)},
        //                                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                        {"FanRPM", rnd.Next(1500, 2500)},
        //                                    }
        //                            },
        //                            {"mbProbe",new BsonDocument()},
        //                            {"MEM", GetDynamicMemRawData()},
        //                            {"Storage",new BsonArray()
        //                                        {
        //                                            new BsonDocument()
        //                                            {//1
        //                                                {"Index",0},
        //                                                {"SN", "BCADevice"+(storageSN).ToString("D5")},
        //                                                {"smart",new BsonDocument()
        //                                                        {
        //                                                            {"5",0},
        //                                                            {"9",4274},
        //                                                            {"12",31},
        //                                                            {"163",14},
        //                                                            {"165",334},
        //                                                            {"167",AvgECList[StorageIndex][j]},
        //                                                            {"169",91},
        //                                                            {"170",142},
        //                                                            {"171",0},
        //                                                            {"172",0},
        //                                                            {"192",0},
        //                                                            {"194",TempData1},
        //                                                            {"229",0},
        //                                                            {"235",0 },
        //                                                            {"241",85336},
        //                                                            {"242",106513}
        //                                                        }
        //                                                },
        //                                                {"Health",HealthList[StorageIndex][j]},
        //                                                {"PECycle",3000},
        //                                                {"iAnalyzer",new BsonDocument()
        //                                                            {
        //                                                                {"Enable", 1},
        //                                                                {"SRC", 2784002},
        //                                                                {"RRC", 39870315},
        //                                                                {"SWC", 1527066},
        //                                                                {"RWC", 66674392},
        //                                                                {"SR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 17629},
        //                                                                            {"1", 60490},
        //                                                                            {"2", 732917},
        //                                                                            {"3", 153909},
        //                                                                            {"4", 727414},
        //                                                                            {"5", 1091643},
        //                                                                        }
        //                                                                },
        //                                                                {"SW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 10397},
        //                                                                            {"1", 52358},
        //                                                                            {"2", 1230507},
        //                                                                            {"3", 41972},
        //                                                                            {"4", 54111},
        //                                                                            {"5", 137721},
        //                                                                        }
        //                                                                },
        //                                                                {"RR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0",9466989},
        //                                                                            {"1",7688031},
        //                                                                            {"2",9023755},
        //                                                                            {"3",1406997},
        //                                                                            {"4",12284543}
        //                                                                        }
        //                                                                },
        //                                                                {"RW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 4381752},
        //                                                                            {"1", 2522908},
        //                                                                            {"2", 13497783},
        //                                                                            {"3", 5396054},
        //                                                                            {"4", 40875895}
        //                                                                        }
        //                                                                },
        //                                                            }
        //                                                }
        //                                            },
        //                                            new BsonDocument()
        //                                            {//2
        //                                                {"Index",1},
        //                                                {"SN", "BCADevice"+(storageSN + 1).ToString("D5")},
        //                                                {"smart",new BsonDocument()
        //                                                        {
        //                                                            {"5",0},
        //                                                            {"9",4274},
        //                                                            {"12",31},
        //                                                            {"163",14},
        //                                                            {"165",334},
        //                                                            {"167",AvgECList[StorageIndex+1][j]},
        //                                                            {"169",91},
        //                                                            {"170",142},
        //                                                            {"171",0},
        //                                                            {"172",0},
        //                                                            {"192",0},
        //                                                            {"194",TempData2},
        //                                                            {"229",0},
        //                                                            {"235",0 },
        //                                                            {"241",85336},
        //                                                            {"242",106513}
        //                                                        }
        //                                                },
        //                                                {"Health",HealthList[StorageIndex+1][j]},
        //                                                {"PECycle",3000},
        //                                                {"iAnalyzer",new BsonDocument()
        //                                                            {
        //                                                                {"Enable", 1},
        //                                                                {"SRC", 6219462},
        //                                                                {"RRC", 59308250},
        //                                                                {"SWC", 2492221},
        //                                                                {"RWC", 95052336},
        //                                                                {"SR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 58894},
        //                                                                            {"1", 103215},
        //                                                                            {"2", 1290914},
        //                                                                            {"3", 326046},
        //                                                                            {"4", 1399984},
        //                                                                            {"5", 3040409},
        //                                                                        }
        //                                                                },
        //                                                                {"SW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 40656},
        //                                                                            {"1", 124687},
        //                                                                            {"2", 2038509},
        //                                                                            {"3", 49115},
        //                                                                            {"4", 69337},
        //                                                                            {"5", 169917},
        //                                                                        }
        //                                                                },
        //                                                                {"RR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0",10632528},
        //                                                                            {"1",13444033},
        //                                                                            {"2",13309369},
        //                                                                            {"3",1983239},
        //                                                                            {"4",19939081}
        //                                                                        }
        //                                                                },
        //                                                                {"RW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 6450957},
        //                                                                            {"1", 3623980},
        //                                                                            {"2", 18445399},
        //                                                                            {"3", 7694365},
        //                                                                            {"4", 58837635}
        //                                                                        }
        //                                                                },
        //                                                            }
        //                                                }
        //                                            }
        //                                        }
        //                            },
        //                            {"Ext",new BsonDocument()},
        //                            {"time", timestamp[j]},
        //                            {"GPU",new BsonDocument()
        //                                {
        //                                    {"CoreClock", rnd.Next(140, 1122)},
        //                                    {"Temp", Convert.ToDouble(rnd.Next(0, 99))},
        //                                    {"MemUsed", rnd.Next(28, 99)},
        //                                    {"Load", rnd.Next(0, 99)},
        //                                    {"FanTemp", Convert.ToDouble(rnd.Next(0, 99))},
        //                                }
        //                            },
        //                        };
        //                            if (TempData1 > 50)
        //                            {
        //                                etList.Add(new EventTemp()
        //                                {
        //                                    deviceName = "Device" + i.ToString("D5"),
        //                                    storageSN = "0",
        //                                    Temp = TempData1.ToString(),
        //                                });
        //                            }
        //                            if (TempData2 > 50)
        //                            {
        //                                etList.Add(new EventTemp()
        //                                {
        //                                    deviceName = "Device" + i.ToString("D5"),
        //                                    storageSN = "1",
        //                                    Temp = TempData2.ToString(),
        //                                });
        //                            }
        //                        }
        //                        StorageIndex += 2;
        //                        storageSN += 2;
        //                        break;
        //                }
        //                continue;
        //            }
        //            switch (DeviceStorageCount[i - 1])
        //            {
        //                case 1:
        //                    for (int j = 0; j < 20; j++)
        //                    {
        //                        TempData1 = rnd.Next(51, 80);
        //                        TestingDynamicData[i - 1][j] = new BsonDocument()
        //                        {
        //                            {"Dev", "Device" + i.ToString("D5")},
        //                            {"CPU",new BsonDocument()
        //                                    {
        //                                        {"0",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"1",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"2",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"3",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"Freq", rnd.Next(1500, 3500)},
        //                                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                        {"FanRPM", rnd.Next(1500, 2500)},
        //                                    }
        //                            },
        //                            {"mbProbe",new BsonDocument()},
        //                            {"MEM", GetDynamicMemRawData()},
        //                            {"Storage",new BsonArray()
        //                                        {
        //                                            new BsonDocument()
        //                                            {//1
        //                                                {"Index",0},
        //                                                {"SN", "BCADevice"+(storageSN).ToString("D5")},
        //                                                {"smart",new BsonDocument()
        //                                                        {
        //                                                            {"5",0},
        //                                                            {"9",4274},
        //                                                            {"12",31},
        //                                                            {"163",14},
        //                                                            {"165",334},
        //                                                            {"167",AvgECList[StorageIndex][j]},
        //                                                            {"169",91},
        //                                                            {"170",142},
        //                                                            {"171",0},
        //                                                            {"172",0},
        //                                                            {"192",0},
        //                                                            {"194",TempData1},
        //                                                            {"229",0},
        //                                                            {"235",0 },
        //                                                            {"241",85336},
        //                                                            {"242",106513}
        //                                                        }
        //                                                },
        //                                                {"Health",HealthList[StorageIndex][j]},
        //                                                {"PECycle",3000},
        //                                                {"iAnalyzer",new BsonDocument()
        //                                                            {
        //                                                                {"Enable", 1},
        //                                                                {"SRC", 2784002},
        //                                                                {"RRC", 39870315},
        //                                                                {"SWC", 1527066},
        //                                                                {"RWC", 66674392},
        //                                                                {"SR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 17629},
        //                                                                            {"1", 60490},
        //                                                                            {"2", 732917},
        //                                                                            {"3", 153909},
        //                                                                            {"4", 727414},
        //                                                                            {"5", 1091643},
        //                                                                        }
        //                                                                },
        //                                                                {"SW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 10397},
        //                                                                            {"1", 52358},
        //                                                                            {"2", 1230507},
        //                                                                            {"3", 41972},
        //                                                                            {"4", 54111},
        //                                                                            {"5", 137721},
        //                                                                        }
        //                                                                },
        //                                                                {"RR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0",9466989},
        //                                                                            {"1",7688031},
        //                                                                            {"2",9023755},
        //                                                                            {"3",1406997},
        //                                                                            {"4",12284543}
        //                                                                        }
        //                                                                },
        //                                                                {"RW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 4381752},
        //                                                                            {"1", 2522908},
        //                                                                            {"2", 13497783},
        //                                                                            {"3", 5396054},
        //                                                                            {"4", 40875895}
        //                                                                        }
        //                                                                },
        //                                                            }
        //                                                }
        //                                            }
        //                                        }
        //                            },
        //                            {"Ext",new BsonDocument()},
        //                            {"time", timestamp[j]}
        //                        };
        //                        if (TempData1 > 50)
        //                        {
        //                            etList.Add(new EventTemp()
        //                            {
        //                                deviceName = "Device" + i.ToString("D5"),
        //                                storageSN = "0",
        //                                Temp = TempData1.ToString(),
        //                            });
        //                        }
        //                    }
        //                    StorageIndex += 1;
        //                    storageSN++;
        //                    break;
        //                case 2:
        //                    for (int j = 0; j < 20; j++)
        //                    {
        //                        TempData1 = rnd.Next(51, 80);
        //                        TempData2 = rnd.Next(51, 80);
        //                        TestingDynamicData[i - 1][j] = new BsonDocument()
        //                        {
        //                            {"Dev", "Device" + i.ToString("D5")},
        //                            {"CPU",new BsonDocument()
        //                                    {
        //                                        {"0",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"1",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"2",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"3",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"Freq", rnd.Next(1500, 3500)},
        //                                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                        {"FanRPM", rnd.Next(1500, 2500)},
        //                                    }
        //                            },
        //                            {"mbProbe",new BsonDocument()},
        //                            {"MEM", GetDynamicMemRawData()},
        //                            {"Storage",new BsonArray()
        //                                        {
        //                                            new BsonDocument()
        //                                            {//1
        //                                                {"Index",0},
        //                                                {"SN", "BCADevice"+(storageSN).ToString("D5")},
        //                                                {"smart",new BsonDocument()
        //                                                        {
        //                                                            {"5",0},
        //                                                            {"9",4274},
        //                                                            {"12",31},
        //                                                            {"163",14},
        //                                                            {"165",334},
        //                                                            {"167",AvgECList[StorageIndex][j]},
        //                                                            {"169",91},
        //                                                            {"170",142},
        //                                                            {"171",0},
        //                                                            {"172",0},
        //                                                            {"192",0},
        //                                                            {"194",TempData1},
        //                                                            {"229",0},
        //                                                            {"235",0 },
        //                                                            {"241",85336},
        //                                                            {"242",106513}
        //                                                        }
        //                                                },
        //                                                {"Health",HealthList[StorageIndex][j]},
        //                                                {"PECycle",3000},
        //                                                {"iAnalyzer",new BsonDocument()
        //                                                            {
        //                                                                {"Enable", 1},
        //                                                                {"SRC", 2784002},
        //                                                                {"RRC", 39870315},
        //                                                                {"SWC", 1527066},
        //                                                                {"RWC", 66674392},
        //                                                                {"SR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 17629},
        //                                                                            {"1", 60490},
        //                                                                            {"2", 732917},
        //                                                                            {"3", 153909},
        //                                                                            {"4", 727414},
        //                                                                            {"5", 1091643},
        //                                                                        }
        //                                                                },
        //                                                                {"SW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 10397},
        //                                                                            {"1", 52358},
        //                                                                            {"2", 1230507},
        //                                                                            {"3", 41972},
        //                                                                            {"4", 54111},
        //                                                                            {"5", 137721},
        //                                                                        }
        //                                                                },
        //                                                                {"RR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0",9466989},
        //                                                                            {"1",7688031},
        //                                                                            {"2",9023755},
        //                                                                            {"3",1406997},
        //                                                                            {"4",12284543}
        //                                                                        }
        //                                                                },
        //                                                                {"RW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 4381752},
        //                                                                            {"1", 2522908},
        //                                                                            {"2", 13497783},
        //                                                                            {"3", 5396054},
        //                                                                            {"4", 40875895}
        //                                                                        }
        //                                                                },
        //                                                            }
        //                                                }
        //                                            },
        //                                            new BsonDocument()
        //                                            {//2
        //                                                {"Index",1},
        //                                                {"SN", "BCADevice"+(storageSN + 1).ToString("D5")},
        //                                                {"smart",new BsonDocument()
        //                                                        {
        //                                                            {"5",0},
        //                                                            {"9",4274},
        //                                                            {"12",31},
        //                                                            {"163",14},
        //                                                            {"165",334},
        //                                                            {"167",AvgECList[StorageIndex+1][j]},
        //                                                            {"169",91},
        //                                                            {"170",142},
        //                                                            {"171",0},
        //                                                            {"172",0},
        //                                                            {"192",0},
        //                                                            {"194",TempData2},
        //                                                            {"229",0},
        //                                                            {"235",0 },
        //                                                            {"241",85336},
        //                                                            {"242",106513}
        //                                                        }
        //                                                },
        //                                                {"Health",HealthList[StorageIndex+1][j]},
        //                                                {"PECycle",3000},
        //                                                {"iAnalyzer",new BsonDocument()
        //                                                            {
        //                                                                {"Enable", 1},
        //                                                                {"SRC", 6219462},
        //                                                                {"RRC", 59308250},
        //                                                                {"SWC", 2492221},
        //                                                                {"RWC", 95052336},
        //                                                                {"SR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 58894},
        //                                                                            {"1", 103215},
        //                                                                            {"2", 1290914},
        //                                                                            {"3", 326046},
        //                                                                            {"4", 1399984},
        //                                                                            {"5", 3040409},
        //                                                                        }
        //                                                                },
        //                                                                {"SW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 40656},
        //                                                                            {"1", 124687},
        //                                                                            {"2", 2038509},
        //                                                                            {"3", 49115},
        //                                                                            {"4", 69337},
        //                                                                            {"5", 169917},
        //                                                                        }
        //                                                                },
        //                                                                {"RR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0",10632528},
        //                                                                            {"1",13444033},
        //                                                                            {"2",13309369},
        //                                                                            {"3",1983239},
        //                                                                            {"4",19939081}
        //                                                                        }
        //                                                                },
        //                                                                {"RW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 6450957},
        //                                                                            {"1", 3623980},
        //                                                                            {"2", 18445399},
        //                                                                            {"3", 7694365},
        //                                                                            {"4", 58837635}
        //                                                                        }
        //                                                                },
        //                                                            }
        //                                                }
        //                                            }
        //                                        }
        //                            },
        //                            {"Ext",new BsonDocument()},
        //                            {"time", timestamp[j]}
        //                        };
        //                        if (TempData1 > 50)
        //                        {
        //                            etList.Add(new EventTemp()
        //                            {
        //                                deviceName = "Device" + i.ToString("D5"),
        //                                storageSN = "0",
        //                                Temp = TempData1.ToString(),
        //                            });
        //                        }
        //                        if (TempData2 > 50)
        //                        {
        //                            etList.Add(new EventTemp()
        //                            {
        //                                deviceName = "Device" + i.ToString("D5"),
        //                                storageSN = "1",
        //                                Temp = TempData2.ToString(),
        //                            });
        //                        }
        //                    }
        //                    StorageIndex += 2;
        //                    storageSN += 2;
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            if (i <= 11)
        //            {
        //                switch (DeviceStorageCount[i - 1])
        //                {
        //                    case 1:
        //                        for (int j = 0; j < 20; j++)
        //                        {
        //                            TempData1 = rnd.Next(25, 50);
        //                            TestingDynamicData[i - 1][j] = new BsonDocument()
        //                            {
        //                                {"Dev", "Device" + i.ToString("D5")},
        //                                {"CPU",new BsonDocument()
        //                                        {
        //                                            {"0",new BsonDocument()
        //                                                {
        //                                                    {"Freq", rnd.Next(1000, 1500)},
        //                                                    {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                    {"Temp", rnd.Next(20, 45)},
        //                                                    {"V", rnd.Next(8, 11)},
        //                                                    {"status", coreStatus[statusIndex]},
        //                                                    {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                    {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                                }
        //                                            },
        //                                            {"1",new BsonDocument()
        //                                                {
        //                                                    {"Freq", rnd.Next(1000, 1500)},
        //                                                    {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                    {"Temp", rnd.Next(20, 45)},
        //                                                    {"V", rnd.Next(8, 11)},
        //                                                    {"status", coreStatus[statusIndex]},
        //                                                    {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                    {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                                }
        //                                            },
        //                                            {"2",new BsonDocument()
        //                                                {
        //                                                    {"Freq", rnd.Next(1000, 1500)},
        //                                                    {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                    {"Temp", rnd.Next(20, 45)},
        //                                                    {"V", rnd.Next(8, 11)},
        //                                                    {"status", coreStatus[statusIndex]},
        //                                                    {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                    {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                                }
        //                                            },
        //                                            {"3",new BsonDocument()
        //                                                {
        //                                                    {"Freq", rnd.Next(1000, 1500)},
        //                                                    {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                    {"Temp", rnd.Next(20, 45)},
        //                                                    {"V", rnd.Next(8, 11)},
        //                                                    {"status", coreStatus[statusIndex]},
        //                                                    {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                    {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                                }
        //                                            },
        //                                            {"Freq", rnd.Next(1500, 3500)},
        //                                            {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                            {"FanRPM", rnd.Next(1500, 2500)},
        //                                        }
        //                                },
        //                                {"mbProbe",new BsonDocument()},
        //                                {"MEM", GetDynamicMemRawData()},
        //                                {"Storage",new BsonArray()
        //                                            {
        //                                                new BsonDocument()
        //                                                {//1
        //                                                    {"Index",0},
        //                                                    {"SN", "BCADevice"+(storageSN).ToString("D5")},
        //                                                    {"smart",new BsonDocument()
        //                                                            {
        //                                                                {"5",0},
        //                                                                {"9",4274},
        //                                                                {"12",31},
        //                                                                {"163",14},
        //                                                                {"165",334},
        //                                                                {"167",AvgECList[StorageIndex][j]},
        //                                                                {"169",91},
        //                                                                {"170",142},
        //                                                                {"171",0},
        //                                                                {"172",0},
        //                                                                {"192",0},
        //                                                                {"194",TempData1},
        //                                                                {"229",0},
        //                                                                {"235",0 },
        //                                                                {"241",85336},
        //                                                                {"242",106513}
        //                                                            }
        //                                                    },
        //                                                    {"Health",HealthList[StorageIndex][j]},
        //                                                    {"PECycle",3000},
        //                                                    {"iAnalyzer",new BsonDocument()
        //                                                                {
        //                                                                    {"Enable", 1},
        //                                                                    {"SRC", 2784002},
        //                                                                    {"RRC", 39870315},
        //                                                                    {"SWC", 1527066},
        //                                                                    {"RWC", 66674392},
        //                                                                    {"SR",new BsonDocument()
        //                                                                            {
        //                                                                                {"0", 17629},
        //                                                                                {"1", 60490},
        //                                                                                {"2", 732917},
        //                                                                                {"3", 153909},
        //                                                                                {"4", 727414},
        //                                                                                {"5", 1091643},
        //                                                                            }
        //                                                                    },
        //                                                                    {"SW",new BsonDocument()
        //                                                                            {
        //                                                                                {"0", 10397},
        //                                                                                {"1", 52358},
        //                                                                                {"2", 1230507},
        //                                                                                {"3", 41972},
        //                                                                                {"4", 54111},
        //                                                                                {"5", 137721},
        //                                                                            }
        //                                                                    },
        //                                                                    {"RR",new BsonDocument()
        //                                                                            {
        //                                                                                {"0",9466989},
        //                                                                                {"1",7688031},
        //                                                                                {"2",9023755},
        //                                                                                {"3",1406997},
        //                                                                                {"4",12284543}
        //                                                                            }
        //                                                                    },
        //                                                                    {"RW",new BsonDocument()
        //                                                                            {
        //                                                                                {"0", 4381752},
        //                                                                                {"1", 2522908},
        //                                                                                {"2", 13497783},
        //                                                                                {"3", 5396054},
        //                                                                                {"4", 40875895}
        //                                                                            }
        //                                                                    },
        //                                                                }
        //                                                    }
        //                                                }
        //                                            }
        //                                },
        //                                {"Ext",new BsonDocument()},
        //                                {"time", timestamp[j]},
        //                                {"GPU",new BsonDocument()
        //                                    {
        //                                        {"CoreClock", rnd.Next(140, 1122)},
        //                                        {"Temp", Convert.ToDouble(rnd.Next(0, 99))},
        //                                        {"MemUsed", rnd.Next(28, 99)},
        //                                        {"Load", rnd.Next(0, 99)},
        //                                        {"FanTemp", Convert.ToDouble(rnd.Next(0, 99))},
        //                                    }
        //                                },
        //                            };
        //                        }
        //                        StorageIndex += 1;
        //                        storageSN++;
        //                        break;
        //                    case 2:
        //                        for (int j = 0; j < 20; j++)
        //                        {
        //                            TempData1 = rnd.Next(25, 50);
        //                            TempData2 = rnd.Next(25, 50);
        //                            TestingDynamicData[i - 1][j] = new BsonDocument()
        //                            {
        //                                {"Dev", "Device" + i.ToString("D5")},
        //                                {"CPU",new BsonDocument()
        //                                        {
        //                                            {"0",new BsonDocument()
        //                                                {
        //                                                    {"Freq", rnd.Next(1000, 1500)},
        //                                                    {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                    {"Temp", rnd.Next(20, 45)},
        //                                                    {"V", rnd.Next(8, 11)},
        //                                                    {"status", coreStatus[statusIndex]},
        //                                                    {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                    {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                                }
        //                                            },
        //                                            {"1",new BsonDocument()
        //                                                {
        //                                                    {"Freq", rnd.Next(1000, 1500)},
        //                                                    {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                    {"Temp", rnd.Next(20, 45)},
        //                                                    {"V", rnd.Next(8, 11)},
        //                                                    {"status", coreStatus[statusIndex]},
        //                                                    {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                    {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                                }
        //                                            },
        //                                            {"2",new BsonDocument()
        //                                                {
        //                                                    {"Freq", rnd.Next(1000, 1500)},
        //                                                    {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                    {"Temp", rnd.Next(20, 45)},
        //                                                    {"V", rnd.Next(8, 11)},
        //                                                    {"status", coreStatus[statusIndex]},
        //                                                    {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                    {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                                }
        //                                            },
        //                                            {"3",new BsonDocument()
        //                                                {
        //                                                    {"Freq", rnd.Next(1000, 1500)},
        //                                                    {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                    {"Temp", rnd.Next(20, 45)},
        //                                                    {"V", rnd.Next(8, 11)},
        //                                                    {"status", coreStatus[statusIndex]},
        //                                                    {"frequency", (coreStatus[statusIndex] == "on") ? rnd.Next(345, 2301) : 0},
        //                                                    {"loading", (coreStatus[statusIndex] == "on") ? rnd.Next(0, 100) : 0 }
        //                                                }
        //                                            },
        //                                            {"Freq", rnd.Next(1500, 3500)},
        //                                            {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                            {"FanRPM", rnd.Next(1500, 2500)},
        //                                        }
        //                                },
        //                                {"mbProbe",new BsonDocument()},
        //                                {"MEM", GetDynamicMemRawData()},    
        //                                {"Storage",new BsonArray()
        //                                            {
        //                                                new BsonDocument()
        //                                                {//1
        //                                                    {"Index",0},
        //                                                    {"SN", "BCADevice"+(storageSN).ToString("D5")},
        //                                                    {"smart",new BsonDocument()
        //                                                            {
        //                                                                {"5",0},
        //                                                                {"9",4274},
        //                                                                {"12",31},
        //                                                                {"163",14},
        //                                                                {"165",334},
        //                                                                {"167",AvgECList[StorageIndex][j]},
        //                                                                {"169",91},
        //                                                                {"170",142},
        //                                                                {"171",0},
        //                                                                {"172",0},
        //                                                                {"192",0},
        //                                                                {"194",TempData1},
        //                                                                {"229",0},
        //                                                                {"235",0 },
        //                                                                {"241",85336},
        //                                                                {"242",106513}
        //                                                            }
        //                                                    },
        //                                                    {"Health",HealthList[StorageIndex][j]},
        //                                                    {"PECycle",3000},
        //                                                    {"iAnalyzer",new BsonDocument()
        //                                                                {
        //                                                                    {"Enable", 1},
        //                                                                    {"SRC", 2784002},
        //                                                                    {"RRC", 39870315},
        //                                                                    {"SWC", 1527066},
        //                                                                    {"RWC", 66674392},
        //                                                                    {"SR",new BsonDocument()
        //                                                                            {
        //                                                                                {"0", 17629},
        //                                                                                {"1", 60490},
        //                                                                                {"2", 732917},
        //                                                                                {"3", 153909},
        //                                                                                {"4", 727414},
        //                                                                                {"5", 1091643},
        //                                                                            }
        //                                                                    },
        //                                                                    {"SW",new BsonDocument()
        //                                                                            {
        //                                                                                {"0", 10397},
        //                                                                                {"1", 52358},
        //                                                                                {"2", 1230507},
        //                                                                                {"3", 41972},
        //                                                                                {"4", 54111},
        //                                                                                {"5", 137721},
        //                                                                            }
        //                                                                    },
        //                                                                    {"RR",new BsonDocument()
        //                                                                            {
        //                                                                                {"0",9466989},
        //                                                                                {"1",7688031},
        //                                                                                {"2",9023755},
        //                                                                                {"3",1406997},
        //                                                                                {"4",12284543}
        //                                                                            }
        //                                                                    },
        //                                                                    {"RW",new BsonDocument()
        //                                                                            {
        //                                                                                {"0", 4381752},
        //                                                                                {"1", 2522908},
        //                                                                                {"2", 13497783},
        //                                                                                {"3", 5396054},
        //                                                                                {"4", 40875895}
        //                                                                            }
        //                                                                    },
        //                                                                }
        //                                                    }
        //                                                },
        //                                                new BsonDocument()
        //                                                {//2
        //                                                    {"Index",1},
        //                                                    {"SN", "BCADevice"+(storageSN + 1).ToString("D5")},
        //                                                    {"smart",new BsonDocument()
        //                                                            {
        //                                                                {"5",0},
        //                                                                {"9",4274},
        //                                                                {"12",31},
        //                                                                {"163",14},
        //                                                                {"165",334},
        //                                                                {"167",AvgECList[StorageIndex+1][j]},
        //                                                                {"169",91},
        //                                                                {"170",142},
        //                                                                {"171",0},
        //                                                                {"172",0},
        //                                                                {"192",0},
        //                                                                {"194",TempData2},
        //                                                                {"229",0},
        //                                                                {"235",0 },
        //                                                                {"241",85336},
        //                                                                {"242",106513}
        //                                                            }
        //                                                    },
        //                                                    {"Health",HealthList[StorageIndex+1][j]},
        //                                                    {"PECycle",3000},
        //                                                    {"iAnalyzer",new BsonDocument()
        //                                                                {
        //                                                                    {"Enable", 1},
        //                                                                    {"SRC", 6219462},
        //                                                                    {"RRC", 59308250},
        //                                                                    {"SWC", 2492221},
        //                                                                    {"RWC", 95052336},
        //                                                                    {"SR",new BsonDocument()
        //                                                                            {
        //                                                                                {"0", 58894},
        //                                                                                {"1", 103215},
        //                                                                                {"2", 1290914},
        //                                                                                {"3", 326046},
        //                                                                                {"4", 1399984},
        //                                                                                {"5", 3040409},
        //                                                                            }
        //                                                                    },
        //                                                                    {"SW",new BsonDocument()
        //                                                                            {
        //                                                                                {"0", 40656},
        //                                                                                {"1", 124687},
        //                                                                                {"2", 2038509},
        //                                                                                {"3", 49115},
        //                                                                                {"4", 69337},
        //                                                                                {"5", 169917},
        //                                                                            }
        //                                                                    },
        //                                                                    {"RR",new BsonDocument()
        //                                                                            {
        //                                                                                {"0",10632528},
        //                                                                                {"1",13444033},
        //                                                                                {"2",13309369},
        //                                                                                {"3",1983239},
        //                                                                                {"4",19939081}
        //                                                                            }
        //                                                                    },
        //                                                                    {"RW",new BsonDocument()
        //                                                                            {
        //                                                                                {"0", 6450957},
        //                                                                                {"1", 3623980},
        //                                                                                {"2", 18445399},
        //                                                                                {"3", 7694365},
        //                                                                                {"4", 58837635}
        //                                                                            }
        //                                                                    },
        //                                                                }
        //                                                    }
        //                                                }
        //                                            }
        //                                },
        //                                {"Ext",new BsonDocument()},
        //                                {"time", timestamp[j]},
        //                                {"GPU",new BsonDocument()
        //                                    {
        //                                        {"CoreClock", rnd.Next(140, 1122)},
        //                                        {"Temp", Convert.ToDouble(rnd.Next(0, 99))},
        //                                        {"MemUsed", rnd.Next(28, 99)},
        //                                        {"Load", rnd.Next(0, 99)},
        //                                        {"FanTemp", Convert.ToDouble(rnd.Next(0, 99))},
        //                                    }
        //                                },
        //                            };
        //                        }
        //                        StorageIndex += 2;
        //                        storageSN += 2;
        //                        break;
        //                }
        //                continue;
        //            }

        //            switch (DeviceStorageCount[i - 1])
        //            {
        //                case 1:
        //                    for (int j = 0; j < 20; j++)
        //                    {
        //                        TempData1 = rnd.Next(25, 50);
        //                        TestingDynamicData[i - 1][j] = new BsonDocument()
        //                        {
        //                            {"Dev", "Device" + i.ToString("D5")},
        //                            {"CPU",new BsonDocument()
        //                                    {
        //                                        {"0",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"1",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"2",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"3",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"Freq", rnd.Next(1500, 3500)},
        //                                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                        {"FanRPM", rnd.Next(1500, 2500)},
        //                                    }
        //                            },
        //                            {"mbProbe",new BsonDocument()},
        //                            {"MEM", GetDynamicMemRawData()},
        //                            {"Storage",new BsonArray()
        //                                        {
        //                                            new BsonDocument()
        //                                            {//1
        //                                                {"Index",0},
        //                                                {"SN", "BCADevice"+(storageSN).ToString("D5")},
        //                                                {"smart",new BsonDocument()
        //                                                        {
        //                                                            {"5",0},
        //                                                            {"9",4274},
        //                                                            {"12",31},
        //                                                            {"163",14},
        //                                                            {"165",334},
        //                                                            {"167",AvgECList[StorageIndex][j]},
        //                                                            {"169",91},
        //                                                            {"170",142},
        //                                                            {"171",0},
        //                                                            {"172",0},
        //                                                            {"192",0},
        //                                                            {"194",TempData1},
        //                                                            {"229",0},
        //                                                            {"235",0 },
        //                                                            {"241",85336},
        //                                                            {"242",106513}
        //                                                        }
        //                                                },
        //                                                {"Health",HealthList[StorageIndex][j]},
        //                                                {"PECycle",3000},
        //                                                {"iAnalyzer",new BsonDocument()
        //                                                            {
        //                                                                {"Enable", 1},
        //                                                                {"SRC", 2784002},
        //                                                                {"RRC", 39870315},
        //                                                                {"SWC", 1527066},
        //                                                                {"RWC", 66674392},
        //                                                                {"SR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 17629},
        //                                                                            {"1", 60490},
        //                                                                            {"2", 732917},
        //                                                                            {"3", 153909},
        //                                                                            {"4", 727414},
        //                                                                            {"5", 1091643},
        //                                                                        }
        //                                                                },
        //                                                                {"SW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 10397},
        //                                                                            {"1", 52358},
        //                                                                            {"2", 1230507},
        //                                                                            {"3", 41972},
        //                                                                            {"4", 54111},
        //                                                                            {"5", 137721},
        //                                                                        }
        //                                                                },
        //                                                                {"RR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0",9466989},
        //                                                                            {"1",7688031},
        //                                                                            {"2",9023755},
        //                                                                            {"3",1406997},
        //                                                                            {"4",12284543}
        //                                                                        }
        //                                                                },
        //                                                                {"RW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 4381752},
        //                                                                            {"1", 2522908},
        //                                                                            {"2", 13497783},
        //                                                                            {"3", 5396054},
        //                                                                            {"4", 40875895}
        //                                                                        }
        //                                                                },
        //                                                            }
        //                                                }
        //                                            }
        //                                        }
        //                            },
        //                            {"Ext",new BsonDocument()},
        //                            {"time", timestamp[j]}
        //                        };
        //                    }
        //                    StorageIndex += 1;
        //                    storageSN++;
        //                    break;
        //                case 2:
        //                    for (int j = 0; j < 20; j++)
        //                    {
        //                        TempData1 = rnd.Next(25, 50);
        //                        TempData2 = rnd.Next(25, 50);
        //                        TestingDynamicData[i - 1][j] = new BsonDocument()
        //                        {
        //                            {"Dev", "Device" + i.ToString("D5")},
        //                            {"CPU",new BsonDocument()
        //                                    {
        //                                        {"0",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"1",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"2",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"3",new BsonDocument()
        //                                            {
        //                                                {"Freq", rnd.Next(1000, 1500)},
        //                                                {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                                {"Temp", rnd.Next(20, 45)},
        //                                                {"V", rnd.Next(8, 11)}
        //                                            }
        //                                        },
        //                                        {"Freq", rnd.Next(1500, 3500)},
        //                                        {"Usage", Convert.ToDouble(rnd.Next(5, 90))},
        //                                        {"FanRPM", rnd.Next(1500, 2500)},
        //                                    }
        //                            },
        //                            {"mbProbe",new BsonDocument()},
        //                            {"MEM", GetDynamicMemRawData()},
        //                            {"Storage",new BsonArray()
        //                                        {
        //                                            new BsonDocument()
        //                                            {//1
        //                                                {"Index",0},
        //                                                {"SN", "BCADevice"+(storageSN).ToString("D5")},
        //                                                {"smart",new BsonDocument()
        //                                                        {
        //                                                            {"5",0},
        //                                                            {"9",4274},
        //                                                            {"12",31},
        //                                                            {"163",14},
        //                                                            {"165",334},
        //                                                            {"167",AvgECList[StorageIndex][j]},
        //                                                            {"169",91},
        //                                                            {"170",142},
        //                                                            {"171",0},
        //                                                            {"172",0},
        //                                                            {"192",0},
        //                                                            {"194",TempData1},
        //                                                            {"229",0},
        //                                                            {"235",0 },
        //                                                            {"241",85336},
        //                                                            {"242",106513}
        //                                                        }
        //                                                },
        //                                                {"Health",HealthList[StorageIndex][j]},
        //                                                {"PECycle",3000},
        //                                                {"iAnalyzer",new BsonDocument()
        //                                                            {
        //                                                                {"Enable", 1},
        //                                                                {"SRC", 2784002},
        //                                                                {"RRC", 39870315},
        //                                                                {"SWC", 1527066},
        //                                                                {"RWC", 66674392},
        //                                                                {"SR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 17629},
        //                                                                            {"1", 60490},
        //                                                                            {"2", 732917},
        //                                                                            {"3", 153909},
        //                                                                            {"4", 727414},
        //                                                                            {"5", 1091643},
        //                                                                        }
        //                                                                },
        //                                                                {"SW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 10397},
        //                                                                            {"1", 52358},
        //                                                                            {"2", 1230507},
        //                                                                            {"3", 41972},
        //                                                                            {"4", 54111},
        //                                                                            {"5", 137721},
        //                                                                        }
        //                                                                },
        //                                                                {"RR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0",9466989},
        //                                                                            {"1",7688031},
        //                                                                            {"2",9023755},
        //                                                                            {"3",1406997},
        //                                                                            {"4",12284543}
        //                                                                        }
        //                                                                },
        //                                                                {"RW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 4381752},
        //                                                                            {"1", 2522908},
        //                                                                            {"2", 13497783},
        //                                                                            {"3", 5396054},
        //                                                                            {"4", 40875895}
        //                                                                        }
        //                                                                },
        //                                                            }
        //                                                }
        //                                            },
        //                                            new BsonDocument()
        //                                            {//2
        //                                                {"Index",1},
        //                                                {"SN", "BCADevice"+(storageSN + 1).ToString("D5")},
        //                                                {"smart",new BsonDocument()
        //                                                        {
        //                                                            {"5",0},
        //                                                            {"9",4274},
        //                                                            {"12",31},
        //                                                            {"163",14},
        //                                                            {"165",334},
        //                                                            {"167",AvgECList[StorageIndex+1][j]},
        //                                                            {"169",91},
        //                                                            {"170",142},
        //                                                            {"171",0},
        //                                                            {"172",0},
        //                                                            {"192",0},
        //                                                            {"194",TempData2},
        //                                                            {"229",0},
        //                                                            {"235",0 },
        //                                                            {"241",85336},
        //                                                            {"242",106513}
        //                                                        }
        //                                                },
        //                                                {"Health",HealthList[StorageIndex+1][j]},
        //                                                {"PECycle",3000},
        //                                                {"iAnalyzer",new BsonDocument()
        //                                                            {
        //                                                                {"Enable", 1},
        //                                                                {"SRC", 6219462},
        //                                                                {"RRC", 59308250},
        //                                                                {"SWC", 2492221},
        //                                                                {"RWC", 95052336},
        //                                                                {"SR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 58894},
        //                                                                            {"1", 103215},
        //                                                                            {"2", 1290914},
        //                                                                            {"3", 326046},
        //                                                                            {"4", 1399984},
        //                                                                            {"5", 3040409},
        //                                                                        }
        //                                                                },
        //                                                                {"SW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 40656},
        //                                                                            {"1", 124687},
        //                                                                            {"2", 2038509},
        //                                                                            {"3", 49115},
        //                                                                            {"4", 69337},
        //                                                                            {"5", 169917},
        //                                                                        }
        //                                                                },
        //                                                                {"RR",new BsonDocument()
        //                                                                        {
        //                                                                            {"0",10632528},
        //                                                                            {"1",13444033},
        //                                                                            {"2",13309369},
        //                                                                            {"3",1983239},
        //                                                                            {"4",19939081}
        //                                                                        }
        //                                                                },
        //                                                                {"RW",new BsonDocument()
        //                                                                        {
        //                                                                            {"0", 6450957},
        //                                                                            {"1", 3623980},
        //                                                                            {"2", 18445399},
        //                                                                            {"3", 7694365},
        //                                                                            {"4", 58837635}
        //                                                                        }
        //                                                                },
        //                                                            }
        //                                                }
        //                                            }
        //                                        }
        //                            },
        //                            {"Ext",new BsonDocument()},
        //                            {"time", timestamp[j]}
        //                        };
        //                    }
        //                    StorageIndex += 2;
        //                    storageSN += 2;
        //                    break;
        //            }
        //        }                     
        //    }          

        //    //Make Event "temperature over thershold" Data
        //    int eventIndex = 0;
        //    foreach(EventTemp et in etList)
        //    {
        //        TestingEventData[eventIndex++]=new BsonDocument()
        //        {
        //            {"Dev",et.deviceName},
        //            {"Message", "Storage "+et.storageSN+" "+"temperature over thershold, value : "+et.Temp+" celsius."},
        //            {"Checked", ((rnd.NextDouble() > 0.5) ? false : true)},
        //            //{"Level", 0},
        //            {"Time", (double)rnd.Next(1506902400, 1506999540)}
        //        };
        //    }

        //    //Make Event "lifespan over thershold" Data
        //    foreach(EventLife el in elList)
        //    {
        //        TestingEventData[eventIndex++] = new BsonDocument()
        //        {
        //            {"Dev", el.deviceName},
        //            {"Message", "Storage "+el.storageSN+" "+"lifespan over thershold, value : "+el.Lifespan+" days."},
        //            {"Checked", ((rnd.NextDouble() > 0.5) ? false : true)},
        //            {"Time", (double)rnd.Next(1506902400, 1506999540)}
        //        };
        //    }

        //    //Make Event Status Data

        //    //Setting
        //    List<int> deviceList = new List<int>(new List<int>(Enumerable.Range(1, (int)Total)));            
        //    deviceList = deviceList.OrderBy(o => rnd.Next()).ToList<int>();

        //    //Make       

        //    for( i= 0; i< EventDeviceOfflineCount; i++)
        //    {
        //        TestingEventData[eventIndex++] = new BsonDocument()
        //        {
        //            {"Dev", "Device"+deviceList[i].ToString("D5")},
        //            {"Message", "Device"+deviceList[i].ToString("D5")+" offline"},
        //            {"Checked", ((rnd.NextDouble() > 0.5) ? false : true)},
        //            //{"Level", 0},
        //            {"Time", (double)rnd.Next(1506988800, 1506999540)}
        //        };
        //    }

        //}
    }
}
