using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public class ThresholdBranchList
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        //public bool EventFlag { get; set; }
        //public bool EmailFlag { get; set; }
        public int ThresholdId { get; set; }
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Threshold Threshold { get; set; }
    }
}
