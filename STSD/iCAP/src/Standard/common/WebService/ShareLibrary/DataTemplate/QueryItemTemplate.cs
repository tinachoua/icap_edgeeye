using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class QueryItemTemplate
    {
        public Type ItemType { get; set; }
        public string Name { get; set; }
        public QueryItemTemplate NextObj { get; set; }
    }
}
