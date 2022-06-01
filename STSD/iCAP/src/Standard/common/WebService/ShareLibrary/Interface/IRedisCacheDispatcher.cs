using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.DataTemplate;

namespace ShareLibrary.Interface
{
    public interface IRedisCacheDispatcher
    {
        void GetConnectionString();
        bool ClearCache();
        bool SetCache(int dbIndex, string key, string value);
        bool SendPWD(string account, string pwd);
        void StartSubscribe();
        bool AddDevice(string devName);
        string GetCache(int dbIndex, string key);
        bool SetStatus(string key, int value);
        string GetStatus(string key);
        bool CleanDeviceStatus();
        bool PublishSetThreshold(ThresholdSettingTemplate d);
        List<KeyValuePair<string, string>> GetAll(int dbIndex);
        void KeyDelete(int dbIndex, string key);
        void KeyDeleteByValue(int dbIndex, string value);
    }
}
