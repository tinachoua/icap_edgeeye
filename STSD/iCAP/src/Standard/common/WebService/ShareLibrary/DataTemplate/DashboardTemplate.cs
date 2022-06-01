using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class DashboardTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public int OriginalCount { get; set; }
        //public int[] WidgetIdRemovedList { get; set; }
        //public int[] WidgetIdAddedList { get; set; }
        public int[] WidgetIdList { get; set; }
    }
}
