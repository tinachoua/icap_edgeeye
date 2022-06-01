using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class DeviceTemplate
    {
        public string DevName { get; set; }
        public string Name { get; set; }
        public string StorageSN { get; set; }
        public string Value { get; set; }
        public string OwnerName { get; set; }
        public string Email { get; set; }
        public int[] BranchId { get; set; }
        public string[] BranchName { get; set; }
        public string Alias { get; set; }
        public int Time { get; set; }
    }
}
