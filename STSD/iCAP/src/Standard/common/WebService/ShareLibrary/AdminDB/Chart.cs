using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public partial class Chart
    {
        public Chart()
        {
            DataChartlist = new HashSet<DataChartList>();
            Widget = new HashSet<Widget>();
            Employee = new HashSet<Employee>();
        }
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Name { get; set; }
        public int SizeFlag { get; set; }

        //Type:
        //first bit means dashboard char or not
        //second bit means group chart or not
        //third bit means map or not
        public int Type { get; set; }
        public virtual ICollection<DataChartList> DataChartlist { get; set; }
        public virtual ICollection<Widget> Widget { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }

    }
}
