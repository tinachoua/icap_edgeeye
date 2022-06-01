using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.Mongo
{
    public static class Schema
    {
        public class Api_key
        {
            public string key { get; set; }
            public DateTime? updateTime { get; set; }
        }
    }
}
