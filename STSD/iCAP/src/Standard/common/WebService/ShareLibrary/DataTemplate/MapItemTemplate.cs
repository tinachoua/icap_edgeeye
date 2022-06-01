using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class MapItemTemplate
    {
        public class Pos
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }
        public int id { get; set; }
        public string name { get; set; }
        public string alias { get; set; }
        public string color { get; set; }
        public string status { get; set; }
        public string owner { get; set; }
        public string detail { get; set; }
        public int time { get; set; }
        public Pos position { get; set; }
    }
}
