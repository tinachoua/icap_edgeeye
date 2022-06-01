using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class VectorMapTemplate
    {
        public class Marker
        {
            public class Style
            {
                public int r { get; set; }
                public string fill { get; set; }
            }
            public double[] latLng { get; set; }
            public Style style { get; set; }
        }
        public Marker[] markers { get; set; } 
        //public Location[] location { get; set; }
        public bool[] eventState { get; set; }
        public int[] deviceCount { get; set; }
        public int mapIndex { get; set; }
        public int[] eventCount { get; set; }
    }
}
