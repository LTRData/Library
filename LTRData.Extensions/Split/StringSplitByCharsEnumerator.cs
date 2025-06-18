#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;

namespace LTRData.Extensions.Split;

/// <summary>
/// Ref enumerator used to enumerate tokens in a <see cref="ReadOnlySpan{Char}"/> or <see cref="Span{Char}"/>
/// </summary>
public ref struct StringSplitByCharsEnumerator
{
    private readonly char delimiter1;
    private readonly char delimiter2;
    private readonly StringSplitOptions options;
    private readonly bool reverse;

    private ReadOnlySpan<char> chars;

    /// <summary>
    /// Current token in enumeration
    /// </summary>
    public ReadOnlySpan<char> Current { get; private set; }

    /// <summary>
    /// Moves to next token in enumeration
    /// </summary>
    /// <returns>True if another token was found, false otherwise</returns>
    public bool MoveNext() => reverse ? MoveNextReverse() : MoveNextForward();

    /// <summary>
    /// Moves to next token in forward order
    /// </summary>
    /// <returns>True if another token was found, false otherwise</returns>
    public bool MoveNextForward()
    {
        while (!chars.IsEmpty)
        {

            var i = chars.IndexOfAny(delimiter1, delimiter2);
            if (i < 0)
            {
                i = chars.Length;
            }

            Current = chars.Slice(0, i);

            if (i < chars.Length)
            {
                chars = chars.Slice(i + 1);
            }
            else
            {
                chars = default;
            }

#if NET5_0_OR_GREATER
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                Current = Current.Trim();
            }
#endif

            if (!Current.IsEmpty ||
                !options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Moves to next token in reverse order
    /// </summary>
    /// <returns>True if another token was found, false otherwise</returns>
    public bool MoveNextReverse()
    {
        while (!chars.IsEmpty)
        {
            var i = chars.LastIndexOfAny(delimiter1, delimiter2);

            Current = i >= 0 ? chars.Slice(i + 1) : chars;

            if (i < 0)
            {
                chars = default;
            }
            else
            {
                chars = chars.Slice(0, i);
            }

#if NET5_0_OR_GREATER
            if (options.HasFlag(StringSplitOptions.TrimEntries))
            {
                Current = Current.Trim();
            }
#endif

            if (!Current.IsEmpty ||
                !options.HasFlag(StringSplitOptions.RemoveEmptyEntries))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns first token in enumeration
    /// </summary>
    /// <returns>First token in enumeration</returns>
    /// <exception cref="InvalidOperationException">Span is empty</exception>
    public ReadOnlySpan<char> First() => MoveNext() ? Current : throw new InvalidOperationException();

    /// <summary>
    /// Returns first token in enumeration or empty span if source is empty
    /// </summary>
    /// <returns>First token in enumeration or empty span if source is empty</returns>
    public ReadOnlySpan<char> FirstOrDefault() => MoveNext() ? Current : default;

    /// <summary>
    /// Returns last token in enumeration
    /// </summary>
    /// <returns>Last token in enumeration</returns>
    /// <exception cref="InvalidOperationException">Span is empty</exception>
    public ReadOnlySpan<char> Last()
    {
        var found = false;
        ReadOnlySpan<char> result = default;

        while (MoveNext())
        {
            found = true;
            result = Current;
        }

        if (found)
        {
            return result;
        }

        throw new InvalidOperationException();
    }

    /// <summary>
    /// Returns last token in enumeration or empty span if source is empty
    /// </summary>
    /// <returns>Last token in enumeration or empty span if source is empty</returns>
    public ReadOnlySpan<char> LastOrDefault()
    {
        ReadOnlySpan<char> result = default;

        while (MoveNext())
        {
            result = Current;
        }

        return result;
    }

    /// <summary>
    /// Returns zero-based token in enumeration
    /// </summary>
    /// <param name="pos">Zero-based index of token to return</param>
    /// <returns>Zero-based token in enumeration</returns>
    /// <exception cref="ArgumentOutOfRangeException">Source span contains fewer tokens than the number requested</exception>
    public ReadOnlySpan<char> ElementAt(int pos)
    {
        for (var i = 0; i <= pos; i++)
        {
            if (!MoveNext())
            {
                throw new ArgumentOutOfRangeException(nameof(pos));
            }
        }

        return Current;
    }

    /// <summary>
    /// Returns zero-based token in enumeration or empty span if source is empty
    /// </summary>
    /// <param name="pos">Zero-based index of token to return</param>
    /// <returns>Zero-based token in enumeration or empty span if source is empty</returns>
    public ReadOnlySpan<char> ElementAtOrDefault(int pos)
    {
        for (var i = 0; i <= pos; i++)
        {
            if (!MoveNext())
            {
                return default;
            }
        }

        return Current;
    }

    /// <summary>
    /// Gets this enumerator
    /// </summary>
    /// <returns>This enumerator</returns>
    public readonly StringSplitByCharsEnumerator GetEnumerator() => this;

    /// <summary>
    /// Constructs an token enumerator over a span
    /// </summary>
    /// <param name="chars">Span to search</param>
    /// <param name="delimiter1">Delimiter alt 1 between each token</param>
    /// <param name="delimiter2">Delimiter alt 2 between each token</param>
    /// <param name="options"><see cref="StringSplitOptions"/> options to apply to search</param>
    /// <param name="reverse">Selects reverse or forward order</param>
    public StringSplitByCharsEnumerator(ReadOnlySpan<char> chars, char delimiter1, char delimiter2, StringSplitOptions options, bool reverse)
    {
        Current = default;
        this.chars = chars;
        this.delimiter1 = delimiter1;
        this.delimiter2 = delimiter2;
        this.options = options;
        this.reverse = reverse;
    }
}

#endif
