using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DashboardAPI.Models.Event
{
    public class EventData
    {
        public string EventId { get; set; }
        public string time { get; set; }
        public string eventclass { get; set; }
        public string devName { get; set; }
        public string info { get; set; }
        public int level { get; set; }
        public string owner { get; set; }
        public bool IsChecked {get;set;}
    }
}
