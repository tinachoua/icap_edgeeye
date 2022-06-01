using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public partial class DataChartList
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }        
        public int DataId { get; set; }
        public int ChartId { get; set; }
        public virtual Data Data { get; set; }
        public virtual Chart Chart { get; set; }
    }
}
