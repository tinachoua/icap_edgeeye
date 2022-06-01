using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class BranchTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }        
        //public bool EventFlag { get; set; }
        //public bool EmailFlag { get; set; }
        public SelectOptionTemplate[] Selected { get; set; }
    }
}
