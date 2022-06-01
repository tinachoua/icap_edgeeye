using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public class Marker
    {
        public Marker()
        {
            MarkerDevicelist = new HashSet<MarkerDevicelist>();
        }
        [Key]
        public string PK_Guid { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Name { get; set; }
        public int CustomizedMapId { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public virtual Widget Widget { get; set; }
        public virtual ICollection<MarkerDevicelist> MarkerDevicelist { get; set; }
    }
}