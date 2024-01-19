#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LTRData.Extensions.Split;

/// <summary>
/// Extension methods for splitting <see cref="ReadOnlySpan{Char}"/> or <see cref="Span{Char}"/> in allocation-free iterations,
/// or splitting <see cref="ReadOnlyMemory{Char}"/> or <see cref="Memory{Char}"/> without allocating arrays.
/// </summary>
public static class SpanSplitExtensions
{
    /// <summary>
    /// Returns an enumerator for each delimited token in a span of characters
    /// </summary>
    /// <param name="chars">Character span to search</param>
    /// <param name="delimiter">Delimiter between each token</param>
    /// <param name="options"><see cref="StringSplitOptions"/> options to apply to search</param>
    /// <returns>Reference enumerator</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringSplitByCharEnumerator Split(this ReadOnlySpan<char> chars, char delimiter, StringSplitOptions options = StringSplitOptions.None) =>
        new(chars, delimiter, options, reverse: false);

    /// <summary>
    /// Returns an enumerator for each delimited token in a span of characters
    /// </summary>
    /// <param name="chars">Character span to search</param>
    /// <param name="delimiter">Delimiter between each token</param>
    /// <param name="options"><see cref="StringSplitOptions"/> options to apply to search</param>
    /// <returns>Reference enumerator</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringSplitByStringEnumerator Split(this ReadOnlySpan<char> chars, ReadOnlySpan<char> delimiter, StringSplitOptions options = StringSplitOptions.None) =>
        new(chars, delimiter, options, reverse: false);

    /// <summary>
    /// Returns a reverse enumerator for each delimited token in a span of characters
    /// </summary>
    /// <param name="chars">Character span to search</param>
    /// <param name="delimiter">Delimiter between each token</param>
    /// <param name="options"><see cref="StringSplitOptions"/> options to apply to search</param>
    /// <returns>Reference enumerator</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringSplitByCharEnumerator SplitReverse(this ReadOnlySpan<char> chars, char delimiter, StringSplitOptions options = StringSplitOptions.None) =>
        new(chars, delimiter, options, reverse: true);

    /// <summary>
    /// Returns a reverse enumerator for each delimited token in a span of characters
    /// </summary>
    /// <param name="chars">Character span to search</param>
    /// <param name="delimiter">Delimiter between each token</param>
    /// <param name="options"><see cref="StringSplitOptions"/> options to apply to search</param>
    /// <returns>Reference enumerator</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StringSplitByStringEnumerator SplitReverse(this ReadOnlySpan<char> chars, ReadOnlySpan<char> delimiter, StringSplitOptions options = StringSplitOptions.None) =>
        new(chars, delimiter, options, reverse: true);

    /// <summary>
    /// Returns an enumerator for each delimited token in a block of memory
    /// </summary>
    /// <param name="chars">Character memory block to search</param>
    /// <param name="delimiter">Delimiter between each token</param>
    /// <param name="options"><see cref="StringSplitOptions"/> options to apply to search</param>
    /// <returns>Iterator</returns>
    public static IEnumerable<ReadOnlyMemory<char>> Split(this ReadOnlyMemory<char> chars, char delimiter, StringSplitOptions options = StringSplitOptions.None)
    {
        do {
            var i = chars.Span.IndexOf(delimiter);
            if (i < 0)
            {
                i = chars.Length;
            }

            var value = chars.Slice(0, i);

#if NET5_0_OR_GREATER
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                value = value.Trim();
            }
#endif

            if (!options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
            {
                yield return value;
            }

            if (i >= chars.Length)
            {
                break;
            }

            chars = chars.Slice(i + 1);
        } while (!chars.IsEmpty);
    }

    /// <summary>
    /// Returns an enumerator for each delimited token in a block of memory
    /// </summary>
    /// <param name="chars">Character memory block to search</param>
    /// <param name="delimiter1">First of two possible delimiters between each token</param>
    /// <param name="delimiter2">Second of two possible delimiters between each token</param>
    /// <param name="options"><see cref="StringSplitOptions"/> options to apply to search</param>
    /// <returns>Iterator</returns>
    public static IEnumerable<ReadOnlyMemory<char>> Split(this ReadOnlyMemory<char> chars, char delimiter1, char delimiter2, StringSplitOptions options = StringSplitOptions.None)
    {
        do {
            var i = chars.Span.IndexOfAny(delimiter1, delimiter2);
            if (i < 0)
            {
                i = chars.Length;
            }

            var value = chars.Slice(0, i);

#if NET5_0_OR_GREATER
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                value = value.Trim();
            }
#endif

            if (!options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
            {
                yield return value;
            }

            if (i >= chars.Length)
            {
                break;
            }

            chars = chars.Slice(i + 1);
        } while (!chars.IsEmpty);
    }

    /// <summary>
    /// Returns a reverse enumerator for each delimited token in a block of memory
    /// </summary>
    /// <param name="chars">Character memory block to search</param>
    /// <param name="delimiter">Delimiter between each token</param>
    /// <param name="options"><see cref="StringSplitOptions"/> options to apply to search</param>
    /// <returns>Iterator</returns>
    public static IEnumerable<ReadOnlyMemory<char>> SplitReverse(this ReadOnlyMemory<char> chars, char delimiter, StringSplitOptions options = StringSplitOptions.None)
    {
        do {
            var i = chars.Span.LastIndexOf(delimiter);

            var value = chars.Slice(i + 1);

#if NET5_0_OR_GREATER
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                value = value.Trim();
            }
#endif

            if (!options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
            {
                yield return value;
            }

            if (i < 0)
            {
                break;
            }

            chars = chars.Slice(0, i);
        } while (!chars.IsEmpty);
    }

    /// <summary>
    /// Returns an enumerator for each delimited token in a block of memory
    /// </summary>
    /// <param name="chars">Character memory block to search</param>
    /// <param name="delimiter">Delimiter between each token</param>
    /// <param name="options"><see cref="StringSplitOptions"/> options to apply to search</param>
    /// <returns>Iterator</returns>
    public static IEnumerable<ReadOnlyMemory<char>> Split(this ReadOnlyMemory<char> chars, ReadOnlyMemory<char> delimiter, StringSplitOptions options = StringSplitOptions.None)
    {
        do {
            var i = chars.Span.IndexOf(delimiter.Span);
            if (i < 0)
            {
                i = chars.Length;
            }

            var value = chars.Slice(0, i);

#if NET5_0_OR_GREATER
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                value = value.Trim();
            }
#endif

            if (!options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
            {
                yield return value;
            }

            if (i >= chars.Length)
            {
                break;
            }

            chars = chars.Slice(i + delimiter.Length);
        } while (!chars.IsEmpty);
    }

    /// <summary>
    /// Returns a reverse enumerator for each delimited token in a block of memory
    /// </summary>
    /// <param name="chars">Character memory block to search</param>
    /// <param name="delimiter">Delimiter between each token</param>
    /// <param name="options"><see cref="StringSplitOptions"/> options to apply to search</param>
    /// <returns>Iterator</returns>
    public static IEnumerable<ReadOnlyMemory<char>> SplitReverse(this ReadOnlyMemory<char> chars, ReadOnlyMemory<char> delimiter, StringSplitOptions options = StringSplitOptions.None)
    {
        do {
            var i = chars.Span.LastIndexOf(delimiter.Span);

            var value = i >= 0 ? chars.Slice(i + delimiter.Length) : chars;

#if NET5_0_OR_GREATER
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                value = value.Trim();
            }
#endif

            if (!options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
            {
                yield return value;
            }

            if (i < 0)
            {
                break;
            }

            chars = chars.Slice(0, i);
        } while (!chars.IsEmpty);
    }

    /// <summary>
    /// Returns an enumerator for each delimited token in a block of memory
    /// </summary>
    /// <param name="chars">Character memory block to search</param>
    /// <param name="delimiter">Delimiter between each token</param>
    /// <param name="options"><see cref="StringSplitOptions"/> options to apply to search</param>
    /// <param name="comparison">Comparison used when searching for delimiters</param>
    /// <returns>Iterator</returns>
    public static IEnumerable<ReadOnlyMemory<char>> Split(this ReadOnlyMemory<char> chars, ReadOnlyMemory<char> delimiter, StringSplitOptions options, StringComparison comparison)
    {
        do {
            var i = chars.Span.IndexOf(delimiter.Span, comparison);
            if (i < 0)
            {
                i = chars.Length;
            }

            var value = chars.Slice(0, i);

#if NET5_0_OR_GREATER
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                value = value.Trim();
            }
#endif

            if (!options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
            {
                yield return value;
            }

            if (i >= chars.Length)
            {
                break;
            }

            chars = chars.Slice(i + delimiter.Length);
        } while (!chars.IsEmpty);
    }
}

#endif
