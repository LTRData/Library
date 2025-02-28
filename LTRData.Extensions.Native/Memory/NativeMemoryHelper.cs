#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LTRData.Extensions.Native.Memory;

/// <summary>
/// Static helper functions for <see cref="NativeMemory{T}"/> implementation.
/// </summary>
public static class NativeMemoryHelper
{
    /// <summary>
    /// Returns a <see cref="String"/> object representing the same value as a
    /// <see cref="ReadOnlySpan{Char}"/>. For some string values that are very common
    /// for file system implementations, a cached intern string is returned instead of
    /// creating a new instance.
    /// </summary>
    /// <param name="span">Source string</param>
    /// <returns>Resulting string</returns>
    public static string GetStringFromSpan(ReadOnlySpan<char> span)
    {
        if (span.IsEmpty)
        {
            return "";
        }
        else if (span.Equals("/".AsSpan(), StringComparison.Ordinal))
        {
            return "/";
        }
        else if (span.Equals(@"\".AsSpan(), StringComparison.Ordinal))
        {
            return @"\";
        }
        else if (span.Equals("*".AsSpan(), StringComparison.Ordinal))
        {
            return "*";
        }
        else if (span.Equals("*.*".AsSpan(), StringComparison.Ordinal))
        {
            return "*.*";
        }
        else if (span.Equals("?".AsSpan(), StringComparison.Ordinal))
        {
            return "?";
        }
        else if (span.Equals(@"\Desktop.ini".AsSpan(), StringComparison.Ordinal))
        {
            return @"\Desktop.ini";
        }
        else if (span.Equals(@"\desktop.ini".AsSpan(), StringComparison.Ordinal))
        {
            return @"\desktop.ini";
        }
        else if (span.Equals(@"\AutoRun.inf".AsSpan(), StringComparison.Ordinal))
        {
            return @"\AutoRun.inf";
        }
        else
        {
            return span.ToString();
        }
    }

    /// <summary>
    /// Copies characters from a <see cref="string"/> to a <see cref="Span{Char}"/>,
    /// clearing remainder of the target memory if larger than the source string. If source is
    /// longer than target buffer size, target receives a truncated version of source string.
    /// </summary>
    /// <param name="buffer">Target buffer</param>
    /// <param name="str">Source string to copy to buffer</param>
    public static void SetString(this Span<char> buffer, string? str)
        => SetString(buffer, str.AsSpan());

    /// <summary>
    /// Copies characters from a <see cref="ReadOnlySpan{Char}"/> to a <see cref="Span{Char}"/>,
    /// clearing remainder of the target memory if larger than the source string. If source is
    /// longer than target buffer size, target receives a truncated version of source string.
    /// </summary>
    /// <param name="buffer">Target buffer</param>
    /// <param name="str">Source string to copy to buffer</param>
    public static void SetString(this Span<char> buffer, ReadOnlySpan<char> str)
    {
        if (str.IsEmpty)
        {
            buffer.Clear();
        }
        else if (str.Length < buffer.Length)
        {
            str.CopyTo(buffer);
            buffer.Slice(str.Length).Clear();
        }
        else
        {
            str.Slice(0, buffer.Length).CopyTo(buffer);
        }
    }

    /// <summary>
    /// Copies characters from a <see cref="string"/> to a <see cref="NativeMemory{Char}"/>,
    /// clearing remainder of the target memory if larger than the source string. If source is
    /// longer than target buffer size, target receives a truncated version of source string.
    /// </summary>
    /// <param name="buffer">Target buffer</param>
    /// <param name="str">Source string to copy to buffer</param>
    public static void SetString(this NativeMemory<char> buffer, string? str)
        => SetString(buffer.Span, str.AsSpan());

    /// <summary>
    /// Copies characters from a <see cref="ReadOnlySpan{Char}"/> to a <see cref="NativeMemory{Char}"/>,
    /// clearing remainder of the target memory if larger than the source string. If source is
    /// longer than target buffer size, target receives a truncated version of source string.
    /// </summary>
    /// <param name="buffer">Target buffer</param>
    /// <param name="str">Source string to copy to buffer</param>
    public static void SetString(this NativeMemory<char> buffer, ReadOnlySpan<char> str)
        => SetString(buffer.Span, str);
}

#endif
