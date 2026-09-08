using System;
using System.IO;
using LTRData.FunctionPlotting;
using LTRData.Graphics.SkiaSharp;
using LTRData.MathExpression;
using LTRData.MathExpression.Diagrams;
using SkiaSharp;
using Xunit;

namespace LTRData.Rendering.Tests;

public class SkiaRenderingTests
{
    [Fact]
    public void PlotPngContainsTheCurveAndEncoderLeavesInputsOpen()
    {
        var viewport = new PlotViewport(new NumericRange(-1, 1), new NumericRange(-1, 1), new CanvasSize(200, 100));
        var geometry = CurveGeometryBuilder.Build(FunctionSampler.Sample(x => 0, viewport.XRange, 101), viewport);
        using var surface = SKSurface.Create(new SKImageInfo(200, 100));
        surface.Canvas.Clear(SKColors.White);
        SkiaPlotRenderer.Render(surface.Canvas, geometry);
        using var image = surface.Snapshot();
        using var stream = new MemoryStream();
        SkiaPngEncoder.Encode(image, stream);
        Assert.True(stream.CanWrite);
        Assert.Equal(200, image.Width);
        using var bitmap = SKBitmap.Decode(stream.ToArray());
        Assert.Equal(200, bitmap.Width);
        Assert.Equal(100, bitmap.Height);
        Assert.NotEqual(SKColors.White, bitmap.GetPixel(100, 50));
        Assert.Equal(SKColors.White, bitmap.GetPixel(100, 10));
    }

    [Fact]
    public void InvalidSampleCreatesAVisibleGap()
    {
        var viewport = new PlotViewport(new NumericRange(-1, 1), new NumericRange(-1, 1), new CanvasSize(200, 100));
        var samples = FunctionSampler.Sample(x => Math.Abs(x) < .2 ? double.NaN : 0, viewport.XRange, 101);
        using var surface = SKSurface.Create(new SKImageInfo(200, 100));
        surface.Canvas.Clear(SKColors.White);
        SkiaPlotRenderer.Render(surface.Canvas, CurveGeometryBuilder.Build(samples, viewport));
        using var image = surface.Snapshot();
        using var bitmap = SKBitmap.FromImage(image);
        Assert.Equal(SKColors.White, bitmap.GetPixel(100, 50));
        Assert.NotEqual(SKColors.White, bitmap.GetPixel(20, 50));
    }

    [Fact]
    public void DiagramUsesMeasuredFontAndProducesNonemptyPng()
    {
        var diagram = ExpressionDiagram.FromSyntax(MathParser.Default.Parse("sin(x) + x^2").Root!);
        using var font = new SKFont(SKTypeface.Default, 20);
        var sizes = SkiaDiagramRenderer.MeasureNodes(diagram, font);
        var layout = TreeDiagramLayout.Create(diagram, sizes);
        using var surface = SKSurface.Create(new SKImageInfo((int)Math.Ceiling(layout.Size.Width), (int)Math.Ceiling(layout.Size.Height)));
        surface.Canvas.Clear(SKColors.White);
        SkiaDiagramRenderer.Render(surface.Canvas, layout, font);
        using var image = surface.Snapshot();
        using var stream = new MemoryStream();
        SkiaPngEncoder.Encode(image, stream);
        using var bitmap = SKBitmap.Decode(stream.ToArray());
        Assert.Equal(surface.Canvas.DeviceClipBounds.Width, bitmap.Width);
        var darkPixels = 0;
        for (var y = 0; y < bitmap.Height; y++)
            for (var x = 0; x < bitmap.Width; x++)
            {
                var color = bitmap.GetPixel(x, y);
                if (color.Red < 100 && color.Green < 100 && color.Blue < 100) darkPixels++;
            }
        Assert.True(darkPixels > 30, "Labels must contain visible glyphs, not just node outlines.");
    }

    [Fact]
    public void OffscreenAxesAreOmitted()
    {
        using var surface = SKSurface.Create(new SKImageInfo(100, 100));
        surface.Canvas.Clear(SKColors.White);
        SkiaPlotRenderer.RenderAxes(surface.Canvas,
            new PlotViewport(new NumericRange(1, 2), new NumericRange(1, 2), new CanvasSize(100, 100)));
        using var image = surface.Snapshot();
        using var bitmap = SKBitmap.FromImage(image);
        for (var y = 0; y < 100; y++)
            for (var x = 0; x < 100; x++) Assert.Equal(SKColors.White, bitmap.GetPixel(x, y));
    }
}
