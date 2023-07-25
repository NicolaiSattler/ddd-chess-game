using System.Globalization;

namespace Chess.Web.Extensions;

public static class DateTimeExtensions
{
    public static string GetVerbalTimeDisplay(this DateTime time)
    {
        var timeDifference = DateTime.UtcNow - time;
        var totalSeconds = (int)timeDifference.TotalSeconds;
        var totalMinutes = (int)timeDifference.TotalMinutes;
        var totalHours = (int)timeDifference.TotalHours;
        var totalDays = (int)timeDifference.TotalDays;

        return timeDifference switch
        {
            _ when timeDifference.TotalSeconds < 60 => $"{totalSeconds} seconds ago.",
            _ when timeDifference.TotalMinutes < 60 => $"{totalMinutes} minutes ago.",
            _ when timeDifference.TotalHours < 24 => $"{totalHours} hours ago.",
            _ => $"{totalDays} days ago.",
        };
    }

    public static DateTime ToLocalTimeZone(this DateTime time, string culture)
         => TimeZoneInfo.ConvertTime(time, TimeZoneInfo.Local);
}