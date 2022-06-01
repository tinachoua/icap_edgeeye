using System;
using System.Collections.Generic;

namespace ShareLibrary.AdminDB
{
    public partial class Device
    {
        public Device()
        {
            Devicecertificate = new HashSet<Devicecertificate>();
            Devicedatalist = new HashSet<Devicedatalist>();
            BranchDeviceList = new HashSet<BranchDeviceList>();
            MarkerDevicelist = new HashSet<MarkerDevicelist>();
        }

        public int Id { get; set; }
        public string Alias { get; set; }       
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public int DeviceClassId { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public float? Latitude { get; set; }
        public float? Longitude { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public int? OwnerId { get; set; }
        public int UploadInterval { get; set; }
        public virtual ICollection<Devicecertificate> Devicecertificate { get; set; }
        public virtual ICollection<Devicedatalist> Devicedatalist { get; set; }
        public virtual ICollection<BranchDeviceList> BranchDeviceList { get; set; }       
        public virtual Deviceclass DeviceClass { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<MarkerDevicelist> MarkerDevicelist { get; set; }
    }
}
