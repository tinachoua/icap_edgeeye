using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MockDataCreate.Models
{
    public class DeviceCount
    {
        public class Specification
        {
            public bool GPU { get; set; }
            public bool CANBus { get; set; }
            public bool OOB { get; set; }
        }
        public class MockDevice
        {
            public uint Total { get; set; }
            public Specification Spec { get; set; }
            public Location Location { get; set; }
            public DeviceRange DeviceRange { get; set; }
            public int[] BranchIdlist { get; set; }

        }
        public MockDevice[] TW { get; set; } = new MockDevice[] {
            new MockDevice(){ 
                Total = 1,
                Spec = new Specification(){
                    GPU = false,
                    CANBus = true,
                    OOB = true
                },
                Location = new Location(){
                   Longitude = 121.634247, // innodisk location
                   Latitude = 25.058798
                }
            },
            new MockDevice(){
                Total = 10,
                Spec = new Specification(){
                    GPU = true,
                    CANBus = false,
                    OOB = false
                }
            },
            new MockDevice(){
                Total = 9,
                Spec = new Specification(){
                    GPU = false,
                    CANBus = false,
                    OOB = false
                }
            }
        };
        public MockDevice[] USA { get; set; } = new MockDevice[] {
            new MockDevice(){
                Total = 20,
                Spec = new Specification(){
                    GPU = false,
                    CANBus = false,
                    OOB = false
                }
            },
        };
        public MockDevice[] JP { get; set; } = new MockDevice[] {
            new MockDevice(){
                Total = 20,
                Spec = new Specification(){
                    GPU = false,
                    CANBus = false,
                    OOB = false
                }
            },
        };
        public MockDevice[] NL { get; set; } = new MockDevice[] {
            new MockDevice(){
                Total = 20,
                Spec = new Specification(){
                    GPU = false,
                    CANBus = false,
                    OOB = false
                }
            },
        };
        public MockDevice[] CN { get; set; } = new MockDevice[] {
            new MockDevice(){
                Total = 20,
                Spec = new Specification(){
                    GPU = false,
                    CANBus = false,
                    OOB = false
                }
            },
        };
        public uint GetCount(MockDevice[] devices)
        {
            uint count = 0;
            foreach (MockDevice device in devices)
            {
                count += device.Total; 
            }
            return count;
        }
        public uint GetTotal()
        {
            return GetCount(CN) + GetCount(NL) + GetCount(USA) + GetCount(TW) + GetCount(JP);
        }
        public void Init()
        {
            DeviceRange range = new DeviceRange();
            foreach (var mockdevice in TW)
            {
                if (mockdevice.Total > 20)
                {
                    mockdevice.DeviceRange = range.GetTWLargeRange();
                }
                else
                {
                    mockdevice.DeviceRange = range.GetTWSmallRange();
                }
            }

            foreach (var mockdevice in USA)
            {
                if (mockdevice.Total > 20)
                {
                    mockdevice.DeviceRange = range.GetUSALargeRange();
                }
                else
                {
                    mockdevice.DeviceRange = range.GetUSASmallRange();
                }
            }

            foreach (var mockdevice in JP)
            {
                if (mockdevice.Total > 20)
                {
                    mockdevice.DeviceRange = range.GetJPLargeRange();
                }
                else
                {
                    mockdevice.DeviceRange = range.GetJPSmallRange();
                }
            }

            foreach (var mockdevice in CN)
            {
                if (mockdevice.Total > 20)
                {
                    mockdevice.DeviceRange = range.GetCNLargeRange();
                }
                else
                {
                    mockdevice.DeviceRange = range.GetCNSmallRange();
                }
            }

            foreach (var mockdevice in NL)
            {
                if (mockdevice.Total > 20)
                {
                    mockdevice.DeviceRange = range.GetNLLargeRange();
                }
                else
                {
                    mockdevice.DeviceRange = range.GetNLSmallRange();
                }
            }
        }
    }
}