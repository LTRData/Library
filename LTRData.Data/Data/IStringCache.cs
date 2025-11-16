// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace LTRData.Data;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IStringCache : ICloneable, IDisposable
{
    ValueTask<int> RemoveItemAsync(string key, CancellationToken cancellationToken);

    ValueTask<int> RemoveAllAsync(CancellationToken cancellationToken);

    ValueTask<int> CleanAsync(CancellationToken cancellationToken);

    [ComVisible(false)]
    ValueTask<int> SetItemAsync(string key, string? data, DateTime? expirydate, CancellationToken cancellationToken);

    ValueTask<string?> GetItemAsync(string key, CancellationToken cancellationToken);
    
    new IStringCache Clone();
}

#endif
