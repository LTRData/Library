using System;
using System.IO;
using SkiaSharp;

namespace LTRData.Graphics.SkiaSharp;

/// <summary>PNG encoding separate from diagram/plot rendering.</summary>
public static class SkiaPngEncoder
{
    /// <summary>Writes a PNG at the destination's current position, leaving both inputs open.</summary>
    public static void Encode(SKImage image, Stream destination)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(destination);
        if (!destination.CanWrite) throw new ArgumentException("The destination must be writable.", nameof(destination));
        using var data = image.Encode(SKEncodedImageFormat.Png, 100)
            ?? throw new InvalidOperationException("Skia could not encode the image as PNG.");
        data.SaveTo(destination);
    }
}
