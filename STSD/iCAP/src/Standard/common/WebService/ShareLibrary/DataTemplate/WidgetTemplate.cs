using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class Threshold
    {
        public string Type { get; set; }
        public string Name { get; set; }
    }

    public class Percentage
    {
        public double[] Divider { get; set; }
        public int DenominatorId { get; set; }        
    }

    public class Boolean
    {
        public string[] label { get; set; }
    }

    public class Numberical
    {
        public double[] Divider { get; set; }
    }

   public class WidgetTemplate
   {
        public int WidgetId { get; set; }
        public string Name { get; set; }
        public int? DataId { get; set; }
        public int? ThresholdId { get; set; }
        public int DataCount { get; set; }
        public int? ChartId { get; set; }
        public byte Width { get; set; }      
        public int[] BranchIdList { get; set; }
        public int? DataGroupId { get; set; }
        public int? ProcessType { get; set; }
        public string SettingStr { get; set; }
        public int MapIndex { get; set; }
        public MapItemTemplate.Pos MapCenter { get; set; }
        public Percentage Percentage { get; set; }
        public Boolean Boolean { get; set; }
        public Numberical Numberical { get; set; }
    }
}
