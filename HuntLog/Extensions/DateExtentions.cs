using System;
using System.Globalization;

namespace HuntLog.Extensions
{
    public static class DateExtentions
    {
        public static string ToNoString(this DateTime dateTime)
        {
            if (dateTime.Year == DateTime.Now.Year)
            {
                return dateTime.ToString("dd MMM", new CultureInfo("nb-NO"));
            }

            return dateTime.ToString("dd MMM yyyy", new CultureInfo("nb-NO"));
        }
    }
}
