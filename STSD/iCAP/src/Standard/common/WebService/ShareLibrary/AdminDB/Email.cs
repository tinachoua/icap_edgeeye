using System;
using System.Collections.Generic;
using System.Text;
using ShareLibrary.AdminDB;

namespace ShareLibrary.AdminDB
{
    public class Email
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string SMTPAddress { get; set; }
        public int? PortNumber { get; set; }
        //public bool? EnableSSL { get; set; }
        public byte Encryption { get; set; } // First bit: SSL, Second Bit: TLS
        public string EmailFrom { get; set; }
        public string Password { get; set; }
        public bool? Enable { get; set; }
        public int? ResendInterval { get; set; }
        public virtual Company Company { get; set; }


    }
}
