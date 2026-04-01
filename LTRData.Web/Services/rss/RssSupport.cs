#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP

using LTRData.Xml;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LTRData.Services.rss;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class RssSupport
{
    private static HttpClient HttpClient => field ??= new();

    public static async Task<rss?> DownloadAsync(string url)
    {
        return XmlSupport.XmlDeserialize<rss>(await HttpClient.GetByteArrayAsync(url).ConfigureAwait(false));
    }

    public static async Task<rss?> DownloadAsync(Uri url)
    {
        return XmlSupport.XmlDeserialize<rss>(await HttpClient.GetByteArrayAsync(url).ConfigureAwait(false));
    }
}

#endif

