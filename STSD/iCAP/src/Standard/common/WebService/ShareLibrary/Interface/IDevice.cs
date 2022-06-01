using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.AdminDB;
using ShareLibrary.DataTemplate;
using Microsoft.AspNetCore.Http;


namespace ShareLibrary.Interface
{
    public interface IDevice : IImages
    {
        int Count();
        void Create(Device dev);
        bool Delete(string devName);
        Device Get(int DeviceId);
        Device Get(string devName);
        DeviceProfileTemplate GetDeviceProfile(string devName);
        int GetID(string devName);
        List<string> GetList();
        List<Device> GetList(int branchId);
        MapItemTemplate[] GetLocation();
        MapItemTemplate GetLocation(string devName);
        string GetOwner(string devName);
        bool? UpdateDeviceProfile(DeviceProfileTemplate devProfile);
        Dictionary<String, String> GetOwner_All();
        SelectOptionTemplate[] GetDeviceList();
        string GetAlias(string devName);
        DeviceProfileTemplate GetAliasOwner(string devName);
        
        Dictionary<String, String> GetAlias_All();

        string Screenshot(string devName);
        Device[] GetDeviceList(int branchId);

        bool UpdateAlias(string devName, string alias);
        //SelectOptionTemplate[] GetSelectedDevice(int branchId);

        //SelectOptionTemplate[] GetUnselectedDevice(int branchId);
        //bool AllowedFileExtensions(List<IFormFile> files);
        //string GetImgBase64(int devId);        
        //bool UploadImg(List<IFormFile> files,bool overwrite,int devId);
    }
}
