using System;
using System.Collections.Generic;

namespace ShareLibrary.AdminDB
{
    public partial class Deviceclass
    {
        public Deviceclass()
        {
            Device = new HashSet<Device>();
        }

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public string Description { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Device> Device { get; set; }
    }
}
