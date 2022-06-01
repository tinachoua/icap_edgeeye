using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    //public class DataOption
    //{       
    //    public int DataId { get; set; }
    //    public string DataName { get; set; }        
    //}
    public class NumbericalDataOption
    {   
        public DataOption[] System { get; set; }
        public DataOption[] CPU { get; set; }
        public DataOption[] Memory { get; set; }
        public DataOption[] Storage { get; set; }
        public DataOption[] External { get; set; }
    }



}
