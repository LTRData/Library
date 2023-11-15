using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using LTRData.Extensions.Formatting;

namespace LTRData.Extensions.IO;

/// <summary>
/// </summary>
public static class ConsoleSupport
{
    /// <summary>
    /// </summary>
    public static string GetConsoleOutputDeviceName()
#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP
        => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "CONOUT$" : "/dev/tty";
#else
        => "CONOUT$";
#endif

    /// <summary>
    /// </summary>
    public static readonly object ConsoleSync = new();

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// </summary>
    public static TraceLevel WriteMsgTraceLevel { get; set; } = TraceLevel.Info;

    /// <summary>
    /// </summary>
    public static void WriteMsg(TraceLevel level, string msg)
    {
        var color = level switch
        {
            TraceLevel.Off => ConsoleColor.Cyan,
            TraceLevel.Error => ConsoleColor.Red,
            TraceLevel.Warning => ConsoleColor.Yellow,
            TraceLevel.Info => ConsoleColor.Gray,
            TraceLevel.Verbose => ConsoleColor.DarkGray,
            _ => ConsoleColor.DarkGray,
        };

        if (level <= WriteMsgTraceLevel)
        {
            lock (ConsoleSync)
            {
                Console.ForegroundColor = color;

                Console.WriteLine(StringFormatting.LineFormat(msg.AsSpan()));

                Console.ResetColor();
            }
        }
    }
#endif
}
