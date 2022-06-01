using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary.DataTemplate
{
    public class CPUCoreTemplate
    {
        public int[] CoreFreq { get; set; }
        public double[] CoreUsage { get; set; }
        public int[] CoreTemp { get; set; }
        public int[] CoreVoltage { get; set; }
    }
}
