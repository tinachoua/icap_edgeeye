using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class PercentageFormat
    {
        public class SettingDividerTemplate
        {       
            public double[] Percentage { get; set; }
            public int DenominatorId { get; set; }                    
            public string DataName { get; set; }
        }
        public string[] Label { get; set; }
        public int Func { get; set; }
        public SettingDividerTemplate Divider { get; set; }
    }
}
