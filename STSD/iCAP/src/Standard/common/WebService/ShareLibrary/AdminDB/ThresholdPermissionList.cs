using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public class ThresholdPermissionList
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int ThresholdId { get; set; }
        public int PermissionId { get; set; }
        public virtual Permission Permission { get; set; }
        public virtual Threshold Threshold { get; set; }
    }
}
