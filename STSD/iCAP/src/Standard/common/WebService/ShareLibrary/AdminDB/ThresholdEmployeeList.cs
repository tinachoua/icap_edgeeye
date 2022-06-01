using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public class ThresholdEmployeeList
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int ThresholdId { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Threshold Threshold { get; set; }
    }
}
