using System;

public class DateTimeHelper
{
    public static DateTime GetDateTimeByUnixTimestamp(int unixTimestamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime().AddSeconds(unixTimestamp);
    }

    public static int GetSecondsLeftForTheEndOfTheDay(DateTime dateTime)
    {
        var startNextDayDate = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59).AddSeconds(1);
        var restTime = startNextDayDate - dateTime;
        return (int)restTime.TotalSeconds;
    }

    public static bool IsNextDay(DateTime previousDate, DateTime targetDate)
    {
        if (targetDate.DayOfYear == previousDate.DayOfYear + 1
            && targetDate.Year == previousDate.Year)
        {
            return true;
        }
        else
        {
            var lastDayOfPrevYear = new DateTime(previousDate.Year, 12, 31);
            return targetDate.DayOfYear == 1
                && lastDayOfPrevYear.DayOfYear == previousDate.DayOfYear
                && targetDate.Year == previousDate.Year + 1;
        }
    }

    public static bool IsNextDay(int prevTimestamp, int targetTimestamp)
    {
        return IsNextDay(GetDateTimeByUnixTimestamp(prevTimestamp), GetDateTimeByUnixTimestamp(targetTimestamp));
    }

    public static bool IsSameDays(int unixTimestamp1, int unixTimestamp2)
    {
        var date1 = GetDateTimeByUnixTimestamp(unixTimestamp1);
        var date2 = GetDateTimeByUnixTimestamp(unixTimestamp2);
        return IsSameDays(date1, date2);
    }

    public static bool IsSameDays(DateTime date1, DateTime date2)
    {
        return date1.DayOfYear == date2.DayOfYear && date1.Year == date2.Year;
    }

    public static bool IsNewYearsEve()
    {
        var now = DateTime.Now;
        return (now.Month == 12 && now.Day >= 15) || (now.Month == 1 && now.Day < 15);
    }

    public static bool IsWinter()
    {
        var now = DateTime.Now;
        return now.Month >= 12 || now.Month <= 2;
    }
}
