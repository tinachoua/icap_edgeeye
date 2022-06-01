using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public class Threshold
    {
        public Threshold()
        {
            ThresholdBranchList = new HashSet<ThresholdBranchList>();
            ThresholdInternalRecipientList = new HashSet<ThresholdEmployeeList>();
            ThresholdExternalRecipientList = new HashSet<ThresholdExternalRecipientList>();
            ThresholdPermissionList = new HashSet<ThresholdPermissionList>();
            Widget = new HashSet<Widget>();
        }
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Name { get; set; }
        public int DataId { get; set; }
        public int? DenominatorId { get; set; }
        public string Value { get; set; }
        public int Func { get; set; }
        public bool Enable { get; set; }
        public int Action { get; set; } // 0: email 1:screenshot
        public byte Mode { get; set; } // 0: normal 1:ratio
        public virtual Data Data { get; set; }
        public virtual ICollection<ThresholdBranchList> ThresholdBranchList { get; set; }
        public virtual ICollection<ThresholdEmployeeList> ThresholdInternalRecipientList { get; set; }
        public virtual ICollection<ThresholdExternalRecipientList> ThresholdExternalRecipientList { get; set; }
        public virtual ICollection<ThresholdPermissionList> ThresholdPermissionList { get; set; }
        public virtual ICollection<Widget> Widget { get; set; }
    }
}
