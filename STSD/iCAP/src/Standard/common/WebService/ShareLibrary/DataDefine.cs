using System;
using System.Collections.Generic;
using System.Text;

namespace ShareLibrary
{
    static public class DataDefine
    {
        public enum DataId
        {
            //Chart
            CHART_BAR = 1,
            CHART_DOUGHNUT = 2,
            CHART_GAUGE = 3,
            CHART_Line = 4,
            CHART_GoogleMap = 5,
            CHART_PIE = 6,
            CHART_SCATTER = 7,
            CHART_TEXT = 8,
            CHART_VECTORMAP = 9,
            CHART_OPENSTREETMAP = 10,
            CHART_GAODEMAP = 11,
            CHART_CUSTOMIZEDMAP = 12,
            //Data
            DATA_DEVICELOCATION = 5
        }
        public enum Width
        {
            SIZE_1X1 = 0b00001,
            SIZE_1X2 = 0b00010,
            SIZE_1X3 = 0b00100,
            SIZE_2X2 = 0b01000,
            SIZE_3X3 = 0b10000,
        }

        public enum DataCalculateFunc
        {
            None = 1,
            Percentage = 2,
            Boolean = 3,
            Numerical = 4,
            Map = 5,
            VectorMap = 6,
            CustomizedMap = 7
        }

        public enum DeviceState
        {
            Offline = 0,
            Online = 1,
            Normal = 2,
            Warning = 3
        }

        public enum Redis
        {
            Token = 0,
            Device_Status = 1
        }
        public enum PermissionFlag
        {
            DashboardSetting = 0b0000000001,
            ProfileSetting =   0b0000000010,
            DeviceSetting =    0b0000000100,
            ThresholdSetting = 0b0000001000,
            UserSetting =      0b0000010000,
            MailSetting =      0b0000100000,
            WidgetSetting =    0b0001000000,
            GroupSetting =     0b0010000000,
            CustomizedMap =    0b0100000000,
            RawDataSetting =   0b1000000000,
        }
        public enum Permission
        {
            //Level_Super = 0,
            Level_Admin = 2,
            Level_Guest = 1,
            //Id_Spuer = 1,
            Id_Admin = 2,
            Id_Guest = 1,
        }

        public class PermissionName
        {
            public static string AdminName { get; } = "Admin";
            public static string GuestName { get; } = "Guest";
        }

        public enum Employee
        {
            Id_Admin = 2,
            Id_Guest = 1,
        }
    }
}
