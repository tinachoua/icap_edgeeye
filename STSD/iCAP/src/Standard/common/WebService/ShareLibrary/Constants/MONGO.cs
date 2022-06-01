using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShareLibrary.Contsants
{
    public static class MONGO
    {
        /// <summary>
        /// api collection
        /// </summary>
        public static string KEYS_COLLECTION = "Keys";
        public static string KEY_ATTR = "key";
        public static string UPDATE_TIME_ATTTR = "updateTime";
        public static string GOOGLE_MAP = "googleMap";

        public static string[] API_TYPE = new string[] {
            GOOGLE_MAP
        };
    }
}