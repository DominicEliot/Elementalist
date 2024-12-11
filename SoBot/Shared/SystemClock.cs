using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SorceryBot.Shared;
public static class SystemClock
{
    private static DateTimeOffset? _customDate;
    private static TimeZoneInfo? _timezone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");

    public static DateTimeOffset Now
    {
        get
        {
            if (_customDate.HasValue)
            {
                return _customDate.Value;
            }

            if (_timezone != null)
            {
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timezone);
            }

            return DateTimeOffset.Now;
        }
    }

    public static DateOnly DateOnlyNow { get => DateOnly.FromDateTime(Now.DateTime); }

    public static void Set(DateTimeOffset customDate) => _customDate = customDate;

    public static void Reset()
    {
        _customDate = null;
    }
}