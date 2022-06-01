using System;
using System.Collections.Generic;

namespace ShareLibrary.AdminDB
{
    public partial class Widget
    {
        public Widget()
        {
            Branchdashboardelement = new HashSet<Branchdashboardelement>();
            Companydashboardelement = new HashSet<Companydashboardelement>();
            WidgetBranchList = new HashSet<WidgetBranchList>();
            Marker = new HashSet<Marker>();
        }

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int DataCount { get; set; }
        public int? DataId { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public byte Width { get; set; } // 1: 1x1, 2: 1x2, 3: 1x3, 4: 2x3, 5: 3x3
        public string Name { get; set; }
        public string SettingStr { get; set; }
        public int? ChartId { get; set; }
        public int? ThresholdId { get; set; }

        public virtual ICollection<Branchdashboardelement> Branchdashboardelement { get; set; }
        public virtual ICollection<Companydashboardelement> Companydashboardelement { get; set; }
        public virtual ICollection<WidgetBranchList> WidgetBranchList { get; set; }
        public virtual Threshold Threshold { get; set; }
        public virtual Data Data { get; set; }
        public virtual Chart Chart { get; set; }
        public virtual ICollection<Marker> Marker { get; set; }
    }
}
