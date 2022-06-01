using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardAPI.Models.Dashboard
{
    public struct loc
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class MapItem
    {
        public int id { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public string status { get; set; }
        public string owner { get; set; }
        public string detail { get; set; }
        public string time { get; set; }
        public loc position { get; set; }
    }
}
