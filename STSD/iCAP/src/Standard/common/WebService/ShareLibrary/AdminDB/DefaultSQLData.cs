using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShareLibrary.AdminDB
{
    public class DefaultSQLData
    {
        public Widget[] Widget { get; set; }
        public WidgetBranchList[] WidgetBranchElement { get; set; }
        public Companydashboardelement[] DashboardElement { get; set; }
        public Branch[] Branch { get; set; }
        public Companydashboard[] Dashboard { get; set; }
    }
}