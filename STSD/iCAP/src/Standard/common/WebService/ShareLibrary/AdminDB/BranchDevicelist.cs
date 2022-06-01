using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public class BranchDeviceList
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int BranchId { get; set; }
        public int DeviceId { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Device Device { get; set; }
    }
}
