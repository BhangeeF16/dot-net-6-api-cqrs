using System.Globalization;

namespace Domain.Common.Extensions;

public static class DateTimeExtensions
{
    public static DateTime GetNearestDate(this DayOfWeek desiredDay, DateTime? targetDate = null)
    {
        targetDate = targetDate != null && targetDate.HasValue ? targetDate : DateTime.UtcNow.Date;
        int diff = (7 + (targetDate.Value.DayOfWeek - desiredDay)) % 7;
        return targetDate.Value.Date.AddDays(-diff);
    }
    public static TimeSpan ToTimeSpan(this string HexaDecimalTimeStamp)
    {
        var timestamp = HexaDecimalTimeStamp.ToInt();
        return new TimeSpan(timestamp);
    }
    public static DateTime? ToDateTime(this string dateTime)
    {
        if (string.IsNullOrEmpty(dateTime))
        {
            return null;
        }

        return DateTimeOffset.ParseExact(dateTime, new string[] { "MMM dd, yyyy", "dd/MM/yyyy", "d/M/yyyy", "dd-MM-yyyy", "d-M-yyyy", "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-dd" }, CultureInfo.InvariantCulture).DateTime;
        //try
        //{
        //}
        //catch (Exception ex)
        //{
        //char formatChar = '-';
        //if (dateTime.Contains('-'))
        //{
        //    formatChar = '-';
        //}

        //if (dateTime.Contains('/'))
        //{
        //    formatChar = '/';
        //}

        //string[] parts = dateTime.Split(formatChar);
        //var month = parts[0].PadLeft(2, '0');
        //var day = parts[1].PadLeft(2, '0');
        //var year = parts[2];
        //string properFormattedDate = $"{month}{formatChar}{day}{formatChar}{year}";

        //return DateTimeOffset.ParseExact(properFormattedDate, new string[] { "MMM dd, yyyy", "dd/MM/yyyy", "dd-MM-yyyy", "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-dd" }, CultureInfo.InvariantCulture).DateTime;
        //}
    }
    public static DateTime? ToDateTime(this long? timeStamp) => timeStamp != null && timeStamp > 0 ? new DateTime(1970, 1, 1).AddSeconds(timeStamp.Value).ToLocalTime() : null;
    public static DateTime? ToDateTime(this string dateTime, bool IsHexaDecimal = false)
    {
        if (!IsHexaDecimal)
        {
            return dateTime.ToDateTime();
        }
        else
        {
            var timestamp = dateTime.ToInt();
            var hexDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return hexDateTime.AddSeconds(timestamp).ToLocalTime();
        }
    }
    public static DateTime UtcToTimeZone(this DateTime UTCDateTime, string TimeZoneID = "AUS Eastern Standard Time")
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID);
        var timeDiff = tz.GetUtcOffset(DateTime.UtcNow);
        UTCDateTime = UTCDateTime.AddHours(-1 * timeDiff.TotalHours);
        return UTCDateTime;
    }
    public static TimeSpan GetOffsetOfTimeZone(string TimeZoneID = "AUS Eastern Standard Time")
    {
        var tz = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneID);
        return tz.GetUtcOffset(DateTime.UtcNow);
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
