using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace LTRData.Sensors;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class Temperature
{

#if NET462_OR_GREATER || NETCOREAPP || NETSTANDARD

    public static Func<IEnumerable<KeyValuePair<string, float>>> EnumerateThermalZone => field ??= GetEnumerateThermalZoneFunc();

    private static Func<IEnumerable<KeyValuePair<string, float>>> GetEnumerateThermalZoneFunc()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return EnumerateWindowsThermalZone;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return EnumerateLinuxThermalZone;
        }
        else
        {
            return () => [];
        }
    }

    [SupportedOSPlatform("linux")]
    public static IEnumerable<KeyValuePair<string, float>> EnumerateLinuxThermalZone()
    {
        return Directory.EnumerateDirectories("/sys/class/thermal", "thermal_zone*")
            .Select(zone =>
            {
                try
                {
                    var type = Path.Combine(zone, "type");

                    var temp = Path.Combine(zone, "temp");

                    if (File.Exists(type)
                        && File.Exists(temp)
                        && File.ReadLines(type).FirstOrDefault() is { } typeString
                        && float.TryParse(File.ReadLines(temp).FirstOrDefault(), out var reading))
                    {
                        return new KeyValuePair<string, float>(typeString, reading / 1000);
                    }
                }
                catch
                {
                }

                return default;
            })
            .Where(zone => zone.Key is not null);
    }

#endif

    [SupportedOSPlatform("windows")]
    public static IEnumerable<KeyValuePair<string, float>> EnumerateWindowsThermalZone()
    {
        var cat = new PerformanceCounterCategory("Thermal Zone Information");

        foreach (var name in cat.GetInstanceNames())
        {
            if (cat.CounterExists("High Precision Temperature"))
            {
                using var temp = new PerformanceCounter("Thermal Zone Information", "High Precision Temperature", name);
                yield return new(name, (temp.NextValue() / 10f) - 273.2f);
            }
            else if (cat.CounterExists("Temperature"))
            {
                using var temp = new PerformanceCounter("Thermal Zone Information", "Temperature", name);
                yield return new(name, temp.NextValue() - 273.2f);
            }
        }
    }
}
