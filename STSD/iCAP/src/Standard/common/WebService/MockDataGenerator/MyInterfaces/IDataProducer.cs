using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.AdminDB;
using ShareLibrary;
using ShareLibrary.DataTemplate;
using MongoDB.Bson;
using MockDataCreate.Models;

namespace MockDataCreate
{
    interface IDataProducer
    {
        void DataMaker(ushort Count, Device[] TestingDevice);
        void DataMaker(DeviceCount deviceCount, Device[] TestingDevice);
        void DataMaker(uint Count, Devicecertificate[] TestingDevice);
        void DataMaker(DeviceCount deviceCount, MockDataFactory.TestingData TestingData);
        //void DataMaker(uint Count, BranchDeviceList[] TestingDevice);
    }
}
