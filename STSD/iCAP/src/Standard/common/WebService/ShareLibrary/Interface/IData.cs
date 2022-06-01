using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.DataTemplate;

namespace ShareLibrary.Interface
{
    public interface IData
    {
        DataSelect[] GetDataSource();
        string GetDataLocation(int dataId);
        int GetExpiryDate();
        bool UpdateExpiryDate(int days);
    }
}
