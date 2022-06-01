using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShareLibrary.DataTemplate;

namespace DashboardAPI.Models.Dashboard
{
    public class PanelItem
    {
        //public int id { get; set; }
        public int widgetId { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string width { get; set; }
        public string label { get; set; }
        public object[] data { get; set; }
        public double lng { get; set; }
        public double lat { get; set; }
        //public Location[] location { get; set; } 
        //public int mapIndex { get; set; }
        //public int[] eventCount { get; set; }
        //public int[] deviceCount { get; set; }
       
    }
}
