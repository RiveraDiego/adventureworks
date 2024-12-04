using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace adventureworks.Utils
{
    public static class TimeZoneHelper
    {
        private static readonly TimeZoneInfo SalvadorTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");

        public static DateTime ConvertToElSalvadorTime(DateTime utcDateTime)
        {
            return TimeZoneInfo.ConvertTime(utcDateTime, SalvadorTimeZone);
        }
    }

}