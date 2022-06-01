using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.AdminDB;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ShareLibrary.DataTemplate
{
    public class PanelItemTemplate
    {
        public int WidgetId { get; set; }
        public string WidgetName { get; set; }
        public string WidgetType { get; set; }
        public byte WidgetWidth { get; set; }
        public string DataName { get; set; }
        public string Unit { get; set; }
        public int DataCount { get; set; }
        public string DataLocation { get; set; }
        public string SettingStr { get; set; }
        public int? GroupId { get; set; }
        public int? ThresholdId { get; set; } 

       // public int[] BranchId { get; set; }
    }
}
