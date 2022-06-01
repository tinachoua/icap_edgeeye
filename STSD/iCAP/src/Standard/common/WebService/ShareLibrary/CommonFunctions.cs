using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using ShareLibrary.Interface;
using System.Linq;

namespace ShareLibrary
{
    public class CommonFunctions
    {

        public static string epoch2string(int epoch)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch).ToString("yyyy/MM/dd hh:mm");
        }

        public static string GetTime24(int epoch)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch).ToString("yyyy/MM/dd HH:mm:ss");
        }

        public static int GetCurrentEpoch()
        {
            return Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
        }

        public static string GetBehivorString(int Func, int Index, bool IsSequential)
        {
            if (Func == 1)
            {
                if (!IsSequential)
                {
                    switch (Index)
                    {
                        case 0:
                            return "64.x K";
                        case 1:
                            return "32.x K";
                        case 2:
                            return "16.x K";
                        case 3:
                            return "8.x K";
                        case 4:
                            return "4.x K";
                    }
                }
                else
                {
                    switch (Index)
                    {
                        case 0:
                            return "8.x M";
                        case 1:
                            return "4.x M";
                        case 2:
                            return "1.x M";
                        case 3:
                            return "128.x K";
                        case 4:
                            return "64.x K";
                        case 5:
                            return "32.x K";
                    }
                }
            }
            else
            {
                switch (Index)
                {
                    case 0:
                        return "128.x K";
                    case 1:
                        return "64.x K";
                    case 2:
                        return "32.x K";
                    case 3:
                        return "16.x K";
                    case 4:
                        return "8.x K";
                    case 5:
                        return "4.x K";
                    case 6:
                        return "0.x K";
                }
            }
            return "";
        }

        public static object GetData(BsonValue bvalue)
        {
            if (bvalue.IsNumeric)
            {
                if (bvalue.IsInt32)
                {
                    return bvalue.AsInt32;
                }
                else if (bvalue.IsInt64)
                {
                    return bvalue.AsInt64;
                }
                else if (bvalue.IsDouble)
                {
                    return bvalue.AsDouble;
                }
            }
            return 0;
        }

        public static bool IsBase64String(string s)
        {
            s = s.Trim();
            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);

        }

        public static string CreateRandomPassword(int passwordLength)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@-=";
            char[] chars = new char[passwordLength];
            Random rd = new Random();

            for (int i = 0; i < passwordLength; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }

            return new string(chars);
        }
    }
}
