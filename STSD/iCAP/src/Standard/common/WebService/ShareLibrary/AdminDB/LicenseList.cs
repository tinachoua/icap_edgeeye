using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareLibrary.AdminDB
{
    public partial class LicenseList
    {
        public LicenseList()
        {
        }

        public int Id { get; set; }
        public int DeviceCount { get; set; }
        public string Key { get; set; }
        public DateTime CreatedDate { get; set; }
        
    }
}
