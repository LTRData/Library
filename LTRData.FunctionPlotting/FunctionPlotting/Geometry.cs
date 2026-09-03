using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LTRData.FunctionPlotting;

#pragma warning disable CS1591

public struct CanvasSize : IEquatable<CanvasSize>
{
    public CanvasSize(double width, double height)
    {
        if (!NumericRange.IsFinite(width) || width <= 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(width),
                "Canvas width must be finite and positive.");
        }

        if (!NumericRange.IsFinite(height) || height <= 0d)
        {
            throw new ArgumentOutOfRangeException(nameof(height),
                "Canvas height must be finite and positive.");
        }

        Width = width;
        Height = height;
    }

    public double Width { get; }

    public double Height { get; }

    internal bool IsValid => NumericRange.IsFinite(Width) && Width > 0d &&
        NumericRange.IsFinite(Height) && Height > 0d;

    public bool Equals(CanvasSize other) =>
        Width.Equals(other.Width) && Height.Equals(other.Height);

    public override bool Equals(object? obj) => obj is CanvasSize other && Equals(other);

    public override int GetHashCode() =>
        unchecked((Width.GetHashCode() * 397) ^ Height.GetHashCode());
}

public sealed class PlotViewport
{
    public PlotViewport(NumericRange xRange, NumericRange yRange, CanvasSize canvas)
    {
        if (!xRange.IsValid)
        {
            throw new ArgumentException("A valid X range is required.", nameof(xRange));
        }

        if (!yRange.IsValid)
        {
            throw new ArgumentException("A valid Y range is required.", nameof(yRange));
        }

        if (!canvas.IsValid)
        {
            throw new ArgumentException("A valid canvas size is required.", nameof(canvas));
        }

        XRange = xRange;
        YRange = yRange;
        Canvas = canvas;
    }

    public NumericRange XRange { get; }

    public NumericRange YRange { get; }

    public CanvasSize Canvas { get; }
}

public struct CanvasPoint : IEquatable<CanvasPoint>
{
    public CanvasPoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; }

    public double Y { get; }

    public bool Equals(CanvasPoint other) => X.Equals(other.X) && Y.Equals(other.Y);

    public override bool Equals(object? obj) => obj is CanvasPoint other && Equals(other);

    public override int GetHashCode() =>
        unchecked((X.GetHashCode() * 397) ^ Y.GetHashCode());
}

public sealed class Polyline
{
    internal Polyline(IEnumerable<CanvasPoint> points)
    {
        Points = new List<CanvasPoint>(points).AsReadOnly();
    }

    public ReadOnlyCollection<CanvasPoint> Points { get; }
}

public sealed class CurveGeometry
{
    internal CurveGeometry(IEnumerable<Polyline> polylines)
    {
        Polylines = new List<Polyline>(polylines).AsReadOnly();
    }

    public ReadOnlyCollection<Polyline> Polylines { get; }
}

public static class CartesianTransform
{
    public static CanvasPoint ToCanvas(double x, double y, PlotViewport viewport)
    {
        if (viewport is null)
        {
            throw new ArgumentNullException(nameof(viewport));
        }

        return new CanvasPoint(
            (x - viewport.XRange.Minimum) * viewport.Canvas.Width /
                viewport.XRange.Length,
            viewport.Canvas.Height -
                (y - viewport.YRange.Minimum) * viewport.Canvas.Height /
                viewport.YRange.Length);
    }

    public static CanvasPoint ToData(double canvasX, double canvasY,
        PlotViewport viewport)
    {
        if (viewport is null)
        {
            throw new ArgumentNullException(nameof(viewport));
        }

        return new CanvasPoint(
            canvasX * viewport.XRange.Length / viewport.Canvas.Width +
                viewport.XRange.Minimum,
            (viewport.Canvas.Height - canvasY) * viewport.YRange.Length /
                viewport.Canvas.Height + viewport.YRange.Minimum);
    }
}

public static class CurveGeometryBuilder
{
    public static CurveGeometry Build(SampleSeries series, PlotViewport viewport)
    {
        if (series is null)
        {
            throw new ArgumentNullException(nameof(series));
        }

        if (viewport is null)
        {
            throw new ArgumentNullException(nameof(viewport));
        }

        var pointLists = new List<List<CanvasPoint>>();
        List<CanvasPoint>? current = null;

        for (var index = 1; index < series.Count; index++)
        {
            var first = series[index - 1];
            var second = series[index];

            if (first.Status != FunctionSampleStatus.Finite ||
                second.Status != FunctionSampleStatus.Finite)
            {
                current = null;
                continue;
            }

            var x0 = first.X;
            var y0 = first.Y;
            var x1 = second.X;
            var y1 = second.Y;

            if (!TryClip(viewport.XRange, viewport.YRange,
                ref x0, ref y0, ref x1, ref y1))
            {
                current = null;
                continue;
            }

            var start = CartesianTransform.ToCanvas(x0, y0, viewport);
            var end = CartesianTransform.ToCanvas(x1, y1, viewport);

            if (current is null || !NearlyEqual(current[current.Count - 1], start))
            {
                current = new List<CanvasPoint> { start };
                pointLists.Add(current);
            }

            if (!NearlyEqual(current[current.Count - 1], end))
            {
                current.Add(end);
            }
        }

        var polylines = new List<Polyline>(pointLists.Count);

        foreach (var points in pointLists)
        {
            if (points.Count >= 2)
            {
                polylines.Add(new Polyline(points));
            }
        }

        return new CurveGeometry(polylines);
    }

    private static bool TryClip(NumericRange xRange, NumericRange yRange,
        ref double x0, ref double y0, ref double x1, ref double y1)
    {
        var deltaX = x1 - x0;
        var deltaY = y1 - y0;
        var start = 0d;
        var end = 1d;

        if (!ClipTest(-deltaX, x0 - xRange.Minimum, ref start, ref end) ||
            !ClipTest(deltaX, xRange.Maximum - x0, ref start, ref end) ||
            !ClipTest(-deltaY, y0 - yRange.Minimum, ref start, ref end) ||
            !ClipTest(deltaY, yRange.Maximum - y0, ref start, ref end))
        {
            return false;
        }

        var originalX = x0;
        var originalY = y0;

        x1 = originalX + end * deltaX;
        y1 = originalY + end * deltaY;
        x0 = originalX + start * deltaX;
        y0 = originalY + start * deltaY;

        return true;
    }

    private static bool ClipTest(double direction, double distance,
        ref double start, ref double end)
    {
        if (direction == 0d)
        {
            return distance >= 0d;
        }

        var ratio = distance / direction;

        if (direction < 0d)
        {
            if (ratio > end)
            {
                return false;
            }

            if (ratio > start)
            {
                start = ratio;
            }
        }
        else
        {
            if (ratio < start)
            {
                return false;
            }

            if (ratio < end)
            {
                end = ratio;
            }
        }

        return true;
    }

    private static bool NearlyEqual(CanvasPoint left, CanvasPoint right)
    {
        const double tolerance = 1e-10;

        return Math.Abs(left.X - right.X) <= tolerance &&
            Math.Abs(left.Y - right.Y) <= tolerance;
    }
}
