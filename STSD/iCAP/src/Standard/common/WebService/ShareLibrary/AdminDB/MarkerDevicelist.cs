using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public class MarkerDevicelist
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int DeviceId { get; set; }
        public string MarkerGuid { get; set; }
        public virtual Marker Marker { get; set; }
        public virtual Device Device { get; set; }
    }
}