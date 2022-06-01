using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary
{
    public class LogAgent
    {
        public struct LogFileFormat
        {
            /// <summary>
            /// The command transmit direction :
            ///     True as response
            ///     False as receive
            /// </summary>
            public bool Direction { get; set; }
            public string Name { get; set; }
            public string Method { get; set; }
            public string URL { get; set; }
            public int ResponseCode { get; set; }
            public string Remark { get; set; }
        }

        public class socketLogFormat
        {
            public string Type { get; set; }
            public string Msg { get; set; }
            public string Ip { get; set; }
            public int Port { get; set; }
            public string Remark { get; set; }
        }

        public static void WriteToLog(LogFileFormat lf)
        {
#if DEBUG
            Console.WriteLine("{0},{1},{2},{3},{4:000},{5}",
                lf.Direction ? "Response" : "Receive",
                lf.Name,
                lf.Method,
                lf.URL,
                lf.ResponseCode,
                lf.Remark);
#endif
        }
        public static void WriteToLog(socketLogFormat lf)
        {
#if DEBUG
            Console.WriteLine("{0},{1},{2},{3},{4}",
                lf.Type,
                lf.Ip,
                lf.Port,
                lf.Msg,
                lf.Remark);
#endif
        }
    }
}
