using MongoDB.Bson;
using ShareLibrary;
using ShareLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace MockDataCreate.Models
{
    public class MockDataStatic
    {
        public MockDataStatic(Random _rnd)
        {
            rnd = _rnd;
            rcd = new RedisCacheDispatcher();
            rcd.GetConnectionString();
        }
        Random rnd;
        public Location Location { get; set; }
        public DeviceRange DeviceRange { get; set; }
        IRedisCacheDispatcher rcd;

        string[] osTypes = new string[] { "Microsoft Windows 10 Enterprise", "Microsoft Windows 10 Enterprise", "Microsoft Windows 10 Enterprise", "Ubuntu 16.04 LTS", "Ubuntu 16.04 LTS", "Ubuntu 16.04 LTS" };
        string[] osVerTypes = new string[] { "x86_64", "x86_64", "x86_64", "16.04 LTS", "16.04 LTS", "16.04 LTS" };
        string[] mountAtTypes = new string[] { "C:\\", "D:\\", "E:\\", "sda", "sda", "sda" };

        BsonDocument GetSystem(string osType, string OSVerType, Location location, DeviceRange deviceRange, bool isTx2)
        {
            string OSArch = "64-bit";
            string name = "innotest";
            if (isTx2)
            {
                osType =  "Ubuntu 16.04.5 LTS";
                OSVerType = "16.04.5 LTS (Xenial Xerus)" ;
                OSArch = "Linux 4.4.38-tegra aarch64";
                name = "tegra-ubuntu";
            }
            double longitude, latitude;
            if (location == null)
            {
                longitude = rnd.Next(deviceRange.MinX, deviceRange.MaxX) / 1000000.0;
                latitude = rnd.Next(deviceRange.MinY, deviceRange.MaxY) / 1000000.0;
            }
            else
            {
                longitude = location.Longitude;
                latitude = location.Latitude;
            }
            return new BsonDocument() {
                {"OS" ,osType},
                {"OSVer", OSVerType},
                {"OSArch", OSArch},
                {"Name", name},
                {"Longitude", longitude},
                {"Latitude", latitude}
            };
        }
        BsonDocument Dev(string name)
        {
            return new BsonDocument()
            {
                { "Alias", ""},
                { "Name", name}
            };
        }
        BsonDocument GetSPDMemoryMockData()
        {
            return new BsonDocument()
            {
               {"Cap", 20971520},
                {"Slot", new BsonDocument()
                        {
                            {"0",new BsonDocument()
                                    {
                                        {"Type", "DDR4"},
                                        {"PN", "i-DIMM"},
                                        {"Manu", "InnoDisk Corporation"},
                                        {"Cap", 4},
                                        {"SN", "00000006"},
                                        {"Date", "24 / 18" },
                                        {"DIMMType", "ECC UDIMM" },
                                        {"OPTemp", "N/A*"},
                                        {"IC_Cfg", "512Mb x 8"},
                                        {"IC_Brand", "Innodisk"},
                                        {"Therm",  "N/A"},
                                        {"CAS_Ltc", 17.22},
                                        {"Rate", 2132},
                                        {"Feature", new BsonDocument(){
                                            {"sICGrade", "Original Grade (IC P/N visible on IC)"},
                                            {"AntiSul", rnd.Next(0, 3)},
                                            {"30GF", (rnd.Next(0, 2) == 0)? false : true},
                                            {"45GF", (rnd.Next(0, 2) == 0)? false : true},
                                            {"bWP", (rnd.Next(0, 2) == 0)? false : true},
                                        } }
                                    }
                            },
                            {"1",new BsonDocument()
                                    {
                                        {"Type", "DDR4"},
                                        {"PN", "i-DIMM"},
                                        {"Manu", "InnoDisk Corporation"},
                                        {"Cap", 4},
                                        {"SN", "00000006"},
                                        {"Date", "24 / 18" },
                                        {"DIMMType", "ECC UDIMM" },
                                        {"OPTemp", "N/A*"},
                                        {"IC_Cfg", "512Mb x 8"},
                                        {"IC_Brand", "Innodisk"},
                                        {"Therm",  "N/A"},
                                        {"CAS_Ltc", 17.83},
                                        {"Rate", 2132},
                                        {"Feature", new BsonDocument(){
                                            {"sICGrade", "Original Grade (IC P/N visible on IC)"},
                                            {"AntiSul", rnd.Next(0, 3)},
                                            {"30GF",(rnd.Next(0, 2) == 0)? false : true},
                                            {"45GF", (rnd.Next(0, 2) == 0)? false : true},
                                            {"bWP", (rnd.Next(0, 2) == 0)? false : true},
                                        } }
                                    }
                            }
                        }
                }
            };
        }
        BsonArray GetStorageMockData(MockStorageFactory.MockStorage[] mockStorages, bool enableOOB, string mountAt)
        {
            BsonArray ret = new BsonArray();
            string FWVer = "S16425";
            for (var i=0; i< mockStorages.Length; i++)
            {
                if (enableOOB)
                {
                    MockOOBStatus(mockStorages[i].StorageSN);
                    FWVer = "B16425";
                }
                ret.Add(new BsonDocument()
                {
                    {"Index", i},
                    {"Model","2.5\"SATA SSD 3ME3"},
                    {"SN", mockStorages[i].StorageSN},
                    {"FWVer", FWVer},
                    {"Par", new BsonDocument()
                            {
                                {"TotalCap", 250051725},
                                {"NumofPar", 1},
                                {"ParInfo",new BsonArray()
                                            {
                                                new BsonDocument()
                                                {
                                                    {"MountAt", mountAt},
                                                    {"Capacity", 249544704}
                                                }
                                            }
                                }
                            }
                    }
                });
            }
            return ret;
        }
        public BsonArray GetNetMockData(int ipIndex)
        {
            BsonArray ret = new BsonArray();

            ret.Add(new BsonDocument()
            {
                {"Name","eth0" },
                {"Type","Ethernet"},
                {"MAC","aa:bb:cc:dd:ee:ff"},
                {"IPv6","" },
                {"IPaddr","192.168.0." + ipIndex},
                {"Netmask","255.255.255.0"}
            });
            return ret;
        }
        BsonDocument GetCANBusData() {
            return new BsonDocument()
            {
                {"0", new BsonDocument()
                    {
                        {"Name", "Engine Speed"},
                        {"Unit", "rpm"},
                        {"Type", 0 }
                    }
                },
                {"1", new BsonDocument()
                    {
                        {"Name", "Tachograph Vehicle Speed"},
                        {"Unit", "km/h"},
                        {"Type", 0 }
                    }
                },
                {"2", new BsonDocument()
                    {
                        {"Name", "Engine Coolant Temperature"},
                        {"Unit", "℃"},
                        {"Type", 0 }
                    }
                },
                {"3", new BsonDocument()
                    {
                        {"Name", "Engine Total Fuel Used"},
                        {"Unit", "kg"},
                        {"Type", 0 }
                    }
                },
                {"4", new BsonDocument()
                    {
                        {"Name", "Boots Pressure"},
                        {"Unit", "kpg"},
                        {"Type", 0 }
                    }
                },
                {"5", new BsonDocument()
                    {
                        {"Name", "Accelerator Pedal Position"},
                        {"Unit", "%"},
                        {"Type", 0 }
                    }
                }
            };
        }
        BsonDocument GetMB(bool IsTx2, int DeviceIndex) {
            if (IsTx2)
            {
                return new BsonDocument() {
                    {"Manu", "Aetina Corporation."},
                    {"Product", "ACE-N310" },
                    {"SN", "012101705311" + DeviceIndex},
                    {"BIOSManu", ""},
                    {"BIOSVer", ""},
                    {"mbTemp", new BsonDocument(){ }}
                };
            }
            return new BsonDocument() {
                {"Manu","Gigabyte Technology Co., Ltd."},
                {"Product","Z270X-UD3-CF" },
                {"SN","Default string" },
                {"BIOSManu","American Megatrends Inc." },
                {"BIOSVer","ALASKA - 1072009"},
                {"mbTemp", new BsonDocument(){ }}
            };
        }
        BsonDocument GPU() {
            return new BsonDocument()
            {
                {"Name", "NVIDIA Tegra X2"},
                {"Arch", "NVIDIA Pascal™"},
                {"DriverVer", "9.0"},
                {"ComputeCap", "6.2"},
                {"CoreNum", "256"},
                {"MemType", "LDDR4"},
                {"MemBusWidth", "128-bit"},
                {"MemSize", "7846 MB"},
                {"MemBandWidth", "59.7 GB/s"},
                {"Clock", "1301 MHz"},
                {"MemClock", "1600MHz"}
            };
        }
        void MockOOBStatus(string storageSN) {
            rcd.SetCache(2, storageSN, "1");
        }
        public BsonDocument GetMockData(string deviceName, MockStorageFactory.MockStorage[] mockStorages, DeviceCount.MockDevice mockDevice, int DeviceIndex, int mockStartTimestamp)
        {
            Int32 staticTime = mockStartTimestamp;
            var randomOs = rnd.Next(6);
            var osType = osTypes[randomOs];
            var OSVerType = osVerTypes[randomOs];
            var mountAt = mountAtTypes[randomOs];
            var spec = mockDevice.Spec;
            var enableOOB = spec.OOB;
            var enableCANBus = spec.CANBus;
            var enableGPU = spec.GPU;
            var deviceRange = mockDevice.DeviceRange;
            var location = mockDevice.Location;

            return new BsonDocument() {
                {"Dev", Dev(deviceName)},
                {"Sys", GetSystem(osType, OSVerType, location, deviceRange, enableGPU)},
                {"CPU",new BsonDocument()
                      {
                         {"Manu", "GenuineIntel"},
                         {"Name", "Intel(R) Core(TM) i5-7500 CPU @ 3.40GHz"},
                         {"Numofcore", 4},
                         {"L2", 1024 },
                         {"L3", 6144 }
                       }
                },
                {"MB", GetMB(enableGPU, DeviceIndex)},
                {"MEM", GetSPDMemoryMockData()},
                {"Storage", GetStorageMockData(mockStorages, enableOOB, mountAt)},
                {"Net", GetNetMockData(DeviceIndex)},
                {"Ext", !enableCANBus ? new BsonDocument(): GetCANBusData()},
                {"Remote",new BsonDocument()},
                {"time", staticTime},
                {"GPU",  enableGPU? GPU(): new BsonDocument()}
            };
        }
    }
}
