using MockDataCreate.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace MockDataCreate.Models
{
    public class DeviceRange
    {
        public int MinX { get; set; }
        public int MinY { get; set; }
        public int MaxX { get; set; }
        public int MaxY { get; set; }
        public DeviceRange GetTWSmallRange()
        {
            return new DeviceRange()
            {
                MinX = RangeDefine.MIN_VALUE_TW_X,
                MinY = RangeDefine.MIN_VALUE_TW_Y,
                MaxX = RangeDefine.SMALL_MAX_VALUE_TW_X,
                MaxY = RangeDefine.SMALL_MAX_VALUE_TW_Y,
            };
        }
        public DeviceRange GetTWLargeRange()
        {
            return new DeviceRange()
            {
                MinX = RangeDefine.MIN_VALUE_TW_X,
                MinY = RangeDefine.MIN_VALUE_TW_Y,
                MaxX = RangeDefine.LARGE_MAX_VALUE_TW_X,
                MaxY = RangeDefine.LARGE_MAX_VALUE_TW_Y,
            };
        }
        public DeviceRange GetUSASmallRange()
        {
            return new DeviceRange()
            {
                MinX = RangeDefine.MIN_VALUE_USA_X,
                MinY = RangeDefine.MIN_VALUE_USA_Y,
                MaxX = RangeDefine.SMALL_MAX_VALUE_USA_X,
                MaxY = RangeDefine.SMALL_MAX_VALUE_USA_Y,
            };
        }
        public DeviceRange GetUSALargeRange()
        {
            return new DeviceRange()
            {
                MinX = RangeDefine.MIN_VALUE_USA_X,
                MinY = RangeDefine.MIN_VALUE_USA_Y,
                MaxX = RangeDefine.LARGE_MAX_VALUE_USA_X,
                MaxY = RangeDefine.LARGE_MAX_VALUE_USA_Y,
            };
        }
        public DeviceRange GetJPSmallRange()
        {
            return new DeviceRange()
            {
                MinX = RangeDefine.MIN_VALUE_JP_X,
                MinY = RangeDefine.MIN_VALUE_JP_Y,
                MaxX = RangeDefine.SMALL_MAX_VALUE_JP_X,
                MaxY = RangeDefine.SMALL_MAX_VALUE_JP_Y,
            };
        }
        public DeviceRange GetJPLargeRange()
        {
            return new DeviceRange()
            {
                MinX = RangeDefine.MIN_VALUE_JP_X,
                MinY = RangeDefine.MIN_VALUE_JP_Y,
                MaxX = RangeDefine.LARGE_MAX_VALUE_JP_X,
                MaxY = RangeDefine.LARGE_MAX_VALUE_JP_Y,
            };
        }
        public DeviceRange GetNLSmallRange()
        {
            return new DeviceRange()
            {
                MinX = RangeDefine.MIN_VALUE_NL_X,
                MinY = RangeDefine.MIN_VALUE_NL_Y,
                MaxX = RangeDefine.SMALL_MAX_VALUE_NL_X,
                MaxY = RangeDefine.SMALL_MAX_VALUE_NL_Y,
            };
        }
        public DeviceRange GetNLLargeRange()
        {
            return new DeviceRange()
            {
                MinX = RangeDefine.MIN_VALUE_NL_X,
                MinY = RangeDefine.MIN_VALUE_NL_Y,
                MaxX = RangeDefine.LARGE_MAX_VALUE_NL_X,
                MaxY = RangeDefine.LARGE_MAX_VALUE_NL_Y,
            };
        }
        public DeviceRange GetCNSmallRange()
        {
            return new DeviceRange()
            {
                MinX = RangeDefine.MIN_VALUE_CN_X,
                MinY = RangeDefine.MIN_VALUE_CN_Y,
                MaxX = RangeDefine.SMALL_MAX_VALUE_CN_X,
                MaxY = RangeDefine.SMALL_MAX_VALUE_CN_Y,
            };
        }
        public DeviceRange GetCNLargeRange()
        {
            return new DeviceRange()
            {
                MinX = RangeDefine.MIN_VALUE_CN_X,
                MinY = RangeDefine.MIN_VALUE_CN_Y,
                MaxX = RangeDefine.LARGE_MAX_VALUE_CN_X,
                MaxY = RangeDefine.LARGE_MAX_VALUE_CN_Y,
            };
        }
    }
}