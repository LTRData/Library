using LTRData.Extensions.Buffers;
using LTRData.Extensions.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LTRData.Extensions.CommandLine;

/// <summary>
/// </summary>
public static class CommandLineParser
{

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// </summary>
    public static Dictionary<string, string[]> ParseCommandLine(IEnumerable<string> args, StringComparer comparer)
    {
        var dict = ParseCommandLineParameter(args)
            .GroupBy(item => item.Key, item => item.Value, comparer)
            .ToDictionary(item => item.Key, item => item.SelectMany(i => i).ToArray(), comparer);

        return dict;
    }

    /// <summary>
    /// </summary>
    public static IEnumerable<KeyValuePair<string, IEnumerable<string>>> ParseCommandLineParameter(IEnumerable<string> args)
    {
        var switches_finished = false;

        foreach (var arg in args)
        {
            if (switches_finished)
            {
            }
            else if (arg.Length == 0 || arg == "-")
            {
                switches_finished = true;
            }
            else if (arg == "--")
            {
                switches_finished = true;
                continue;
            }
            else if (arg.StartsWith("--", StringComparison.Ordinal) || IOExtensions.IsWindows && arg.StartsWith("/", StringComparison.Ordinal))
            {
                var namestart = 1;
                if (arg[0] == '-')
                {
                    namestart = 2;
                }

                var valuepos = arg.IndexOf('=');
                if (valuepos < 0)
                {
                    valuepos = arg.IndexOf(':');
                }

                string name;
                IEnumerable<string> value;

                if (valuepos >= 0)
                {
                    name = arg.Substring(namestart, valuepos - namestart);
                    value = SingleValueEnumerable.Get(arg.Substring(valuepos + 1));
                }
                else
                {
                    name = arg.Substring(namestart);
                    value = Enumerable.Empty<string>();
                }

                yield return new KeyValuePair<string, IEnumerable<string>>(name, value);
            }
            else if (arg.StartsWith("-", StringComparison.Ordinal))
            {
                for (int i = 1, loopTo = arg.Length - 1; i <= loopTo; i++)
                {
                    var name = arg.Substring(i, 1);
                    IEnumerable<string> value;

                    if (i + 1 < arg.Length && (arg[i + 1] == '=' || arg[i + 1] == ':'))
                    {
                        value = SingleValueEnumerable.Get(arg.Substring(i + 2));
                        yield return new KeyValuePair<string, IEnumerable<string>>(name, value);
                        break;
                    }

                    value = Enumerable.Empty<string>();
                    yield return new KeyValuePair<string, IEnumerable<string>>(name, value);
                }
            }
            else
            {
                switches_finished = true;
            }

            if (switches_finished)
            {
                yield return new KeyValuePair<string, IEnumerable<string>>(string.Empty, SingleValueEnumerable.Get(arg));
            }
        }
    }

#endif
}
