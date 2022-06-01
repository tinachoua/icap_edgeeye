using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class WidgetInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }      
        public string ChartName { get; set; }
        public string[] Label { get; set; }
        public string Width { get; set; }        
    }

    public class EditDashboardTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public WidgetInfo[] WidgetInfo { get; set; }
        public int[] WidgetIdList { get; set; }
    }
}
