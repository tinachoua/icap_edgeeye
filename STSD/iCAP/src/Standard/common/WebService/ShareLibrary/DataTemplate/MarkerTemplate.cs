using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class MarkerTemplate
    {
        public int MapId { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public int[] Devices { get; set; }
        public string Name { get; set; }
    }
}