using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.AdminDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using ShareLibrary;
using System.Linq;
using System.IO;
using MockDataCreate.Models;

namespace MockDataCreate
{
    public class DBWriter: DataDBDispatcher, IDBWriter
    {     
        // AdminDB     
        private icapContext ic = new icapContext();

        public void DataWriter(Device[] TestingDevice)
        {
            Device dv = ic.Device.Find(1);

            if(dv==null)
                foreach (Device d in TestingDevice)
                {                
                    ic.Device.Add(d);
                }
                ic.SaveChanges();
        } 

        public void InsertBranchDevicelist(DeviceCount deviceCount) {
            int index = 1;
            try
            {
                foreach (var setting in deviceCount.TW)
                {
                    for (var i = 0; i < setting.Total; i++)
                    {
                        int deviceId = index + i;
                        for (var j = 0; j < setting.BranchIdlist.Length; j++)
                        {
                            ic.BranchDeviceList.Add(new BranchDeviceList()
                            {
                                BranchId = setting.BranchIdlist[j],
                                DeviceId = deviceId
                            });
                        };
                    }
                    index += (int)setting.Total;
                }
                foreach (var setting in deviceCount.USA)
                {
                    for (var i = 0; i < setting.Total; i++)
                    {
                        int deviceId = index + i;
                        for (var j = 0; j < setting.BranchIdlist.Length; j++)
                        {
                            ic.BranchDeviceList.Add(new BranchDeviceList()
                            {
                                BranchId = setting.BranchIdlist[j],
                                DeviceId = deviceId
                            });
                        };
                    }
                    index += (int)setting.Total;
                }
                foreach (var setting in deviceCount.JP)
                {
                    for (var i = 0; i < setting.Total; i++)
                    {
                        int deviceId = index + i;
                        for (var j = 0; j < setting.BranchIdlist.Length; j++)
                        {
                            ic.BranchDeviceList.Add(new BranchDeviceList()
                            {
                                BranchId = setting.BranchIdlist[j],
                                DeviceId = deviceId
                            });
                        };
                    }
                    index += (int)setting.Total;
                }
                foreach (var setting in deviceCount.NL)
                {
                    for (var i = 0; i < setting.Total; i++)
                    {
                        int deviceId = index + i;
                        for (var j = 0; j < setting.BranchIdlist.Length; j++)
                        {
                            ic.BranchDeviceList.Add(new BranchDeviceList()
                            {
                                BranchId = setting.BranchIdlist[j],
                                DeviceId = deviceId
                            });
                        };
                    }
                    index += (int)setting.Total;
                }
                foreach (var setting in deviceCount.CN)
                {
                    for (var i = 0; i < setting.Total; i++)
                    {
                        int deviceId = index + i;
                        for (var j = 0; j < setting.BranchIdlist.Length; j++)
                        {
                            ic.BranchDeviceList.Add(new BranchDeviceList()
                            {
                                BranchId = setting.BranchIdlist[j],
                                DeviceId = deviceId
                            });
                        };
                    }
                    index += (int)setting.Total;
                }
                ic.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

//        public void DataWriter(BranchDeviceList[] TestBranchDeviceElement)
//        {
//            foreach(BranchDeviceList b in TestBranchDeviceElement)
//            {
//                ic.BranchDeviceList.Add(b);
//            }
//#if DEMO
//            ////TX2
//            for (int i = 2; i < 12; i++)
//            {
//                ic.BranchDeviceList.Add(new BranchDeviceList()
//                {
//                    Id = TestBranchDeviceElement.Length + i,
//                    BranchId = 2,
//                    DeviceId = i,
//                });
//            }
//#endif
//            ic.SaveChanges();
//        }
        public void DataWriter(Devicecertificate[] TestingDeviceCertificate)
        {
            Devicecertificate dc = ic.Devicecertificate.Find(1);

            if (dc==null)
                foreach (Devicecertificate d in TestingDeviceCertificate)
                {
                    ic.Devicecertificate.Add(d);
                }
                ic.SaveChanges();
        }

      
        // MongoDB
       

        //Check
        public bool CheckDataDBDataExist()
        {
            this.GetConnectionString();
            BsonDocument b = GetLastRawData("Device00001-static");

            if (b == null)
                return false;
            return true;
        }
        //Write
        public void DataWriter(string CollectionName, BsonDocument bd)
        {
            //this.GetConnectionString();
            var mongoclient = new MongoClient(mongoDBConnectionString);
            var database = mongoclient.GetDatabase(DBName);
            var collection = database.GetCollection<BsonDocument>(CollectionName);

            collection.InsertOne(bd);   
        }       
    }
}
