using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public class VectorMap
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Name { get; set; }


    }
}
