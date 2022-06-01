using System;
using System.Collections.Generic;

namespace ShareLibrary.AdminDB
{
    public partial class Companydashboardelement
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public int DashboardId { get; set; }
        public bool DeletedFlag { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int WidgetId { get; set; }
        public int IteratorIndex { get; set; }

        public virtual Companydashboard Dashboard { get; set; }
        public virtual Widget Widget { get; set; }
    }
}
