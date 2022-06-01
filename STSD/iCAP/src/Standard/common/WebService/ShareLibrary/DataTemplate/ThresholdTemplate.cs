using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class ThresholdTemplate
    {
        public class WebSetting
        {
            public ThresholdSetting Setting { get; set; }
            public object[] PermissionRecipientList { get; set; }
            public object[] EmployeeRecipientList { get; set; }
            public object[] ExternalRecipientList { get; set; }
        }

        public class GroupSetting
        {
            public int Id { get; set; }
            public SelectOptionTemplate[] Selected { get; set; }
            public bool Enable { get; set; }
            public SelectOptionTemplate[] Unselected { get; set; }
        }

        public class ThresholdSetting
        {
            public int Id { get; set; }
            public int DataId { get; set; }
            public int? DenominatorId { get; set; }
            public string Value { get; set; }
            public bool DeletedFlag { get; set; }
            public int Action { get; set; }
            public int Func { get; set; }
            public string Name { get; set; }
            public byte Mode { get; set; }
        }

        public ThresholdSetting Setting { get; set; }
        public int[] PermissionIdList { get; set; }
        public int[] EmployeeIdList { get; set; }
        public int[] ExternalRecipientIdList { get; set; }
        public string[] ExternalRecipientList { get; set; }
    }
}
