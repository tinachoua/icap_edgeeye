using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class SettingItemTemplate
    {
        public class SettingDividerTemplate
        {
            public double[] Percentage { get; set; }
            public int DenominatorId { get; set; }
            public bool[] Boolean { get; set; }
            public double[] Number { get; set; }
        }

        public string[] Label { get; set; }
        public int Func { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
        public int MapIndex { get; set; } // 1: Africa 2: Asia 3: Europe 4: North America 5:
        public string FilePath { get; set; }
        public SettingDividerTemplate Divider { get; set; }
    }
}
