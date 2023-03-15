using System.Globalization;

namespace Domain.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static TimeSpan ToTimeSpan(this string HexaDecimalTimeStamp)
        {
            var timestamp = HexaDecimalTimeStamp.ToInt();
            return new TimeSpan(timestamp);
        }
        public static DateTime? ToDateTime(this string dateTime, bool IsHexaDecimal = false)
        {
            if (!IsHexaDecimal)
            {
                if (DateTimeOffset.TryParse(dateTime, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset result))
                {
                    return result.DateTime;
                }
            }
            else
            {
                var timestamp = dateTime.ToInt();
                var hexDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                return hexDateTime.AddSeconds(timestamp).ToLocalTime();
            }
            return null;
        }
        public static DateTime UtcToTimeZone(this DateTime dateTime, bool isSend = true, string TimeZoneID = "AUS Eastern Standard Time")
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID);
            var timeDiff = tz.GetUtcOffset(DateTime.UtcNow);
            dateTime = dateTime.AddHours((isSend ? -1 : 1) * timeDiff.TotalHours);
            return dateTime;
        }
        public static DateTime UnixTimeStampToDateTime(this double unixTimeStamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        public static long GetDifferenceInMonths(DateTime StartDate, DateTime EndDate)
        {
            var days = from day in StartDate.DaysInRangeUntil(EndDate)
                       let start = Max(day.AddHours(7), StartDate)
                       let end = Min(day.AddHours(19), EndDate)
                       select (end - start).TotalDays;

            return (long)Math.Round(days.Sum() / 30);
        }
        public static IEnumerable<DateTime> DaysInRangeUntil(this DateTime start, DateTime end)
        {
            return Enumerable.Range(0, 1 + (int)(end.Date - start.Date).TotalDays)
                             .Select(dt => start.Date.AddDays(dt));
        }
        public static bool IsWeekendDay(this DateTime dt)
        {
            return dt.DayOfWeek == DayOfWeek.Saturday
                || dt.DayOfWeek == DayOfWeek.Sunday;
        }
        public static DateTime Max(DateTime start, DateTime end)
        {
            return new DateTime(Math.Max(start.Ticks, end.Ticks));
        }
        public static DateTime Min(DateTime start, DateTime end)
        {
            return new DateTime(Math.Min(start.Ticks, end.Ticks));
        }

        public static int ToSeconds(this int value, TimeType valueType)
        {
            switch (valueType)
            {
                case TimeType.Minutes:
                    return value * 60;
                case TimeType.Hours:
                    return value * 60 * 60;
                case TimeType.Days:
                    return value * 24 * 60 * 60;
                default:
                    return value;
            }
        }
        public static int ToMinutes(this int value, TimeType valueType)
        {
            switch (valueType)
            {
                case TimeType.Seconds:
                    return value / 60;
                case TimeType.Hours:
                    return value * 60;
                case TimeType.Days:
                    return value * 24 * 60;
                default:
                    return value;
            }
        }
        public static int ToDays(this int value, TimeType valueType)
        {
            switch (valueType)
            {
                case TimeType.Seconds:
                    return value / 60 / 60 / 24;
                case TimeType.Minutes:
                    return value / 60 / 24;
                case TimeType.Hours:
                    return value / 24;
                default:
                    return value;
            }
        }
        public static int ToHours(this int value, TimeType valueType)
        {
            switch (valueType)
            {
                case TimeType.Seconds:
                    return value / 60 / 60;
                case TimeType.Minutes:
                    return value / 60;
                case TimeType.Days:
                    return value * 24;
                default:
                    return value;
            }
        }
        public static long ToMinutes(this long value, TimeType valueType)
        {
            switch (valueType)
            {
                case TimeType.Seconds:
                    return value / 60;
                case TimeType.Hours:
                    return value * 60;
                case TimeType.Days:
                    return value * 24 * 60;
                default:
                    return value;
            }
        }
        public static long ToDays(this long value, TimeType valueType)
        {
            switch (valueType)
            {
                case TimeType.Seconds:
                    return value / 60 / 60 / 24;
                case TimeType.Minutes:
                    return value / 60 / 24;
                case TimeType.Hours:
                    return value / 24;
                default:
                    return value;
            }
        }
        public static long ToHours(this long value, TimeType valueType)
        {
            switch (valueType)
            {
                case TimeType.Seconds:
                    return value / 60 / 60;
                case TimeType.Minutes:
                    return value / 60;
                case TimeType.Days:
                    return value * 24;
                default:
                    return value;
            }
        }
    }
    public enum TimeType
    {
        Days = 1,
        Hours = 2,
        Minutes = 3,
        Seconds = 4,
    }
}
