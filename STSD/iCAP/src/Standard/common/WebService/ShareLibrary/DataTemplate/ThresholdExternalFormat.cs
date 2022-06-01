using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class ThresholdExternalFormat
    {
        public class SettingDividerTemplate
        {
            public ThresholdTemplate Threshold { get; set; }
            public string Unit { get; set; }
        }
        public class ThresholdTemplate
        {
            public class Obj
            {
                public string Name { get; set; }
            }
            public Obj External { get; set; }
        }
        public string[] Label { get; set; }
        public int Func { get; set; }
        public SettingDividerTemplate Divider { get; set; }
    }
}
