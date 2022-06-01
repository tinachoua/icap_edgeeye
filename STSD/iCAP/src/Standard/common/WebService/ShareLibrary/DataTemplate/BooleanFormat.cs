using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class BooleanFormat
    {
        public class SettingDividerTemplate
        {           
            public bool[] Boolean { get; set; }
        }
        public string[] Label { get; set; }
        public int Func { get; set; }
        public SettingDividerTemplate Divider { get; set; }
    }
}
