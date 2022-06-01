using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.AdminDB;
using MongoDB.Bson;
using MockDataCreate.Models;

namespace MockDataCreate
{
    interface IDBWriter
    {
        void DataWriter(Device[] TestingDevice);
        void DataWriter(Devicecertificate[] TestingDeviceCertificate);
        //void DataWriter(BranchDeviceList[] TestingBranchDeviceElement);
        bool CheckDataDBDataExist();
        void DataWriter(string CollectionName, BsonDocument bd);
        void InsertBranchDevicelist(DeviceCount DeviceCount);
    }
}
