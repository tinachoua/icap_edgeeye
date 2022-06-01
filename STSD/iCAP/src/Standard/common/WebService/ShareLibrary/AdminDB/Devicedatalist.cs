using System;
using System.Collections.Generic;

namespace ShareLibrary.AdminDB
{
    public partial class Devicedatalist
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int DataId { get; set; }
        public bool DeletedFlag { get; set; }
        public int DeviceId { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public virtual Data Data { get; set; }
        public virtual Device Device { get; set; }
    }
}
