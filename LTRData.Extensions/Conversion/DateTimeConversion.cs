using System;

namespace LTRData.Extensions.Conversion;

/// <summary>
/// </summary>
public static class DateTimeConversion
{
    private static readonly long maxFileTime = DateTime.MaxValue.ToFileTime() + 1;

    private static readonly long minFileTime = 0;

    /// <summary>
    /// Attempts to convert FILETIME value to DateTime value
    /// </summary>
    /// <param name="fileTime">FILETIME value to convert</param>
    /// <returns>Converted DateTime value if conversion was successful, null otherwise</returns>
    public static DateTime? TryFileTimeToDateTime(long fileTime)
    {
        if (fileTime > minFileTime && fileTime < maxFileTime)
        {
            return DateTime.FromFileTime(fileTime);
        }

        return null;
    }

    /// <summary>
    /// Attempts to convert FILETIME value to DateTime value
    /// </summary>
    /// <param name="fileTime">FILETIME value to convert</param>
    /// <returns>Converted DateTime value if conversion was successful, null otherwise</returns>
    public static DateTime? TryFileTimeToDateTimeUtc(long fileTime)
    {
        if (fileTime > minFileTime && fileTime < maxFileTime)
        {
            return DateTime.FromFileTimeUtc(fileTime);
        }

        return null;
    }

    /// <summary>
    /// Attempts to convert FILETIME value to DateTimeOffset value
    /// </summary>
    /// <param name="fileTime">FILETIME value to convert</param>
    /// <returns>Converted DateTimeOffset value if conversion was successful, null otherwise</returns>
    public static DateTimeOffset? TryFileTimeToDateTimeOffset(long fileTime)
    {
        if (fileTime > minFileTime && fileTime < maxFileTime)
        {
            return DateTimeOffset.FromFileTime(fileTime);
        }

        return null;
    }
}
