using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class DeviceProfileTemplate
    {
        public int Id { get; set; }
        public string DevName { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string PhotoURL { get; set; }
        public string OwnerName { get; set; }
        public string Email { get; set; }
        public int[] BranchId { get; set; }
        public string[] BranchName { get; set; }
    }

    class DPCompare : IEqualityComparer<DeviceProfileTemplate>
    {
        public bool Equals(DeviceProfileTemplate x, DeviceProfileTemplate y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null | y == null)
                return false;
            else if (x.DevName == y.DevName)
                return true;
            else
                return false;
        }

        public int GetHashCode(DeviceProfileTemplate obj)
        {
            return obj.DevName.GetHashCode();
        }
    }


}
