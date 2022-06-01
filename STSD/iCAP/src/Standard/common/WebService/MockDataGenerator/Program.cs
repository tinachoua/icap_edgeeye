using System;
using ShareLibrary.AdminDB;
using ShareLibrary;
using ShareLibrary.Interface;
using MongoDB.Bson;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using MockDataCreate.Models;


namespace MockDataCreate
{
    class Program
    {
        static void Main(string[] args)
        {
            uint EventDeviceOfflineCount = 0;
            DeviceCount deviceCount;
            if (File.Exists("DeviceCount.json"))
            {
                var serializer = new JsonSerializer();
                using (StreamReader r = File.OpenText("DeviceCount.json"))
                using (var jsonTextReader = new JsonTextReader(r))
                {
                    deviceCount = serializer.Deserialize<DeviceCount>(jsonTextReader);
                }
            }
            else
            {
                deviceCount = new DeviceCount();

                using (StreamWriter sw = new StreamWriter(new FileStream("DeviceCount.json", FileMode.CreateNew)))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(deviceCount));
                }
            }
            deviceCount.Init();

            uint Total = deviceCount.GetTotal();

            if (Total > 100)
            {
                EventDeviceOfflineCount = (uint)Math.Floor(Total / 12.0);
            }
            else
            {
                EventDeviceOfflineCount = (uint)Math.Floor(Total / 10.0);
            }
        
            //Arrangement           
            DataProducer dp = new DataProducer();
            DBWriter dbw = new DBWriter();
            IDataProducer _dp = dp;
            IDBWriter _dbw = dbw;

            //AdminDB
            Device[] TestingDevice = new Device[Total];
            Devicecertificate[] TestingDeviceCertificate = new Devicecertificate[Total];
            //BranchDeviceList[] TestBranchDeviceElement = new BranchDeviceList[Total];

            _dp.DataMaker(deviceCount, TestingDevice);
            _dp.DataMaker(Total, TestingDeviceCertificate);
            //_dp.DataMaker(Total, TestBranchDeviceElement);

            try
            {
                _dbw.DataWriter(TestingDevice);
            }
            catch (Exception e)
            {
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "MockDataCreate",
                    URL = "",
                    ResponseCode = 0,
                    Remark = $"Insert Device Data Exception Info : \r\n{e.Message}\r\n"
                });
            }

            try
            {
                _dbw.DataWriter(TestingDeviceCertificate);
            }
            catch (Exception e)
            {
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "MockDataCreate",
                    URL = "",
                    ResponseCode = 0,
                    Remark = $"Insert TestingDeviceCertificate Exception Info : \r\n{e.Message}\r\n"
                });
            }

            try
            {
                _dbw.InsertBranchDevicelist(deviceCount);
            }
            catch (Exception e)
            {
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "MockDataCreate",
                    URL = "",
                    ResponseCode = 0,
                    Remark = $"Insert TestBranchDeviceElement Exception Info : \r\n{e.Message}\r\n"
                });
            }

            //MongoDB
            MockDataFactory.TestingData TestingData = new MockDataFactory.TestingData();

            try
            {
                _dp.DataMaker(deviceCount, TestingData);
            }
            catch (Exception e)
            {
                LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                {
                    Direction = true,
                    Name = "",
                    Method = "MockDataCreate",
                    URL = "",
                    ResponseCode = 0,
                    Remark = $"DataMaker Info : \r\n{e.Message}\r\n"
                });
            }

            bool exist = _dbw.CheckDataDBDataExist();

            if (!exist)
            {
                int dataMaxCount = TestingData.GetMaxDataCount();
                TestingData.EventData.Sort(delegate (BsonDocument c1, BsonDocument c2) { return c1["Time"].CompareTo(c2["Time"]); });

                for (var i=0; i < dataMaxCount; i++)
                {
                    try
                    {
                        string CollectionName;
                        if ( i < TestingData.StaticData.Count)
                        {
                            CollectionName = "Device" + (i + 1).ToString("D5") + "-static";
                            _dbw.DataWriter(CollectionName, TestingData.StaticData[i]);
                        }
                        if (i < TestingData.DynamicData.Count)
                        {
                            CollectionName = "Device" + (i + 1).ToString("D5") + "-dynamic";
                            for (int j = 0; j < MockDataDynamic.DYNAMIC_DATA_COUNT; j++)
                            {
                                _dbw.DataWriter(CollectionName, TestingData.DynamicData[i][j]);
                            }
                        }
                        if (i < TestingData.StorageAnalyzerData.Count)
                        {
                            CollectionName = "StorageAnalyzer";
                            _dbw.DataWriter(CollectionName, TestingData.StorageAnalyzerData[i]);
                        }
                        if (i < TestingData.EventData.Count)
                        {
                            CollectionName = "EventLog";
                            _dbw.DataWriter(CollectionName, TestingData.EventData[i]);
                        }

                    }
                    catch(Exception e)
                    {
                        LogAgent.WriteToLog(new LogAgent.LogFileFormat()
                        {
                            Direction = true,
                            Name = "",
                            Method = "Main",
                            URL = "",
                            ResponseCode = 0,
                            Remark = $"Insert Data Exception Info : \r\n{e.Message}\r\n"
                        });
                        break;
                    }
                }
            }
        }
    }
}
