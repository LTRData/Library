using System;
using LTRData.FunctionPlotting;
using SkiaSharp;

namespace LTRData.Graphics.SkiaSharp;

/// <summary>Backend-specific styling for Cartesian axes and a curve.</summary>
public sealed record SkiaPlotTheme
{
    /// <summary>Default plot styling.</summary>
    public static SkiaPlotTheme Default { get; } = new();
    /// <summary>Curve stroke color.</summary>
    public SKColor CurveColor { get; init; } = new(30, 88, 175);
    /// <summary>Axis stroke color.</summary>
    public SKColor AxisColor { get; init; } = new(155, 163, 175);
    /// <summary>Positive curve stroke width in canvas units.</summary>
    public float StrokeWidth { get; init; } = 2;
}

/// <summary>Draws existing plot geometry without sampling, clearing, or retaining paths.</summary>
public static class SkiaPlotRenderer
{
    /// <summary>Draws one curve. Call repeatedly to compose explicit overlays.</summary>
    public static void Render(SKCanvas canvas, CurveGeometry geometry, SkiaPlotTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        ArgumentNullException.ThrowIfNull(geometry);
        theme ??= SkiaPlotTheme.Default;
        if (!float.IsFinite(theme.StrokeWidth) || theme.StrokeWidth <= 0)
            throw new ArgumentOutOfRangeException(nameof(theme));
        using var paint = new SKPaint
        {
            Color = theme.CurveColor, Style = SKPaintStyle.Stroke,
            StrokeWidth = theme.StrokeWidth, IsAntialias = true
        };
        using var path = new SKPathBuilder();
        foreach (var line in geometry.Polylines)
        {
            path.Reset();
            for (var i = 0; i < line.Points.Count; i++)
            {
                var point = line.Points[i];
                if (!double.IsFinite(point.X) || !double.IsFinite(point.Y) ||
                    Math.Abs(point.X) > float.MaxValue || Math.Abs(point.Y) > float.MaxValue)
                    throw new ArgumentException("Geometry contains an unrenderable coordinate.", nameof(geometry));
                if (i == 0) path.MoveTo((float)point.X, (float)point.Y);
                else path.LineTo((float)point.X, (float)point.Y);
            }
            using var curve = path.Detach();
            canvas.DrawPath(curve, paint);
        }
    }

    /// <summary>Draws only axes that intersect the viewport, without changing canvas state.</summary>
    public static void RenderAxes(SKCanvas canvas, PlotViewport viewport, SkiaPlotTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        ArgumentNullException.ThrowIfNull(viewport);
        if (viewport.Canvas.Width > float.MaxValue || viewport.Canvas.Height > float.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(viewport));
        theme ??= SkiaPlotTheme.Default;
        using var paint = new SKPaint { Color = theme.AxisColor, StrokeWidth = 1, IsAntialias = true };
        // Compute each axis independently: an offscreen other coordinate need not be representable.
        if (viewport.XRange.Minimum <= 0 && viewport.XRange.Maximum >= 0)
        {
            var x = (float)(-viewport.XRange.Minimum / viewport.XRange.Length * viewport.Canvas.Width);
            canvas.DrawLine(x, 0, x, (float)viewport.Canvas.Height, paint);
        }
        if (viewport.YRange.Minimum <= 0 && viewport.YRange.Maximum >= 0)
        {
            var y = (float)(viewport.YRange.Maximum / viewport.YRange.Length * viewport.Canvas.Height);
            canvas.DrawLine(0, y, (float)viewport.Canvas.Width, y, paint);
        }
    }
}
