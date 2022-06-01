using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public partial class Branch
    {
        public Branch()
        {
            Branchdashboardlist = new HashSet<Branchdashboardlist>();
            Branchroles = new HashSet<Branchroles>();
            WidgetBranchList = new HashSet<WidgetBranchList>();
            BranchDeviceList = new HashSet<BranchDeviceList>();
            ThresholdBranchList = new HashSet<ThresholdBranchList>();
        }
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public string Description { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public int TimeZone { get; set; }
        //public int Notification { get; set; } // first bit : event   second bit: email

        public virtual ICollection<Branchdashboardlist> Branchdashboardlist { get; set; }
        public virtual ICollection<Branchroles> Branchroles { get; set; }      
        public virtual Company Company { get; set; }      
        public virtual ICollection<WidgetBranchList> WidgetBranchList { get; set; }
        public virtual ICollection<BranchDeviceList> BranchDeviceList { get; set; }
        public virtual ICollection<ThresholdBranchList> ThresholdBranchList { get; set; }


    }
}
