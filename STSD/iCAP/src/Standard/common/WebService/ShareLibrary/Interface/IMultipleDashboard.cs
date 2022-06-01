using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.DataTemplate;
using ShareLibrary.AdminDB;

namespace ShareLibrary.Interface
{
    public interface IMultipleDashboard
    {
        int[] GetDashboardIdList(int companyId);
        PanelItemTemplate[] GetPanelItem(int dashboardId);
        string GetDataLocation(int DataId);
        string[] GetDeviceListByWidgetId(int widgetId);
        string[] GetDashboardName(int companyId);
        SelectOptionTemplate[] GetDashboardList();
        WidgetOption[] GetDashboardWidgetList(int dashboardId);
        void SetWidgetOrder(WidgetOrderTemplate dashboardElement);
        void Create(string dashboardName);
        //void Update(DashboardTemplate updateDashboard);
        bool SaveDashboardInfo(DashboardTemplate dashboardInfo);
        void Delete(int dashboardId);
        string GetNameByDashboardId(int dashboardId);
        int[] GetWidgetId(int dashboardId);
        PanelTemplate GetPanelInfo();
        PanelTemplate GetNewPanelInfo();
        object[] GetDeviceNameAliasOwnerListByWidgetId(int widgetId);
        bool DashboardExist(int dashboardId);
        bool WidgetInDashboard(int dashboardId, int widgetId);

    }
}
