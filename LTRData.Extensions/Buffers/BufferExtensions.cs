using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using LTRData.Extensions.Buffers;
#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP
using LTRData.Extensions.Split;
#endif

namespace LTRData.Extensions.Buffers;

/// <summary>
/// </summary>
public static class BufferExtensions
{

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// Encapsulation of <see cref="string.Join(string?, IEnumerable{string?})"/> as an extension method
    /// </summary>
    /// <param name="strings">String to join</param>
    /// <param name="separator">Separator to insert between each joined element</param>
    /// <returns>Joined string</returns>
    public static string Join(this IEnumerable<string> strings, string separator) => string.Join(separator, strings);

    /// <summary>
    /// Encapsulation of <see cref="string.Join(string?, string?[])"/> as an extension method
    /// </summary>
    /// <param name="strings">String to join</param>
    /// <param name="separator">Separator to insert between each joined element</param>
    /// <returns>Joined string</returns>
    public static string Join(this string[] strings, string separator) => string.Join(separator, strings);

#else

    /// <summary>
    /// Encapsulation of <see cref="string.Join(string, string[])"/> as an extension method
    /// </summary>
    /// <param name="strings">String to join</param>
    /// <param name="separator">Separator to insert between each joined element</param>
    /// <returns>Joined string</returns>
    public static string Join(this IEnumerable<string> strings, string separator) => string.Join(separator, strings.ToArray());

    /// <summary>
    /// Encapsulation of <see cref="string.Join(string?, string?[])"/> as an extension method
    /// </summary>
    /// <param name="strings">String to join</param>
    /// <param name="separator">Separator to insert between each joined element</param>
    /// <returns>Joined string</returns>
    public static string Join(this string[] strings, string separator) => string.Join(separator, strings);

#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP

    /// <summary>
    /// Encapsulation of <see cref="string.Join{T}(char, IEnumerable{T})"/> as an extension method
    /// </summary>
    /// <param name="strings">String to join</param>
    /// <param name="separator">Separator to insert between each joined element</param>
    /// <returns>Joined string</returns>
    public static string Join(this IEnumerable<string> strings, char separator) => string.Join(separator, strings);

    /// <summary>
    /// Encapsulation of <see cref="string.Join(char, string?[])"/> as an extension method
    /// </summary>
    /// <param name="strings">String to join</param>
    /// <param name="separator">Separator to insert between each joined element</param>
    /// <returns>Joined string</returns>
    public static string Join(this string[] strings, char separator) => string.Join(separator, strings);

#elif NETSTANDARD || NET40_OR_GREATER

    /// <summary>
    /// Encapsulation of <see cref="string.Join{T}(string, IEnumerable{T})"/> as an extension method
    /// </summary>
    /// <param name="strings">String to join</param>
    /// <param name="separator">Separator to insert between each joined element</param>
    /// <returns>Joined string</returns>
    public static string Join(this IEnumerable<string> strings, char separator) => string.Join(separator.ToString(), strings);

    /// <summary>
    /// Encapsulation of <see cref="string.Join(string, string?[])"/> as an extension method
    /// </summary>
    /// <param name="strings">String to join</param>
    /// <param name="separator">Separator to insert between each joined element</param>
    /// <returns>Joined string</returns>
    public static string Join(this string[] strings, char separator) => string.Join(separator.ToString(), strings);

#else

    /// <summary>
    /// Encapsulation of <see cref="string.Join(string, string?[])"/> as an extension method
    /// </summary>
    /// <param name="strings">String to join</param>
    /// <param name="separator">Separator to insert between each joined element</param>
    /// <returns>Joined string</returns>
    public static string Join(this IEnumerable<string> strings, char separator) => string.Join(separator.ToString(), strings.ToArray());

    /// <summary>
    /// Encapsulation of <see cref="string.Join(string, string?[])"/> as an extension method
    /// </summary>
    /// <param name="strings">String to join</param>
    /// <param name="separator">Separator to insert between each joined element</param>
    /// <returns>Joined string</returns>
    public static string Join(this string[] strings, char separator) => string.Join(separator.ToString(), strings);

#endif

    /// <summary>
    /// Encapsulation of <see cref="string.Concat(string?[])"/> as an extension method
    /// </summary>
    /// <param name="strings">String to join</param>
    /// <returns>Joined string</returns>
    public static string Concat(this IEnumerable<string> strings) => string.Concat(strings);

    /// <summary>
    /// Encapsulation of <see cref="string.Concat(string?[])"/> as an extension method
    /// </summary>
    /// <param name="strings">String to join</param>
    /// <returns>Joined string</returns>
    public static string Concat(this string[] strings) => string.Concat(strings);

#if NET45_OR_GREATER
    /// <summary>
    /// Returns a read-only wrapper for a dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) where TKey : notnull
        => new(dictionary);
#endif

#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ToEnumerable<T>(this Memory<T> span) => MemoryMarshal.ToEnumerable((ReadOnlyMemory<T>)span);

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<T> ToEnumerable<T>(this ReadOnlyMemory<T> span) => MemoryMarshal.ToEnumerable(span);

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsChars(this byte[] bytes) => MemoryMarshal.Cast<byte, char>(bytes);

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsChars(this Memory<byte> bytes) => MemoryMarshal.Cast<byte, char>(bytes.Span);

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsChars(this ReadOnlyMemory<byte> bytes) => MemoryMarshal.Cast<byte, char>(bytes.Span);

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsChars(this Span<byte> bytes) => MemoryMarshal.Cast<byte, char>(bytes);

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> AsChars(this ReadOnlySpan<byte> bytes) => MemoryMarshal.Cast<byte, char>(bytes);

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<byte> AsSpan(this SafeBuffer ptr) =>
        new(ptr.DangerousGetHandle().ToPointer(), (int)ptr.ByteLength);

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> AsSpan(this MemoryStream memoryStream) =>
        memoryStream.GetBuffer().AsSpan(0, checked((int)memoryStream.Length));

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<byte> AsSpan(this UnmanagedMemoryStream memoryStream) =>
        new(memoryStream.PositionPointer - memoryStream.Position, (int)memoryStream.Length);

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<byte> AsMemory(this MemoryStream memoryStream) =>
        memoryStream.GetBuffer().AsMemory(0, checked((int)memoryStream.Length));

#if !NETCOREAPP
    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T CastRef<T>(this ReadOnlySpan<byte> bytes) where T : unmanaged =>
        ref MemoryMarshal.Cast<byte, T>(bytes)[0];

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T CastRef<T>(this Span<byte> bytes) where T : unmanaged =>
        ref MemoryMarshal.Cast<byte, T>(bytes)[0];
#else
    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref readonly T CastRef<T>(this ReadOnlySpan<byte> bytes) where T : unmanaged =>
        ref MemoryMarshal.AsRef<T>(bytes);

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T CastRef<T>(this Span<byte> bytes) where T : unmanaged =>
        ref MemoryMarshal.AsRef<T>(bytes);
#endif

    /// <summary>
    /// Sets a bit to 1 in a bit field.
    /// </summary>
    /// <param name="data">Bit field</param>
    /// <param name="bitnumber">Bit number to set to 1</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetBit(this Span<byte> data, int bitnumber) =>
        data[bitnumber >> 3] |= (byte)(1 << (~bitnumber & 7));

    /// <summary>
    /// Sets a bit to 0 in a bit field.
    /// </summary>
    /// <param name="data">Bit field</param>
    /// <param name="bitnumber">Bit number to set to 0</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ClearBit(this Span<byte> data, int bitnumber) =>
        data[bitnumber >> 3] &= unchecked((byte)~(1 << (~bitnumber & 7)));

    /// <summary>
    /// Gets a bit from a bit field.
    /// </summary>
    /// <param name="data">Bit field</param>
    /// <param name="bitnumber">Bit number to get</param>
    /// <returns>True if value of specified bit is 1, false if 0.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool GetBit(this ReadOnlySpan<byte> data, int bitnumber) =>
        (data[bitnumber >> 3] & 1 << (~bitnumber & 7)) != 0;

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> CreateReadOnlySpan<T>(in T source, int length) =>
        MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(source), length);

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> CreateSpan<T>(ref T source, int length) =>
        MemoryMarshal.CreateSpan(ref source, length);

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> AsReadOnlyBytes<T>(in T source) where T : unmanaged =>
        MemoryMarshal.AsBytes(CreateReadOnlySpan(source, 1));

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> AsBytes<T>(ref T source) where T : unmanaged =>
        MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref source, 1));

    /// <summary>
    /// </summary>
    public static int GetHashCode<T>(in T source) where T : unmanaged =>
        GetHashCode(AsReadOnlyBytes(source));

#else

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> AsReadOnlyBytes<T>(in T source) where T : unmanaged =>
        MemoryMarshal.AsBytes(CreateReadOnlySpan(source, 1));

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ReadOnlySpan<T> CreateReadOnlySpan<T>(in T source, int length) =>
        new(Unsafe.AsPointer(ref Unsafe.AsRef(source)), length);

    /// <summary>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe Span<T> CreateSpan<T>(ref T source, int length) =>
        new(Unsafe.AsPointer(ref source), length);

#endif

    /// <summary>
    /// </summary>
    public static int GetHashCode(ReadOnlySpan<byte> ptr)
    {
        var result = 0;
        for (var i = 0; i < ptr.Length; i++)
        {
            result ^= ptr[i] << (i & 0x3) * 8;
        }

        return result;
    }

#if !NET6_0_OR_GREATER
    /// <summary>
    /// Returns character memory block beyond whitespace characters
    /// </summary>
    /// <param name="str">Character memory block</param>
    /// <returns>Character memory block beyond whitespace characters</returns>
    public static ReadOnlyMemory<char> TrimStart(this ReadOnlyMemory<char> str)
        => str.Slice(str.Length - str.Span.TrimStart().Length);

    /// <summary>
    /// Returns character memory block before whitespace characters
    /// </summary>
    /// <param name="str">Character memory block</param>
    /// <returns>Character memory block before whitespace characters</returns>
    public static ReadOnlyMemory<char> TrimEnd(this ReadOnlyMemory<char> str)
        => str.Slice(0, str.Span.TrimEnd().Length);

    /// <summary>
    /// Returns character memory block beyond and before whitespace characters
    /// </summary>
    /// <param name="str">Character memory block</param>
    /// <returns>Character memory block beyond and before whitespace characters</returns>
    public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> str)
        => str.TrimStart().TrimEnd();

    /// <summary>
    /// Returns character memory block beyond given trim character
    /// </summary>
    /// <param name="str">Character memory block</param>
    /// <param name="chr">Character to trim</param>
    /// <returns>Character memory block beyond given trim character</returns>
    public static ReadOnlyMemory<char> TrimStart(this ReadOnlyMemory<char> str, char chr)
        => str.Slice(str.Length - str.Span.TrimStart(chr).Length);

    /// <summary>
    /// Returns character memory block before given trim character
    /// </summary>
    /// <param name="str">Character memory block</param>
    /// <param name="chr">Character to trim</param>
    /// <returns>Character memory block before given trim string</returns>
    public static ReadOnlyMemory<char> TrimEnd(this ReadOnlyMemory<char> str, char chr)
        => str.Slice(0, str.Span.TrimEnd(chr).Length);

    /// <summary>
    /// Returns character memory block beyond and before given trim character
    /// </summary>
    /// <param name="str">Character memory block</param>
    /// <param name="chr">Character to trim</param>
    /// <returns>Character memory block beyond given trim character</returns>
    public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> str, char chr)
        => str.TrimStart(chr).TrimEnd(chr);

    /// <summary>
    /// Returns character memory block beyond given trim haracter
    /// </summary>
    /// <param name="str">Character memory block</param>
    /// <param name="chrs">String to trim</param>
    /// <returns>Character memory block beyond given trim string</returns>
    public static ReadOnlyMemory<char> TrimStart(this ReadOnlyMemory<char> str, ReadOnlySpan<char> chrs)
        => str.Slice(str.Span.TrimStart(chrs).Length - str.Length);

    /// <summary>
    /// Returns character memory block before given trim string
    /// </summary>
    /// <param name="str">Character memory block</param>
    /// <param name="chrs">String to trim</param>
    /// <returns>Character memory block before given trim string</returns>
    public static ReadOnlyMemory<char> TrimEnd(this ReadOnlyMemory<char> str, ReadOnlySpan<char> chrs)
        => str.Slice(0, str.Span.TrimEnd(chrs).Length);

    /// <summary>
    /// Returns character memory block beyond and before given trim string
    /// </summary>
    /// <param name="str">Character memory block</param>
    /// <param name="chrs">String to trim</param>
    /// <returns>Character memory block beyond and before given trim string</returns>
    public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> str, ReadOnlySpan<char> chrs)
        => str.TrimStart(chrs).TrimEnd(chrs);

#endif

#if !NETCOREAPP

    /// <summary>
    /// </summary>
    public static StringBuilder Append(this StringBuilder sb, ReadOnlyMemory<char> value)
    {
        if (MemoryMarshal.TryGetString(value, out var text, out var start, out var length))
        {
            return sb.Append(text, start, length);
        }

        return sb.Append(value.ToString());
    }

#endif

    /// <summary>
    /// Returns character memory block beyond given trim characters
    /// </summary>
    /// <param name="str">Character memory block</param>
    /// <param name="chrs">Characters to trim</param>
    /// <returns>Character memory block beyond given trim characters</returns>
    public static ReadOnlyMemory<char> TrimStartAny(this ReadOnlyMemory<char> str, char[] chrs)
        => str.Slice(str.Span.TrimStartAny(chrs).Length - str.Length);

    /// <summary>
    /// Returns character memory block before given trim characters
    /// </summary>
    /// <param name="str">Character memory block</param>
    /// <param name="chrs">Characters to trim</param>
    /// <returns>Character memory block before given trim characters</returns>
    public static ReadOnlyMemory<char> TrimEndAny(this ReadOnlyMemory<char> str, char[] chrs)
        => str.Slice(0, str.Span.TrimEndAny(chrs).Length);

    /// <summary>
    /// Returns character memory block beyond and before given trim characters
    /// </summary>
    /// <param name="str">Character memory block</param>
    /// <param name="chrs">Characters to trim</param>
    /// <returns>Character memory block beyond and before given trim characters</returns>
    public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> str, char[] chrs)
        => str.TrimStartAny(chrs).TrimEndAny(chrs);

    /// <summary>
    /// Returns character memory block beyond given trim characters
    /// </summary>
    /// <param name="str">Character memory block</param>
    /// <param name="chrs">Characters to trim</param>
    /// <returns>Character memory block beyond given trim characters</returns>
    public static ReadOnlySpan<char> TrimStartAny(this ReadOnlySpan<char> str, char[] chrs)
    {
        foreach (var chr in chrs)
        {
            str = str.TrimStart(chr);
        }

        return str;
    }

    /// <summary>
    /// Returns character memory block before given trim characters
    /// </summary>
    /// <param name="str">Character memory block</param>
    /// <param name="chrs">Characters to trim</param>
    /// <returns>Character memory block before given trim characters</returns>
    public static ReadOnlySpan<char> TrimEndAny(this ReadOnlySpan<char> str, char[] chrs)
    {
        foreach (var chr in chrs)
        {
            str = str.TrimEnd(chr);
        }

        return str;
    }
    

    /// <summary>
    /// Return a managed reference to Span, or a managed null reference
    /// if Span is empty.
    /// </summary>
    /// <param name="span">Span to return reference for or null</param>
    /// <returns>Managed reference</returns>
    public static unsafe ref readonly T AsRef<T>(this ReadOnlySpan<T> span) where T : unmanaged
    {
        if (span.IsEmpty)
        {
            return ref *(T*)null;
        }
        else
        {
            return ref span[0];
        }
    }

    /// <summary>
    /// Return a managed reference to Span, or a managed null reference
    /// if Span is empty.
    /// </summary>
    /// <param name="span">Span to return reference for or null</param>
    /// <returns>Managed reference</returns>
    public static unsafe ref T AsRef<T>(this Span<T> span) where T : unmanaged
    {
        if (span.IsEmpty)
        {
            return ref *(T*)null;
        }
        else
        {
            return ref span[0];
        }
    }

    /// <summary>
    /// Return a managed reference to string, or a managed null reference
    /// if given a null reference.
    /// </summary>
    /// <param name="str">String to return reference for or null</param>
    /// <returns>Managed reference</returns>
    public static unsafe ref readonly char AsRef(this string? str)
    {
        if (str is null)
        {
            return ref *(char*)null;
        }
        else
        {
            return ref MemoryMarshal.GetReference(str.AsSpan());
        }
    }

    /// <summary>
    /// Returns a reference to a character string guaranteed to be null
    /// terminated. If the supplied buffer is null terminated, a reference
    /// to the first character in buffer is returned. Otherwise, a new char
    /// array is created, data copied to it, and a reference to the first
    /// character in the new array is returned.
    /// </summary>
    /// <param name="strMemory">Input string</param>
    /// <returns>Reference to output string with characters equal to
    /// input string, but guaranteed to be null terminated.</returns>
    public static unsafe ref readonly char MakeNullTerminated(this ReadOnlyMemory<char> strMemory)
    {
        if (strMemory.IsEmpty)
        {
            return ref *(char*)null;
        }

        if (MemoryMarshal.TryGetString(strMemory, out var text, out var start, out var length) &&
            start + length == text.Length)
        {
            return ref MemoryMarshal.GetReference(strMemory.Span);
        }

        if (MemoryMarshal.TryGetArray(strMemory, out var chars) &&
            chars.Array is not null &&
            chars.Offset + chars.Count < chars.Array.Length &&
            chars.Array[chars.Offset + chars.Count] == '\0')
        {
            return ref chars.Array[chars.Offset];
        }

        var buffer = new char[strMemory.Length + 1];
        strMemory.CopyTo(buffer);
        return ref buffer[0];
    }

    /// <summary>
    /// Workaround for Visual Basic Span consumers
    /// </summary>
    /// <typeparam name="T">Type of elements of span</typeparam>
    /// <param name="span">span</param>
    /// <param name="index">index of element in span to return</param>
    /// <returns>Copy of element at position</returns>
    public static T GetItem<T>(this ReadOnlySpan<T> span, int index) => span[index];

    /// <summary>
    /// Workaround for Visual Basic Span consumers
    /// </summary>
    /// <typeparam name="T">Type of elements of span</typeparam>
    /// <param name="span">span</param>
    /// <param name="index">index of element in span to return</param>
    /// <returns>Copy of element at position</returns>
    public static T GetItem<T>(this Span<T> span, int index) => span[index];

    /// <summary>
    /// Workaround for Visual Basic Span consumers
    /// </summary>
    /// <typeparam name="T">Type of elements of span</typeparam>
    /// <param name="memory">span</param>
    /// <param name="index">index of element in span to set</param>
    /// <param name="item">reference to item to assign to index in span</param>
    public static void SetItem<T>(this Memory<T> memory, int index, in T item) => memory.Span[index] = item;

    /// <summary>
    /// Workaround for Visual Basic Span consumers
    /// </summary>
    /// <typeparam name="T">Type of elements of span</typeparam>
    /// <param name="span">span</param>
    /// <param name="index">index of element in span to set</param>
    /// <param name="item">reference to item to assign to index in span</param>
    public static void SetItem<T>(this Span<T> span, int index, in T item) => span[index] = item;

    /// <summary>
    /// Workaround for Visual Basic Span consumers
    /// </summary>
    /// <typeparam name="T">Type of elements of span</typeparam>
    /// <param name="memory">span</param>
    /// <param name="index">index of element in span to return</param>
    /// <returns>Copy of element at position</returns>
    public static T GetItem<T>(this ReadOnlyMemory<T> memory, int index) => memory.Span[index];

    /// <summary>
    /// Workaround for Visual Basic Span consumers
    /// </summary>
    /// <typeparam name="T">Type of elements of span</typeparam>
    /// <param name="memory">span</param>
    /// <param name="index">index of element in span to return</param>
    /// <returns>Copy of element at position</returns>
    public static T GetItem<T>(this Memory<T> memory, int index) => memory.Span[index];

    /// <summary>
    /// Parses a multi-string where each string is terminated by null char
    /// and the whole buffer is terminated by double null chars.
    /// </summary>
    /// <param name="bytes">Memory that contains the double null terminated string</param>
    /// <returns>Each individual string in the buffer</returns>
    public static IEnumerable<string> ParseDoubleTerminatedString(this Memory<byte> bytes) =>
        ((ReadOnlyMemory<byte>)bytes).ParseDoubleTerminatedString();

    /// <summary>
    /// Parses a multi-string where each string is terminated by null char
    /// and the whole buffer is terminated by double null chars.
    /// </summary>
    /// <param name="bytes">Memory that contains the double null terminated string</param>
    /// <returns>Each individual string in the buffer</returns>
    public static IEnumerable<string> ParseDoubleTerminatedString(this ReadOnlyMemory<byte> bytes)
    {
        var endpos = MemoryMarshal.Cast<byte, char>(bytes.Span).IndexOf('\0');

        while (endpos > 0)
        {
            yield return MemoryMarshal.Cast<byte, char>(bytes.Span).Slice(0, endpos).ToString();

            bytes = bytes.Slice(endpos + 1 << 1);

            endpos = MemoryMarshal.Cast<byte, char>(bytes.Span).IndexOf('\0');
        }
    }

    /// <summary>
    /// Parses a multi-string where each string is terminated by null char
    /// and the whole buffer is terminated by double null chars.
    /// </summary>
    /// <param name="chars">Memory that contains the double null terminated string</param>
    /// <returns>Each individual string in the buffer</returns>
    public static IEnumerable<ReadOnlyMemory<char>> ParseDoubleTerminatedString(this Memory<char> chars) =>
        ((ReadOnlyMemory<char>)chars).ParseDoubleTerminatedString();

    /// <summary>
    /// Parses a multi-string where each string is terminated by null char
    /// and the whole buffer is terminated by double null chars.
    /// </summary>
    /// <param name="chars">Memory that contains the double null terminated string</param>
    /// <returns>Each individual string in the buffer</returns>
    public static IEnumerable<ReadOnlyMemory<char>> ParseDoubleTerminatedString(this ReadOnlyMemory<char> chars)
    {
        var endpos = chars.Span.IndexOf('\0');

        while (endpos > 0)
        {
            yield return chars.Slice(0, endpos);

            chars = chars.Slice(endpos + 1);

            endpos = chars.Span.IndexOf('\0');
        }
    }

    /// <summary>
    /// </summary>
    public static bool Contains(this ReadOnlySpan<char> str, char chr)
        => str.IndexOf(chr) >= 0;

#endif

#if !NETCOREAPP && !NETSTANDARD && !NET461_OR_GREATER

    /// <summary>
    /// Appends an item to an enumeration
    /// </summary>
    /// <typeparam name="T">Type of items in enumeration</typeparam>
    /// <param name="values">Source enumeration</param>
    /// <param name="value">Item to append</param>
    /// <returns>New enumeration with item appended</returns>
    public static IEnumerable<T> Append<T>(this IEnumerable<T> values, T value)
    {
        foreach (var v in values)
        {
            yield return v;
        }

        yield return value;
    }

    /// <summary>
    /// Prepends an item to an enumeration
    /// </summary>
    /// <typeparam name="T">Type of items in enumeration</typeparam>
    /// <param name="values">Source enumeration</param>
    /// <param name="value">Item to prepend</param>
    /// <returns>New enumeration with item prepended</returns>
    public static IEnumerable<T> Prepend<T>(this IEnumerable<T> values, T value)
    {
        yield return value;

        foreach (var v in values)
        {
            yield return v;
        }
    }

#endif

    /// <summary>
    /// </summary>
    public static string[] Split(this string str, char separator, int count, StringSplitOptions options = StringSplitOptions.None)
        => str.Split(new[] { separator }, count, options);

    /// <summary>
    /// </summary>
    public static string[] Split(this string str, char separator, StringSplitOptions options = StringSplitOptions.None)
        => str.Split(new[] { separator }, options);

#if !NETSTANDARD2_1_OR_GREATER && !NETCOREAPP
    /// <summary>
    /// </summary>
    public static bool Contains(this string str, char chr)
        => str.IndexOf(chr) >= 0;

    /// <summary>
    /// </summary>
    public static bool Contains(this string str, string substr)
        => str.IndexOf(substr) >= 0;

    /// <summary>
    /// </summary>
    public static bool Contains(this string str, string substr, StringComparison comparison)
        => str.IndexOf(substr, comparison) >= 0;

    /// <summary>
    /// </summary>
    public static bool StartsWith(this string str, char chr)
        => str is not null && str.Length > 0 && str[0] == chr;

    /// <summary>
    /// </summary>
    public static bool EndsWith(this string str, char chr)
        => str is not null && str.Length > 0 && str[str.Length - 1] == chr;
#endif

#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// Return position of first empty element, or the entire span length if
    /// no empty elements are found.
    /// </summary>
    /// <param name="buffer">Span to search</param>
    /// <returns>Position of first found empty element or entire span length if none found</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfTerminator<T>(this ReadOnlySpan<T> buffer) where T : unmanaged, IEquatable<T>
    {
        var endpos = buffer.IndexOf(default(T));
        return endpos >= 0 ? endpos : buffer.Length;
    }

    /// <summary>
    /// Return position of first empty element, or the entire span length if
    /// no empty elements are found.
    /// </summary>
    /// <param name="buffer">Span to search</param>
    /// <returns>Position of first found empty element or entire span length if none found</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfTerminator<T>(this Span<T> buffer) where T : unmanaged, IEquatable<T>
    {
        var endpos = buffer.IndexOf(default(T));
        return endpos >= 0 ? endpos : buffer.Length;
    }

    /// <summary>
    /// Reads null terminated ASCII string from byte buffer.
    /// </summary>
    /// <param name="buffer">Byte buffer</param>
    /// <param name="offset">Offset in byte buffer where the string starts</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadNullTerminatedAsciiString(byte[] buffer, int offset)
    {
        var endpos = buffer.AsSpan(offset).IndexOfTerminator();
        return Encoding.ASCII.GetString(buffer, offset, endpos);
    }

    /// <summary>
    /// Reads null terminated Unicode string from byte buffer.
    /// </summary>
    /// <param name="buffer">Byte buffer</param>
    /// <param name="offset">Offset in byte buffer where the string starts</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> ReadNullTerminatedUnicode(byte[] buffer, int offset)
        => MemoryMarshal.Cast<byte, char>(buffer.AsSpan(offset)).ReadNullTerminatedUnicode();

    /// <summary>
    /// Reads null terminated Unicode string from byte buffer.
    /// </summary>
    /// <param name="buffer">Byte buffer</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<char> ReadNullTerminatedUnicode(this Span<byte> buffer)
        => MemoryMarshal.Cast<byte, char>(buffer).ReadNullTerminatedUnicode();

    /// <summary>
    /// Reads null terminated Unicode string from byte buffer.
    /// </summary>
    /// <param name="buffer">Byte buffer</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> ReadNullTerminatedUnicode(this ReadOnlySpan<byte> buffer)
        => MemoryMarshal.Cast<byte, char>(buffer).ReadNullTerminatedUnicode();

    /// <summary>
    /// Reads null terminated Unicode string from char buffer.
    /// </summary>
    /// <param name="chars">Buffer</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<char> ReadNullTerminatedUnicode(this ReadOnlySpan<char> chars)
    {
        var endpos = chars.IndexOfTerminator();
        return chars.Slice(0, endpos);
    }

    /// <summary>
    /// Reads null terminated Unicode string from char buffer.
    /// </summary>
    /// <param name="chars">Buffer</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<char> ReadNullTerminatedUnicode(this Span<char> chars)
    {
        var endpos = chars.IndexOfTerminator();
        return chars.Slice(0, endpos);
    }

    /// <summary>
    /// Reads null terminated Unicode string from byte buffer.
    /// </summary>
    /// <param name="buffer">Byte buffer</param>
    /// <param name="offset">Offset in byte buffer where the string starts</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadNullTerminatedUnicodeString(byte[] buffer, int offset)
        => MemoryMarshal.Cast<byte, char>(buffer.AsSpan(offset)).ReadNullTerminatedUnicodeString();

    /// <summary>
    /// Reads null terminated Unicode string from byte buffer.
    /// </summary>
    /// <param name="buffer">Byte buffer</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadNullTerminatedUnicodeString(this Span<byte> buffer)
        => MemoryMarshal.Cast<byte, char>(buffer).ReadNullTerminatedUnicodeString();

    /// <summary>
    /// Reads null terminated Unicode string from byte buffer.
    /// </summary>
    /// <param name="buffer">Byte buffer</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadNullTerminatedUnicodeString(this ReadOnlySpan<byte> buffer)
        => MemoryMarshal.Cast<byte, char>(buffer).ReadNullTerminatedUnicodeString();

    /// <summary>
    /// Reads null terminated Unicode string from char buffer.
    /// </summary>
    /// <param name="chars">Buffer</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadNullTerminatedUnicodeString(this ReadOnlySpan<char> chars)
    {
        var endpos = chars.IndexOfTerminator();
        return chars.Slice(0, endpos).ToString();
    }

    /// <summary>
    /// Reads null terminated Unicode string from char buffer.
    /// </summary>
    /// <param name="chars">Buffer</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadNullTerminatedUnicodeString(this Span<char> chars)
    {
        var endpos = chars.IndexOfTerminator();
        return chars.Slice(0, endpos).ToString();
    }

    /// <summary>
    /// Reads null terminated Unicode string from char buffer.
    /// </summary>
    /// <param name="chars">Buffer</param>
    /// <returns>Memory region up to null terminator</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<char> ReadNullTerminatedUnicodeString(this ReadOnlyMemory<char> chars)
    {
        var endpos = chars.Span.IndexOfTerminator();
        return chars.Slice(0, endpos);
    }

    /// <summary>
    /// Reads null terminated Unicode string from char buffer.
    /// </summary>
    /// <param name="chars">Buffer</param>
    /// <returns>Memory region up to null terminator</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<char> ReadNullTerminatedUnicodeString(this Memory<char> chars)
    {
        var endpos = chars.Span.IndexOfTerminator();
        return chars.Slice(0, endpos);
    }

    /// <summary>
    /// Converts first character of each word to uppercase
    /// </summary>
    /// <param name="str">Source sring</param>
    /// <param name="minWordLength">Minimum length of words to apply uppercase initial</param>
    /// <returns>Converted string</returns>
    public static string InitialCapital(this ReadOnlyMemory<char> str, int minWordLength)
    {
        if (str.IsEmpty)
        {
            return string.Empty;
        }

        if (minWordLength < 2)
        {
            minWordLength = 2;
        }

        var words = str.Split(' ').Select(word =>
        {
            if (MemoryMarshal.ToEnumerable(word).Any(char.IsLower))
            {
                return word;
            }

            if (word.Length >= minWordLength)
            {
                return $"{char.ToUpper(word.Span[0])}{word.Slice(1).ToString().ToLower()}".AsMemory();
            }

            return word;
        });

        return string.Join(" ", words);
    }

#endif

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP

    /// <summary>
    /// Reads null terminated ASCII string from byte buffer.
    /// </summary>
    /// <param name="buffer">Byte buffer</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadNullTerminatedAsciiString(this ReadOnlySpan<byte> buffer)
    {
        var endpos = buffer.IndexOfTerminator();
        return Encoding.ASCII.GetString(buffer[..endpos]);
    }

    /// <summary>
    /// Reads null terminated ASCII string from byte buffer.
    /// </summary>
    /// <param name="buffer">Byte buffer</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadNullTerminatedAsciiString(this Span<byte> buffer)
    {
        var endpos = buffer.IndexOfTerminator();
        return Encoding.ASCII.GetString(buffer[..endpos]);
    }

#elif NET45_OR_GREATER || NETSTANDARD

    /// <summary>
    /// Reads null terminated ASCII string from byte buffer.
    /// </summary>
    /// <param name="buffer">Byte buffer</param>
    /// <returns>Managed string</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadNullTerminatedAsciiString(this ReadOnlySpan<byte> buffer)
    {
        var endpos = buffer.IndexOfTerminator();
        return Encoding.ASCII.GetString(buffer.Slice(0, endpos).ToArray());
    }

#endif

#if !NET6_0_OR_GREATER
    /// <summary>Returns the maximum value in a generic sequence according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <typeparam name="TKey">The type of key to compare elements by.</typeparam>
    /// <param name="source">A sequence of values to determine the maximum value of.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>The value with the maximum key in the sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">No key extracted from <paramref name="source" /> implements the <see cref="IComparable" /> or <see cref="System.IComparable{TKey}" /> interface.</exception>
    /// <remarks>
    /// <para>If <typeparamref name="TKey" /> is a reference type and the source sequence is empty or contains only values that are <see langword="null" />, this method returns <see langword="null" />.</para>
    /// </remarks>
    public static TSource? MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        => source.OrderByDescending(keySelector).FirstOrDefault();

    /// <summary>Returns the maximum value in a generic sequence according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <typeparam name="TKey">The type of key to compare elements by.</typeparam>
    /// <param name="source">A sequence of values to determine the maximum value of.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">The <see cref="IComparer{TKey}" /> to compare keys.</param>
    /// <returns>The value with the maximum key in the sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">No key extracted from <paramref name="source" /> implements the <see cref="IComparable" /> or <see cref="IComparable{TKey}" /> interface.</exception>
    /// <remarks>
    /// <para>If <typeparamref name="TKey" /> is a reference type and the source sequence is empty or contains only values that are <see langword="null" />, this method returns <see langword="null" />.</para>
    /// </remarks>
    public static TSource? MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
        => source.OrderByDescending(keySelector, comparer).FirstOrDefault();

    /// <summary>
    /// </summary>
    public static TSource? MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        => source.OrderBy(keySelector).FirstOrDefault();

    /// <summary>Returns the maximum value in a generic sequence according to a specified key selector function.</summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
    /// <typeparam name="TKey">The type of key to compare elements by.</typeparam>
    /// <param name="source">A sequence of values to determine the maximum value of.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <param name="comparer">The <see cref="IComparer{TKey}" /> to compare keys.</param>
    /// <returns>The value with the maximum key in the sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
    /// <exception cref="ArgumentException">No key extracted from <paramref name="source" /> implements the <see cref="IComparable" /> or <see cref="IComparable{TKey}" /> interface.</exception>
    /// <remarks>
    /// <para>If <typeparamref name="TKey" /> is a reference type and the source sequence is empty or contains only values that are <see langword="null" />, this method returns <see langword="null" />.</para>
    /// </remarks>
    public static TSource? MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IComparer<TKey>? comparer)
        => source.OrderBy(keySelector, comparer).FirstOrDefault();
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// </summary>
    public static int GetSequenceHash(this ReadOnlySpan<byte> str)
    {
        var hash = new HashCode();
        hash.AddBytes(str);
        return hash.ToHashCode();
    }
#endif

#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP
    /// <summary>
    /// </summary>
    public static int GetSequenceHash<T>(this ReadOnlySpan<T> str)
    {
        var hash = new HashCode();
        foreach (var element in str)
        {
            hash.Add(element);
        }

        return hash.ToHashCode();
    }

    /// <summary>
    /// </summary>
    public static int GetSequenceHash<T>(this Span<T> str)
    {
        var hash = new HashCode();
        foreach (var element in str)
        {
            hash.Add(element);
        }

        return hash.ToHashCode();
    }

    /// <summary>
    /// </summary>
    public static int GetSequenceHash<T>(this IEnumerable<T> str)
    {
        var hash = new HashCode();
        foreach (var element in str)
        {
            hash.Add(element);
        }

        return hash.ToHashCode();
    }
#endif

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    /// <summary>
    /// </summary>
    public static T[] RentArray<T>(int size) => System.Buffers.ArrayPool<T>.Shared.Rent(size);

    /// <summary>
    /// </summary>
    public static void ReturnArray<T>(T[] array) => System.Buffers.ArrayPool<T>.Shared.Return(array);
#else
    /// <summary>
    /// </summary>
    public static T[] RentArray<T>(int size) => new T[size];

    /// <summary>
    /// </summary>
    public static void ReturnArray<T>(T[] _) { }
#endif
}
