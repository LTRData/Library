#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LTRData.Net;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class DownloadCache
{
    private static HttpClient HttpClient => field ??= new();

    private static ConcurrentDictionary<string, Task<byte[]>> Cache => field ??= new();

    public static async Task<byte[]?> DownloadAndCacheDataAsync(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        try
        {
            return await Cache.GetOrAdd(str, HttpClient.GetByteArrayAsync).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to download image {str}: {ex}");
            Cache.TryRemove(str, out _);
            return null;
        }
    }
}

#endif
