using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardAPI.Models.Setting
{
    public class ThresholdData
    {
        public string Class { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public int Enable { get; set; }
        public int Func { get; set; }
    }
}
