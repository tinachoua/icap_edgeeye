using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.DataTemplate;

namespace ShareLibrary.DataTemplate
{    
    public class DataSelect
    {
        public SelectOptionTemplate GroupOption { get; set; }
        public SelectOptionTemplate[] DataOption { get; set; }
    }

    public class ChartWidthSelect
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ChartSizeTemplate ChartSize { get; set; }

    }

    public class EditWidgetTemplate
    {
        public WidgetTemplate WidgetTemplate { get; set; }        
        public DataSelect[] DataSelect { get; set; }
        public SelectOptionTemplate[] BranchOption { get; set; }
        //public ChartWidthSelect[] ChartWidthSelect { get; set; }
        public object[] ChartOption { get; set; }
        public ChartSizeTemplate[] SizeOption { get; set; }
        public SelectOptionTemplate[] ThresholdOption { get; set; }

        //public List<ThresholdSettingTemplate> ThresholdSetting { get; set; }
    }
}
