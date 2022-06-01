using System;
using System.Collections.Generic;

namespace ShareLibrary.AdminDB
{
    public partial class Company
    {
        public Company()
        {
            Branch = new HashSet<Branch>();
            //Companydashboardlist = new HashSet<Companydashboardlist>();
            Employee = new HashSet<Employee>();
            Companydashboard = new HashSet<Companydashboard>();
        }

        public int Id { get; set; }
        public string Address { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public float? Latitude { get; set; }
        public string LogoUrl { get; set; }
        public float? Longitude { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string WebSite { get; set; }
        public virtual ICollection<Branch> Branch { get; set; }
        public virtual ICollection<Companydashboard> Companydashboard { get; set; }
        //public virtual ICollection<Companydashboardlist> Companydashboardlist { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
        public virtual ICollection<Email> Email { get; set; }
    }
}
