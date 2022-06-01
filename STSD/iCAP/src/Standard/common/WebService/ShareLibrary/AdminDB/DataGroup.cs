using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareLibrary.AdminDB
{
    public partial class DataGroup
    {
        public DataGroup()
        {
            Data = new HashSet<Data>();
        }

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Data> Data { get; set; }
    }
}
