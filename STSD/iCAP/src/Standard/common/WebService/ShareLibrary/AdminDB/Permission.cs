using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public class Permission
    {
        public Permission()
        {
            Employee = new HashSet<Employee>();
            ThresholdPermissionList = new HashSet<ThresholdPermissionList>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int Create { get; set; }
        public int Update { get; set; }
        public int Delete { get; set; }
        public byte Level { get; set; }
        public virtual ICollection<Employee> Employee { get; set; }
        public virtual ICollection<ThresholdPermissionList> ThresholdPermissionList { get; set; }
    }
}
