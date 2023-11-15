#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP
using LTRData.Extensions.Buffers;
using LTRData.Extensions.Split;
#endif
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;

namespace LTRData.Extensions.Formatting;

/// <summary>
/// </summary>
public static class StringFormatting
{

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// Capitalize first character in each word.
    /// </summary>
    /// <param name="str">String with words</param>
    /// <param name="CultureInfo"></param>
    /// <param name="MinWordLength"></param>
    /// <returns></returns>
    public static string InitialCapital(this string str, CultureInfo CultureInfo, int MinWordLength)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        if (MinWordLength < 2)
        {
            MinWordLength = 2;
        }

        var words = str.Split(' ');
        for (int i = 0, loopTo = words.GetUpperBound(0); i <= loopTo; i++)
        {
            if (words[i].Any(char.IsLower))
            {
                continue;
            }

            if (words[i].Length >= MinWordLength)
            {
                words[i] = char.ToUpper(words[i][0], CultureInfo) + words[i].Substring(1).ToLower(CultureInfo);
            }
        }

        str = string.Join(" ", words);

        return str;
    }

    /// <summary>
    /// Capitalize first character in each word.
    /// </summary>
    /// <param name="str">String with words</param>
    /// <param name="MinWordLength"></param>
    /// <returns></returns>
    public static string InitialCapitalInvariant(this string str, int MinWordLength)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        if (MinWordLength < 2)
        {
            MinWordLength = 2;
        }

        var words = str.Split(' ');
        for (int i = 0, loopTo = words.GetUpperBound(0); i <= loopTo; i++)
        {
            if (words[i].Any(char.IsLower))
            {
                continue;
            }

            if (words[i].Length >= MinWordLength)
            {
                words[i] = char.ToUpperInvariant(words[i][0]) + words[i].Substring(1).ToLowerInvariant();
            }
        }

        str = string.Join(" ", words);

        return str;
    }

    /// <summary>
    /// Capitalize first character in each word.
    /// </summary>
    /// <param name="str">String with words</param>
    /// <param name="MinWordLength"></param>
    /// <returns></returns>
    public static string InitialCapital(this string str, int MinWordLength)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        if (MinWordLength < 2)
        {
            MinWordLength = 2;
        }

        var words = str.Split(' ');
        for (int i = 0, loopTo = words.GetUpperBound(0); i <= loopTo; i++)
        {
            if (words[i].Any(char.IsLower))
            {
                continue;
            }

            if (words[i].Length >= MinWordLength)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }
        }

        str = string.Join(" ", words);

        return str;
    }

    /// <summary>
    /// Capitalize first character in each word and joins result as space-delimited string.
    /// </summary>
    /// <param name="words">String with words</param>
    /// <param name="MinWordLength"></param>
    /// <returns></returns>
    public static string InitialCapitalInvariantJoin(this IEnumerable<string> words, int MinWordLength)
    {
        if (words is null)
        {
            return "";
        }

        if (MinWordLength < 2)
        {
            MinWordLength = 2;
        }

        return (from word in words
                select word.Any(char.IsLower) || word.Length < MinWordLength ? word : char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant()).Concat();
    }

    /// <summary>
    /// Capitalize first character in each word and joins result as space-delimited string.
    /// </summary>
    /// <param name="words">String with words</param>
    /// <param name="MinWordLength"></param>
    /// <returns></returns>
    public static string InitialCapitalJoin(this IEnumerable<string> words, int MinWordLength)
    {
        if (words is null)
        {
            return "";
        }

        if (MinWordLength < 2)
        {
            MinWordLength = 2;
        }

        return (from word in words
                select word.Any(char.IsLower) || word.Length < MinWordLength ? word : char.ToUpper(word[0]) + word.Substring(1).ToLower()).Concat();
    }

#endif

#if NETFRAMEWORK || (NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)

    /// <summary>
    /// </summary>
    /// <param name="encoding"></param>
    /// <param name="s"></param>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static byte[] GetBytes(this Encoding encoding, string s, int index, int count)
        => encoding.GetBytes(s.ToCharArray(index, count));

#endif

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

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// Adjusts a string to a display with specified width by wrapping complete words.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="indentWidth"></param>
    /// <param name="lineWidth">Display width. If omitted, defaults to console window width.</param>
    /// <param name="wordDelimiter">Word separator character.</param>
    /// <param name="fillChar"></param>
    [SecuritySafeCritical]
    public static string LineFormat(ReadOnlySpan<char> msg,
                                    int indentWidth = 0,
                                    int? lineWidth = default,
                                    char wordDelimiter = ' ',
                                    char fillChar = ' ')
    {
        if (msg.IsWhiteSpace())
        {
            return "";
        }

        int width;

        if (lineWidth.HasValue)
        {
            width = lineWidth.Value;
        }

        else if (Console.IsOutputRedirected)
        {
            width = 79;
        }
        else
        {
            width = Console.WindowWidth - 1;

        }

        var resultLines = new List<string>();

        foreach (var origLinePtr in msg.Split('\n'))
        {
            var origLine = origLinePtr.TrimEnd('\r');

            var result = new StringBuilder();

            var line = new StringBuilder(width);

            foreach (var word in origLine.Split(wordDelimiter))
            {
                if (word.Length >= width)
                {
                    result.Append(word);
                    result.AppendLine();
                    continue;
                }

                if (word.Length + line.Length >= width)
                {
                    result.AppendLine(line.ToString());
                    line.Length = 0;
                    line.Append(fillChar, indentWidth);
                }

                if (line.Length > 0)
                {
                    line.Append(wordDelimiter);
                }

                line.Append(word);
            }

            if (line.Length > 0)
            {
                result.Append(line);
            }

            resultLines.Add(result.ToString());
        }

        return string.Join(Environment.NewLine, resultLines);
    }

    /// <summary>
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="width"></param>
    /// <param name="fillChar"></param>
    /// <returns></returns>
    public static string Center(this string? msg, int width, char fillChar)
    {
        if (msg is null || string.IsNullOrEmpty(msg))
        {
            return new string(fillChar, width);
        }
        else if (msg.Length == width)
        {
            return msg;
        }
        else if (msg.Length > width)
        {
            return msg.Substring((msg.Length - width) / 2, width);
        }
        else
        {
            var FillStr = new string(fillChar, (width - msg.Length) / 2);

            return $"{FillStr}{msg}{FillStr}";
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static string Center(this string msg, int width) => Center(msg, width, ' ');

    /// <summary>
    /// Left adjusts a string by truncating or padding to ensure that the resulting string is
    /// exactly the specified length
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="width"></param>
    /// <param name="fillChar"></param>
    public static string LeftAdjust(this string? msg, int width, char fillChar)
        => (msg ?? "").PadRight(width, fillChar).Substring(0, width);

    /// <summary>
    /// Left adjusts a string by truncating or padding to ensure that the resulting string is
    /// exactly the specified length
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="width"></param>
    public static string LeftAdjust(this string? msg, int width)
        => (msg ?? "").PadRight(width).Substring(0, width);

    /// <summary>
    /// Right adjusts a string by truncating or padding to ensure that the resulting string is
    /// exactly the specified length
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="width"></param>
    /// <param name="fillChar"></param>
    public static string RightAdjust(this string? msg, int width, char fillChar)
    {
        var padded = (msg ?? "").PadLeft(width, fillChar);
        return padded.Substring(padded.Length - width);
    }

    /// <summary>
    /// Right adjusts a string by truncating or padding to ensure that the resulting string is
    /// exactly the specified length
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="width"></param>
    public static string RightAdjust(this string? msg, int width)
    {
        var padded = (msg ?? "").PadLeft(width);
        return padded.Substring(padded.Length - width);
    }

    /// <summary>
    /// Adjusts a string to a display with specified width and height by truncating or padding to
    /// fill each line with complete words and spaces. This method ensures that the resulting
    /// string is exactly height * width characters long.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="width">Display width</param>
    /// <param name="height">Display height</param>
    /// <param name="wordDelimiter">Word separator character.</param>
    /// <param name="fillChar">Fill character.</param>
    public static string BoxFormat(ReadOnlySpan<char> msg, int width, int height, char wordDelimiter = ' ', char fillChar = ' ')
    {
        var result = new StringBuilder(height * width);
        var line = new StringBuilder(width);

        foreach (var wordPtr in msg.Split(wordDelimiter))
        {
            var word = wordPtr;

            if (word.Length > width)
            {
                word = word.Slice(0, width);
            }

            if (word.Length + line.Length > width)
            {
                result.Append(line.ToString().PadRight(width, fillChar));
                line.Length = 0;
            }

            line.Append(word);
            if (line.Length >= width)
            {
                result.Append(LeftAdjust(line.ToString(), width, fillChar));
                line.Length = 0;
            }
            else
            {
                line.Append(wordDelimiter);
            }
        }

        result.Append(line);

        return LeftAdjust(result.ToString(), height * width, fillChar);
    }
#endif
}
