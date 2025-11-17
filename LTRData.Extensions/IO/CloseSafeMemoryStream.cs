// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System.IO;

namespace LTRData.Extensions.IO;

/// <summary>
/// A <see cref="MemoryStream"/> where contents can be accessed
/// even after dispose.
/// </summary>
public sealed class CloseSafeMemoryStream : MemoryStream
{
    /// <summary>
    /// Ignored
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        Position = 0L;
        base.Dispose(false);
    }
}

