using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public partial class Branchdashboard
    {
        public Branchdashboard()
        {
            Branchdashboardelement = new HashSet<Branchdashboardelement>();
            Branchdashboardlist = new HashSet<Branchdashboardlist>();
        }

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Branchdashboardelement> Branchdashboardelement { get; set; }
        public virtual ICollection<Branchdashboardlist> Branchdashboardlist { get; set; }
    }
}
