using System;
#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Buffers;
#endif
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LTRData.Extensions.Formatting;

/// <summary>
/// </summary>
public static class HexExtensions
{

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// </summary>
    public static string ToHexString(this ReadOnlySpan<byte> data, ReadOnlySpan<char> delimiter)
    {
        if (data.IsEmpty)
        {
            return string.Empty;
        }

        var capacity = data.Length << 1;
        if (!delimiter.IsEmpty)
        {
            capacity += delimiter.Length * (data.Length - 1);
        }

        var result = new string('\0', capacity);

        var ptr = MemoryMarshal.AsMemory(result.AsMemory()).Span;

        foreach (var b in data)
        {
            if (!delimiter.IsEmpty && ptr.Length < capacity)
            {
                delimiter.CopyTo(ptr);
                ptr = ptr.Slice(delimiter.Length);
            }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
            b.TryFormat(ptr, out _, "x2", NumberFormatInfo.InvariantInfo);
#else
            b.ToString("x2", NumberFormatInfo.InvariantInfo).AsSpan().CopyTo(ptr);
#endif
            ptr = ptr.Slice(2);
        }

        return result;
    }

    /// <summary>
    /// </summary>
    public static byte[] ParseHexString(string str)
        => ParseHexString(str.AsSpan());

    /// <summary>
    /// </summary>
    public static byte[] ParseHexString(ReadOnlySpan<char> str)
    {
        var bytes = new byte[(str.Length >> 1)];

        for (int i = 0, loopTo = bytes.Length - 1; i <= loopTo; i++)
        {
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
            bytes[i] = byte.Parse(str.Slice(i << 1, 2), NumberStyles.HexNumber);
#else
            bytes[i] = byte.Parse(str.Slice(i << 1, 2).ToString(), NumberStyles.HexNumber);
#endif
        }

        return bytes;
    }

    /// <summary>
    /// </summary>
    public static IEnumerable<byte> ParseHexString(IEnumerable<char> str)
    {
        var buffer = ArrayPool<char>.Shared.Rent(2);
        try
        {
            foreach (var c in str)
            {
                if (buffer[0] == '\0')
                {
                    buffer[0] = c;
                }
                else
                {
                    buffer[1] = c;
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
                    yield return byte.Parse(buffer.AsSpan(0, 2), NumberStyles.HexNumber);
#else
                    yield return byte.Parse(new string(buffer, 0, 2), NumberStyles.HexNumber);
#endif
                    buffer[0] = '\0';
                }
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// </summary>
    public static byte[] ParseHexString(string str, int offset, int count)
        => ParseHexString(str.AsSpan(offset, count));

    /// <summary>
    /// </summary>
    public static string? ToHexString(this IReadOnlyCollection<byte> data) => data.ToHexString(null);

    /// <summary>
    /// </summary>
    public static string? ToHexString(this IReadOnlyCollection<byte> data, string? delimiter)
    {
        if (data is null)
        {
            return null;
        }

        if (data.Count == 0)
        {
            return string.Empty;
        }

        var capacity = data.Count << 1;
        if (delimiter is not null)
        {
            capacity += delimiter.Length * (data.Count - 1);
        }

#if NETCOREAPP
        var result = string.Create(capacity,
            (data, delimiter, capacity),
            static (ptr, v) =>
            {
                foreach (var b in v.data)
                {
                    if (v.delimiter is not null && ptr.Length < v.capacity)
                    {
                        v.delimiter.AsSpan().CopyTo(ptr);
                        ptr = ptr.Slice(v.delimiter.Length);
                    }

                    b.TryFormat(ptr, out _, "x2", NumberFormatInfo.InvariantInfo);
                    ptr = ptr.Slice(2);
                }
            });
#else
        var result = new string('\0', capacity);

        var ptr = MemoryMarshal.AsMemory(result.AsMemory()).Span;

        foreach (var b in data)
        {
            if (delimiter is not null && ptr.Length < capacity)
            {
                delimiter.AsSpan().CopyTo(ptr);
                ptr = ptr.Slice(delimiter.Length);
            }

            b.ToString("x2", NumberFormatInfo.InvariantInfo).AsSpan().CopyTo(ptr);
            ptr = ptr.Slice(2);
        }
#endif

        return result;
    }

    /// <summary>
    /// </summary>
    public static bool TryFormatHexString(this byte[] data, ReadOnlySpan<char> delimiter, Span<char> destination, bool upperCase)
        => TryFormatHexString(data.AsSpan(), delimiter, destination, upperCase);

    /// <summary>
    /// </summary>
    public static bool TryFormatHexString(this Span<byte> data, ReadOnlySpan<char> delimiter, Span<char> destination, bool upperCase)
        => TryFormatHexString((ReadOnlySpan<byte>)data, delimiter, destination, upperCase);

    /// <summary>
    /// </summary>
    public static bool TryFormatHexString(this ReadOnlySpan<byte> data, ReadOnlySpan<char> delimiter, Span<char> destination, bool upperCase)
    {
        if (data.IsEmpty)
        {
            return true;
        }

        var capacity = data.Length << 1;
        if (!delimiter.IsEmpty)
        {
            capacity += delimiter.Length * (data.Length - 1);
        }

        if (capacity > destination.Length)
        {
            return false;
        }

        var ptr = destination;

        foreach (var b in data)
        {
            if (!delimiter.IsEmpty && ptr.Length < capacity)
            {
                delimiter.CopyTo(ptr);
                ptr = ptr.Slice(delimiter.Length);
            }

#if NETCOREAPP
            b.TryFormat(ptr, out _, upperCase ? "X2" : "x2", NumberFormatInfo.InvariantInfo);
#else
            b.ToString(upperCase ? "X2" : "x2", NumberFormatInfo.InvariantInfo).AsSpan().CopyTo(ptr);
#endif
            ptr = ptr.Slice(2);
        }

        return true;
    }

    /// <summary>
    /// </summary>
    public static string ToHexString(this byte[] data)
        => ((ReadOnlySpan<byte>)data).ToHexString(default);

    /// <summary>
    /// </summary>
    public static string ToHexString(this byte[] data, string? delimiter)
        => ((ReadOnlySpan<byte>)data).ToHexString(delimiter.AsSpan());

    /// <summary>
    /// </summary>
    public static string ToHexString(this byte[] data, int offset, int count)
        => data.AsSpan(offset, count).ToHexString(null);

    /// <summary>
    /// </summary>
    public static string ToHexString(this byte[] data, int offset, int count, string? delimiter)
        => data.AsSpan(offset, count).ToHexString(delimiter);

    /// <summary>
    /// </summary>
    public static string ToHexString(this Span<byte> data)
        => ((ReadOnlySpan<byte>)data).ToHexString(null);

    /// <summary>
    /// </summary>
    public static string ToHexString(this Span<byte> data, string? delimiter)
        => ((ReadOnlySpan<byte>)data).ToHexString(delimiter.AsSpan());

    /// <summary>
    /// </summary>
    public static string ToHexString(this ReadOnlySpan<byte> data)
        => data.ToHexString(null);

    /// <summary>
    /// </summary>
    public static TextWriter WriteHex(this TextWriter writer, IEnumerable<byte> bytes)
    {
        var i = 0;
        foreach (var line in bytes.FormatHexLines())
        {
            writer.WriteLine($"{(ushort)(i >> 16):X4} {(ushort)i:X4}  {line}");
            i += 0x10;
        }

        return writer;
    }

    /// <summary>
    /// </summary>
    public static IEnumerable<string> FormatHexLines(this IEnumerable<byte> bytes)
    {
        var sb = ArrayPool<char>.Shared.Rent(67);
        try
        {
            byte pos = 0;
            foreach (var b in bytes)
            {
                if (pos == 0)
                {
                    "                        -                                          ".CopyTo(0, sb, 0, 67);
                }

#if NETCOREAPP
                var bstr = 0;
                if ((pos & 8) == 0)
                {
                    bstr = pos * 3;
                }
                else
                {
                    bstr = 2 + pos * 3;
                }
                b.TryFormat(sb.AsSpan(bstr), out _, "X2");
#else
                var bstr = b.ToString("X2");
                if ((pos & 8) == 0)
                {
                    sb[pos * 3] = bstr[0];
                    sb[pos * 3 + 1] = bstr[1];
                }
                else
                {
                    sb[2 + pos * 3] = bstr[0];
                    sb[2 + pos * 3 + 1] = bstr[1];
                }
#endif

                sb[51 + pos] = char.IsControl((char)b) ? '.' : (char)b;

                pos++;
                pos &= 0xf;

                if (pos == 0)
                {
                    yield return new(sb, 0, 67);
                }
            }

            if (pos > 0)
            {
                yield return new(sb, 0, 67);
            }
        }
        finally
        {
            ArrayPool<char>.Shared.Return(sb);
        }
    }

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToHexString<T>(in T source) where T : unmanaged =>
        ToHexString(Buffers.BufferExtensions.AsReadOnlyBytes(source));

#endif

    /// <summary>
    /// Returns a string with each byte expressed in two-character hexadecimal notation.
    /// </summary>
    public static string? ToHexString(this IEnumerable<byte>? bytes)
    {
        if (bytes is null)
        {
            return null;
        }

        if (bytes is ICollection<byte> collection)
        {
            return collection.ToHexString();
        }

        var valuestr = new StringBuilder();
        foreach (var b in bytes)
        {
            valuestr.Append(b.ToString("x2"));
        }

        return valuestr.ToString();
    }

    /// <summary>
    /// Returns a string with each byte expressed in two-character hexadecimal notation.
    /// </summary>
    public static string? ToHexString(this IEnumerable<byte>? bytes, string? delimiter)
    {
        if (bytes is null)
        {
            return null;
        }

        if (bytes is ICollection<byte> collection)
        {
            return collection.ToHexString(delimiter);
        }

        var valuestr = new StringBuilder();
        foreach (var b in bytes)
        {
            if (valuestr.Length > 0)
            {
                valuestr.Append(delimiter ?? string.Empty);
            }

            valuestr.Append(b.ToString("x2"));
        }

        return valuestr.ToString();
    }
}
