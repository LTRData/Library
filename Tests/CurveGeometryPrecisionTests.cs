using System;
using LTRData.FunctionPlotting;
using Xunit;

namespace LTRData.Extensions.Tests;

public class CurveGeometryPrecisionTests
{
    [Fact]
    public void OppositeLargeFiniteValuesClipToFiniteBoundaryCoordinates()
    {
        var viewport = new PlotViewport(new NumericRange(-1, 1), new NumericRange(-1, 1), new CanvasSize(200, 100));
        var series = new SampleSeries(new[]
        {
            new FunctionSample(-1, -1e308, FunctionSampleStatus.Finite),
            new FunctionSample(1, 1e308, FunctionSampleStatus.Finite)
        });
        var line = Assert.Single(CurveGeometryBuilder.Build(series, viewport).Polylines);
        Assert.Equal(100d, line.Points[0].Y);
        Assert.Equal(0d, line.Points[line.Points.Count - 1].Y);
        foreach (var point in line.Points)
        {
            Assert.InRange(point.X, 0, 200);
            Assert.InRange(point.Y, 0, 100);
        }
    }

    [Fact]
    public void LargeFiniteViewportTransformsDoNotOverflowIntermediateProducts()
    {
        var viewport = new PlotViewport(new NumericRange(0, 1e308), new NumericRange(0, 1e308), new CanvasSize(200, 100));
        var point = CartesianTransform.ToCanvas(5e307, 5e307, viewport);
        Assert.Equal(100d, point.X);
        Assert.Equal(50d, point.Y);
        var data = CartesianTransform.ToData(100, 50, viewport);
        Assert.Equal(5e307, data.X);
        Assert.Equal(5e307, data.Y);
    }
}
