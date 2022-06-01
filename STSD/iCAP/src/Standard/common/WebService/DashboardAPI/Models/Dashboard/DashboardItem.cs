using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardAPI.Models.Dashboard
{
    public class DashboardItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public PanelItem[] panels { get; set; }
    }
}
