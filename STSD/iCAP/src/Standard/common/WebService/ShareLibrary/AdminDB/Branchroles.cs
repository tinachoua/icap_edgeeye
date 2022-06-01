using System;
using System.Collections.Generic;

namespace ShareLibrary.AdminDB
{
    public partial class Branchroles
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public int EmployeeId { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public virtual Branch Branch { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
