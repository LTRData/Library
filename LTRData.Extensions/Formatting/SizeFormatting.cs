using LTRData.Extensions.Buffers;
using System;
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Collections.Concurrent;
#endif
#if NET6_0_OR_GREATER
using System.Collections.Immutable;
#endif
using System.Collections.Generic;
using System.Globalization;

namespace LTRData.Extensions.Formatting;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0056 // Use index operator

/// <summary>
/// Methods for formatting byte sizes with MB, GB etc suffixes
/// </summary>
public static class SizeFormatting
{
    private static readonly ICollection<KeyValuePair<ulong, string>> Multipliers = new List<KeyValuePair<ulong, string>>
    {
        new(1UL << 60, " EB"),
        new(1UL << 50, " PB"),
        new(1UL << 40, " TB"),
        new(1UL << 30, " GB"),
        new(1UL << 20, " MB"),
        new(1UL << 10, " KB")
    }
#if NET6_0_OR_GREATER
    .ToImmutableList();
#else
    .AsReadOnly();
#endif

    /// <summary>
    /// Formats byte sizes with MB, GB etc suffixes
    /// </summary>
    /// <param name="size">Number of bytes</param>
    /// <returns>Number of bytes formatted as string</returns>
    public static string FormatBytes(ulong size)
    {
        foreach (var multiplier in Multipliers)
        {
            if (size >= multiplier.Key)
            {
                return $"{size / (double)multiplier.Key:0.0}{multiplier.Value}";
            }
        }

        return $"{size} byte";
    }

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
    private static readonly ConcurrentDictionary<int, string> PrecisionFormatStrings = new();

    /// <summary>
    /// Formats byte sizes with MB, GB etc suffixes
    /// </summary>
    /// <param name="size">Number of bytes</param>
    /// <param name="precision">Number of decimals in formatted string</param>
    /// <returns>Number of bytes formatted as string</returns>
    public static string FormatBytes(ulong size, int precision)
    {
        foreach (var multiplier in Multipliers)
        {
            if (size >= multiplier.Key)
            {
                var precisionFormatString =
                    PrecisionFormatStrings.GetOrAdd(precision,
                                                    precision => $"0.{new string('0', precision - 1)}");

                return $"{(size / (double)multiplier.Key).ToString(precisionFormatString)}{multiplier.Value}";
            }
        }

        return $"{size} byte";
    }

    /// <summary>
    /// Formats byte sizes with MB, GB etc suffixes
    /// </summary>
    /// <param name="size">Number of bytes</param>
    /// <param name="precision">Number of decimals in formatted string</param>
    /// <returns>Number of bytes formatted as string</returns>
    public static string FormatBytes(long size, int precision)
    {
        foreach (var multiplier in Multipliers)
        {
            if (size >= (long)multiplier.Key)
            {
                var precisionFormatString =
                    PrecisionFormatStrings.GetOrAdd(precision,
                                                    precision => $"0.{new string('0', precision - 1)}");

                return $"{(size / (double)multiplier.Key).ToString(precisionFormatString)}{multiplier.Value}";
            }
        }

        return $"{size} byte";
    }
#else
    private static readonly Dictionary<int, string> PrecisionFormatStrings = new();

    /// <summary>
    /// Formats byte sizes with MB, GB etc suffixes
    /// </summary>
    /// <param name="size">Number of bytes</param>
    /// <param name="precision">Number of decimals in formatted string</param>
    /// <returns>Number of bytes formatted as string</returns>
    public static string FormatBytes(ulong size, int precision)
    {
        foreach (var multiplier in Multipliers)
        {
            if (size >= multiplier.Key)
            {
                lock (PrecisionFormatStrings)
                {
                    if (!PrecisionFormatStrings.TryGetValue(precision, out var precisionFormatString))
                    {
                        PrecisionFormatStrings.Add(precision, $"0.{new string('0', precision - 1)}");
                    }

                    return $"{(size / (double)multiplier.Key).ToString(precisionFormatString)}{multiplier.Value}";
                }
            }
        }

        return $"{size} byte";
    }

    /// <summary>
    /// Formats byte sizes with MB, GB etc suffixes
    /// </summary>
    /// <param name="size">Number of bytes</param>
    /// <param name="precision">Number of decimals in formatted string</param>
    /// <returns>Number of bytes formatted as string</returns>
    public static string FormatBytes(long size, int precision)
    {
        foreach (var multiplier in Multipliers)
        {
            if (size >= (long)multiplier.Key)
            {
                lock (PrecisionFormatStrings)
                {
                    if (!PrecisionFormatStrings.TryGetValue(precision, out var precisionFormatString))
                    {
                        PrecisionFormatStrings.Add(precision, $"0.{new string('0', precision - 1)}");
                    }

                    return $"{(size / (double)multiplier.Key).ToString(precisionFormatString)}{multiplier.Value}";
                }
            }
        }

        return $"{size} byte";
    }
#endif

    /// <summary>
    /// Formats byte sizes with MB, GB etc suffixes
    /// </summary>
    /// <param name="size">Number of bytes</param>
    /// <returns>Number of bytes formatted as string</returns>
    public static string FormatBytes(long size)
    {
        foreach (var multiplier in Multipliers)
        {
            if (Math.Abs(size) >= (long)multiplier.Key)
            {
                return $"{size / (double)multiplier.Key:0.0}{multiplier.Value}";
            }
        }

        return size == 1L ? $"{size} byte" : $"{size} bytes";
    }

    /// <summary>
    /// Attempts to parse a suffixed byte size, for example 1M as a numeric value.
    /// </summary>
    /// <param name="formatted">Suffixed size to convert</param>
    /// <returns>Number of bytes if sring was successfully parsed, null otherwise</returns>
    public static long? ParseSuffixedSize(string formatted)
        => TryParseSuffixedSize(formatted, out var result) ? result : null;

    /// <summary>
    /// Attempts to parse a suffixed byte size, for example 1M as a numeric value.
    /// </summary>
    /// <param name="formatted">Suffixed size to convert</param>
    /// <param name="parseSuffixedSizeRet">Number of bytes if sring was successfully parsed</param>
    /// <returns>True if sring was successfully parsed, false otherwise</returns>
    public static bool TryParseSuffixedSize(string formatted, out long parseSuffixedSizeRet)
    {
        parseSuffixedSizeRet = 0;

        if (string.IsNullOrEmpty(formatted))
        {
            return false;
        }

        if (formatted.StartsWith("0x", StringComparison.OrdinalIgnoreCase) || formatted.StartsWith("&H", StringComparison.Ordinal))
        {
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
            return long.TryParse(formatted.AsSpan(2), NumberStyles.AllowHexSpecifier, provider: null, out parseSuffixedSizeRet);
#else
            return long.TryParse(formatted.Substring(2), NumberStyles.AllowHexSpecifier, provider: null, out parseSuffixedSizeRet);
#endif
        }

        var Suffix = formatted[formatted.Length - 1];

        if (char.IsLetter(Suffix))
        {
            var factor = char.ToUpper(Suffix) switch
            {
                'E' => 1L << 60,
                'P' => 1L << 50,
                'T' => 1L << 40,
                'G' => 1L << 30,
                'M' => 1L << 20,
                'K' => 1L << 10,
                _ => throw new FormatException($"Bad suffix: {Suffix}"),
            };

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
            if (long.TryParse(formatted.AsSpan(0, formatted.Length - 1), NumberStyles.Any, provider: null, out parseSuffixedSizeRet))
            {
                parseSuffixedSizeRet *= factor;
                return true;
            }
#else
            if (long.TryParse(formatted.Substring(0, formatted.Length - 1), NumberStyles.Any, provider: null, out parseSuffixedSizeRet))
            {
                parseSuffixedSizeRet *= factor;
                return true;
            }
#endif

            return false;
        }
        else
        {
            return long.TryParse(formatted, NumberStyles.Any, provider: null, out parseSuffixedSizeRet);
        }
    }
}
