using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class BranchSettingInfo
    {
        public BranchTemplate Detail { get; set; }
        public SelectOptionTemplate[] Unselected { get; set; }
    }
}
