using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;


namespace ShareLibrary.DataTemplate
{
    public class WidgetClickPayload
    {
        public int DashboardId { get; set; }
        public int WidgetId { get; set; }
        public int LabelIndex { get; set; }
        public int DataStart { get; set; }
        public int DataLength { get; set; }
    }
}
