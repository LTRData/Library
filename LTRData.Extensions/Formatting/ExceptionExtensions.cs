using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LTRData.Extensions.Formatting;

/// <summary>
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// </summary>
    public static IEnumerable<Exception> Enumerate(this Exception? ex)
    {
        while (ex is not null)
        {
            if (ex is TargetInvocationException)
            {
                ex = ex.InnerException;
            }
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
            else if (ex is AggregateException aex)
            {
                foreach (var iex in aex.InnerExceptions.SelectMany(Enumerate))
                {
                    yield return iex;
                }

                yield break;
            }
#endif
            else if (ex is ReflectionTypeLoadException rtlex)
            {
                yield return ex;

                foreach (var iex in rtlex.LoaderExceptions.SelectMany(Enumerate))
                {
                    yield return iex;
                }

                ex = ex.InnerException;
            }
            else
            {
                yield return ex;

                ex = ex.InnerException;
            }
        }
    }

    /// <summary>
    /// </summary>
    public static IEnumerable<string> EnumerateMessages(this Exception? ex)
    {
        while (ex is not null)
        {
            if (ex is TargetInvocationException)
            {
                ex = ex.InnerException;
            }
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
            else if (ex is AggregateException agex)
            {
                foreach (var msg in agex.InnerExceptions.SelectMany(EnumerateMessages))
                {
                    yield return msg;
                }

                yield break;
            }
#endif
            else if (ex is ReflectionTypeLoadException tlex)
            {
                yield return ex.Message;

                foreach (var msg in tlex.LoaderExceptions.SelectMany(EnumerateMessages))
                {
                    yield return msg;
                }

                ex = ex.InnerException;
            }
            else if (ex is Win32Exception win32ex)
            {
                yield return $"{win32ex.Message} ({win32ex.NativeErrorCode})";

                ex = ex.InnerException;
            }
            else
            {
                yield return ex.Message;

                ex = ex.InnerException;
            }
        }
    }

    /// <summary>
    /// </summary>
    public static string JoinMessages(this Exception exception) =>
        exception.JoinMessages(Environment.NewLine + Environment.NewLine);

    /// <summary>
    /// </summary>
    public static string JoinMessages(this Exception exception, string separator) =>
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
        string.Join(separator, exception.EnumerateMessages());
#else
        string.Join(separator, exception.EnumerateMessages().ToArray());
#endif

}
