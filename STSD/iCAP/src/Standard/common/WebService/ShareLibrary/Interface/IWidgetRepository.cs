using System;
using System.Collections.Generic;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using ShareLibrary.AdminDB;
using ShareLibrary.DataTemplate;


namespace ShareLibrary.Interface
{
   public interface IWidgetRepository
    {
        bool CheckDataExists(int? Id);
        void Create(WidgetTemplate w);
        void Create(string widgetName);
        WidgetTemplate Get(int Id);
        bool Update(WidgetTemplate w);
        void Delete(int Id);
        SelectOptionTemplate[] GetDataGroupList();
        NumbericalDataOption GetNumbericalDataList();
        SelectOptionTemplate[] GetChartSelect(int? datId);
        object[] GetChartSelect();
        SelectOptionTemplate[] GetMapList();
        ChartSizeTemplate GetChartSize(int chartId);
        ChartSizeTemplate[] GetChartSize();
        SelectOptionTemplate[] GetWidgetList();
        //DataSelect[] GetDataSelect();
        SelectOptionTemplate[] GetBranchSelect();
        //ChartWidthSelect[] GetChartWidthSelect(int? dataId);
        WidgetInfo[] GetWidgetInfo();
        PanelItemTemplate GetPanelSetting(int widgetId);
        DeviceProfileTemplate[] GetDevice(int widgetId);
        DeviceProfileTemplate GetDevice(string devName);
        object GetWidgetNameAndWidth(int widgetId);
    }
}
