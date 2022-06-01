using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class EmailSettingTemplate
    {      
        public string SMTPAddress { get; set; }
        public int? PortNumber { get; set; }
        //public bool? EnableSSL { get; set; }
        public bool EnableSSL { get; set; }
        public bool EnableTLS { get; set; }
        public string EmailFrom { get; set; }
        public string Password { get; set; }
        public bool? Enable { get; set; }
        public int? ResendInterval { get; set; }
    }
}
