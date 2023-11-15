using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Threading.Tasks;
#endif

namespace LTRData.Extensions.IO;

/// <summary>
/// </summary>
public static class IOExtensions
{
    /// <summary>
    /// </summary>
#if NET5_0_OR_GREATER
    public static bool IsWindows { get; } = OperatingSystem.IsWindows();
#elif NET471_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static bool IsWindows { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
    public static bool IsWindows { get; } = true;
#endif

#if (NET45_OR_GREATER || NETSTANDARD || NETCOREAPP) && !NET7_0_OR_GREATER
    /// <summary>
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<string?> ReadLineAsync(this TextReader reader, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return reader.ReadLineAsync();
    }

    /// <summary>
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="str"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task WriteLineAsync(this TextWriter writer, ReadOnlyMemory<char> str, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return MemoryMarshal.TryGetString(str, out var text, out int start, out int length) && start == 0 && length == text.Length
            ? writer.WriteLineAsync(text)
            : MemoryMarshal.TryGetArray(str, out var segment)
            ? writer.WriteLineAsync(segment.Array!, segment.Offset, segment.Count)
            : writer.WriteLineAsync(str.ToString());
    }
#endif

    /// <summary>
    /// Reads lines from a <see cref="TextReader"/> as an <see cref="IEnumerable{String}"/>.
    /// </summary>
    /// <param name="reader"><see cref="TextReader"/> object to read</param>
    /// <returns>Enumeration of lines</returns>
    public static IEnumerable<string> EnumerateLines(this TextReader reader)
    {
        for (
            var line = reader.ReadLine();
            line is not null;
            line = reader.ReadLine())
        {
            yield return line;
        }
    }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    /// <summary>
    /// Asynchronously reads lines from a <see cref="TextReader"/> as an <see cref="IAsyncEnumerable{String}"/>.
    /// </summary>
    /// <param name="reader"><see cref="TextReader"/> object to read</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Enumeration of lines</returns>
    public static async IAsyncEnumerable<string> AsyncEnumerateLines(this TextReader reader, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false) is { } line)
        {
            yield return line;
        }
    }
#endif
}
