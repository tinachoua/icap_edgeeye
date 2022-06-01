using System;
using System.Collections.Generic;

namespace ShareLibrary.AdminDB
{
    public partial class Employee
    {
        public Employee()
        {
            Branchroles = new HashSet<Branchroles>();
            Device = new HashSet<Device>();
            ThresholdInternalRecipientList = new HashSet<ThresholdEmployeeList>();
        }
        public int Id { get; set; }
        public int PermissionId { get; set; }
        public int CompanyId { get; set; }
        public int MapId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public string Email { get; set; }
        public string EmployeeNumber { get; set; }
        public string FirstName { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string LastName { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }

        //public int Notification { get; set; }
        // first bit : email tag
        // second bit : phone message
        // third bit : line bot
        public virtual ICollection<Branchroles> Branchroles { get; set; }
        public virtual ICollection<Device> Device { get; set; }
        public virtual ICollection<ThresholdEmployeeList> ThresholdInternalRecipientList { get; set; }
        public virtual Company Company { get; set; }
        public virtual Chart Chart { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
