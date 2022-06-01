using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public class WidgetBranchList
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int BranchId { get; set; }
        public int WidgetId { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Widget Widget { get; set; }
    }
}
