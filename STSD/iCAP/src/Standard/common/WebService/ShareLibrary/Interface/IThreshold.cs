using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.DataTemplate;

namespace ShareLibrary.Interface
{
    public interface IThreshold
    {
        SelectOptionTemplate[] GetThresholdList();
        void Create(ThresholdTemplate thresholdInfo);
        bool Update(ThresholdTemplate thresholdInfo);
        void Delete(int id);
        SelectOptionTemplate[] GetSelectedGroup(int id);
        bool NameExists(string thresholdName);
        bool NameExists(string thresholdName, int id);
        //ThresholdTemplate GetDetail(int id);
        bool IsEnable(int thresholdId);
        bool ThresholdExists(int thresholdId);
        bool Save(ThresholdTemplate.GroupSetting thGroupSetting);
        object[] GetSetting();
        ThresholdTemplate.WebSetting GetSetting(int thresholdId);
        AdminDB.Threshold GetThresholdSetting(int thresholdId);
    }
}
