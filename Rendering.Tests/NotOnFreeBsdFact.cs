using System;
using Xunit;

namespace LTRData.Rendering.Tests;

/// <summary>
/// Skips native SkiaSharp rendering tests on FreeBSD, where compatible native assets are unavailable.
/// </summary>
public sealed class NotOnFreeBsdFactAttribute : FactAttribute
{
    public NotOnFreeBsdFactAttribute()
    {
        if (OperatingSystem.IsFreeBSD())
        {
            Skip = "SkiaSharp native assets used by the rendering tests are not supported on FreeBSD.";
        }
    }
}
