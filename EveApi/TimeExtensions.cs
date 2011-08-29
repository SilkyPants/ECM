using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace EveApi
{
    public static class TimeExtensions
    {
        /// <summary>
        /// Converts a UTC DateTime to the API date/time string.
        /// </summary>
        /// <param name="timeUTC"></param>
        /// <returns></returns>
        public static string DateTimeToTimeString(this DateTime timeUTC)
        {
            // timeUTC = yyyy-MM-dd HH:mm:ss
            string result = String.Format(CultureInfo.CurrentCulture, "{0:d4}-{1:d2}-{2:d2} {3:d2}:{4:d2}:{5:d2}",
                                          timeUTC.Year,
                                          timeUTC.Month,
                                          timeUTC.Day,
                                          timeUTC.Hour,
                                          timeUTC.Minute,
                                          timeUTC.Second);
            return result;
        }

        /// <summary>
        /// Converts an API date/time string to a UTC DateTime.
        /// </summary>
        /// <param name="timeUTC"></param>
        /// <returns></returns>
        public static DateTime TimeStringToDateTime(this String timeUTC)
        {
            DateTime dt = DateTime.MinValue;

            // timeUTC = yyyy-MM-dd HH:mm:ss
            if (String.IsNullOrEmpty(timeUTC))
                return dt;

            DateTime.TryParse(timeUTC, CultureInfo.CurrentCulture, DateTimeStyles.AdjustToUniversal, out dt);

            return dt;
        }
    }
}
