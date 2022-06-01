using System;
using System.Collections.Generic;

namespace ShareLibrary.AdminDB
{
    public partial class Data
    {
        public Data()
        {
            Widget = new HashSet<Widget>();
            Devicedatalist = new HashSet<Devicedatalist>();
            DataChartList = new HashSet<DataChartList>();
            Threshold = new HashSet<Threshold>();
        }
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Name { get; set; }
        public int GroupId { get; set; }
        public string Location { get; set; }
        public bool Numberical { get; set; }
        public string Unit { get; set; }
        public bool Dynamic { get; set; }
        public string EventMessage { get; set; }
        public virtual ICollection<Widget> Widget { get; set; }
        public virtual DataGroup Group { get; set; }
        public virtual ICollection<Devicedatalist> Devicedatalist { get; set; }
        public virtual ICollection<DataChartList> DataChartList { get; set; }
        public virtual ICollection<Threshold> Threshold { get; set; }
    }
}
