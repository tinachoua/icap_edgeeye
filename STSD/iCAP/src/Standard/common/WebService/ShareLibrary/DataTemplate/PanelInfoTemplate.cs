using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class PanelTemplate
    {
        public class WidgetSettingFormat
        {
            public class DataContent
            {
                public string Name { get; set; }
                public string Path { get; set; }
                public string Unit { get; set; }
            }
            public int WidgetId { get; set; }
            public int? DataId { get; set; }
            public int? ThresholdId { get; set; }
            public object ThresholdSetting { get; set; }
            public string WidgetName { get; set; }
            public string ChartType { get; set; }
            public byte WidgetWidth { get; set; }
            public SettingItemTemplate Setting { get; set; }
            public object[] Devices { get; set; }
            public DataContent Denominator { get; set; }
            public DataContent Data { get; set; }
            public object[] Markers { get; set; }

        }
        public WidgetSettingFormat[] WidgetSetting { get; set; }
    }
}