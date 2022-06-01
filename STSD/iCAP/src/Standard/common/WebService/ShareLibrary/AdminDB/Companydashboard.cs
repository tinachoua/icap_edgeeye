using System;
using System.Collections.Generic;

namespace ShareLibrary.AdminDB
{
    public partial class Companydashboard
    {
        public Companydashboard()
        {
            Companydashboardelement = new HashSet<Companydashboardelement>();
            //Companydashboardlist = new HashSet<Companydashboardlist>();
        }

        public int Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Name { get; set; }
        
        public virtual ICollection<Companydashboardelement> Companydashboardelement { get; set; }
        public virtual Company Company { get; set; }
        //public virtual ICollection<Companydashboardlist> Companydashboardlist { get; set; }
    }
}
