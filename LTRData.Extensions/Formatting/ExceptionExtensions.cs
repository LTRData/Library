using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LTRData.Extensions.Formatting;

public static class ExceptionExtensions
{
    public static IEnumerable<Exception> Enumerate(this Exception? ex)
    {
        while (ex is not null)
        {
            if (ex is TargetInvocationException)
            {
                ex = ex.InnerException;
            }
            else if (ex is AggregateException aex)
            {
                foreach (var iex in aex.InnerExceptions.SelectMany(Enumerate))
                {
                    yield return iex;
                }

                yield break;
            }
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

    public static IEnumerable<string> EnumerateMessages(this Exception? ex)
    {
        while (ex is not null)
        {
            if (ex is TargetInvocationException)
            {
                ex = ex.InnerException;
            }
            else if (ex is AggregateException agex)
            {
                foreach (var msg in agex.InnerExceptions.SelectMany(EnumerateMessages))
                {
                    yield return msg;
                }

                yield break;
            }
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string JoinMessages(this Exception exception) =>
        exception.JoinMessages(Environment.NewLine + Environment.NewLine);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string JoinMessages(this Exception exception, string separator) =>
        string.Join(separator, exception.EnumerateMessages());

}
