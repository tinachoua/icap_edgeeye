using System;
using System.Collections.Generic;

namespace ShareLibrary.AdminDB
{
    public partial class Devicecertificate
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public int DeviceId { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string Password { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Thumbprint { get; set; }

        public virtual Device Device { get; set; }
    }
}
