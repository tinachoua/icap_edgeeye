using System;
using System.Collections.Generic;

namespace ShareLibrary.AdminDB
{
    public partial class Branchdashboardelement
    {
        public int Id { get; set; }
        public int Column { get; set; }
        public DateTime CreatedDate { get; set; }
        public int DashboardId { get; set; }
        public bool DeletedFlag { get; set; }
        public int Height { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int Row { get; set; }
        public int WidgetId { get; set; }

        public virtual Branchdashboard Dashboard { get; set; }
        public virtual Widget Widget { get; set; }
    }
}
