using LTRData.FunctionPlotting;
using LTRData.MathExpression;
using System;
using Xunit;

namespace LTRData.Extensions.Tests;

[Trait("Contract", "Modern")]
public class FunctionPlottingTests
{
    [Fact]
    public void FixedCountSamplingIncludesBothDomainEndpoints()
    {
        var samples = FunctionSampler.Sample(x => x * 2d,
            new NumericRange(-1d, 1d), 5);

        Assert.Equal(5, samples.Count);
        Assert.Equal(-1d, samples[0].X);
        Assert.Equal(-0.5d, samples[1].X);
        Assert.Equal(0d, samples[2].X);
        Assert.Equal(0.5d, samples[3].X);
        Assert.Equal(1d, samples[4].X);
        Assert.Equal(2d, samples[4].Y);
    }

    [Fact]
    public void SamplingClassifiesEveryNonFiniteAndErrorState()
    {
        var samples = FunctionSampler.Sample(x =>
        {
            if (x == -2d)
            {
                return double.NaN;
            }

            if (x == -1d)
            {
                return double.NegativeInfinity;
            }

            if (x == 0d)
            {
                throw new ArithmeticException("Deliberate sample failure.");
            }

            if (x == 1d)
            {
                return double.PositiveInfinity;
            }

            return x;
        }, new NumericRange(-2d, 2d), 5);

        Assert.Equal(FunctionSampleStatus.NotANumber, samples[0].Status);
        Assert.Equal(FunctionSampleStatus.NegativeInfinity, samples[1].Status);
        Assert.Equal(FunctionSampleStatus.EvaluationError, samples[2].Status);
        Assert.Equal(FunctionSampleStatus.PositiveInfinity, samples[3].Status);
        Assert.Equal(FunctionSampleStatus.Finite, samples[4].Status);
    }

    [Fact]
    public void InvalidSamplesSplitCurveGeometry()
    {
        var samples = new SampleSeries(new[]
        {
            new FunctionSample(-2d, -2d, FunctionSampleStatus.Finite),
            new FunctionSample(-1d, -1d, FunctionSampleStatus.Finite),
            new FunctionSample(0d, double.NaN, FunctionSampleStatus.NotANumber),
            new FunctionSample(1d, 1d, FunctionSampleStatus.Finite),
            new FunctionSample(2d, 2d, FunctionSampleStatus.Finite)
        });

        var geometry = CurveGeometryBuilder.Build(samples,
            new PlotViewport(new NumericRange(-2d, 2d),
                new NumericRange(-2d, 2d), new CanvasSize(400d, 200d)));

        Assert.Equal(2, geometry.Polylines.Count);
        Assert.Equal(2, geometry.Polylines[0].Points.Count);
        Assert.Equal(2, geometry.Polylines[1].Points.Count);
    }

    [Fact]
    public void ClippingKeepsCrossingSegmentWithBothEndpointsOutside()
    {
        var samples = new SampleSeries(new[]
        {
            new FunctionSample(-1d, -2d, FunctionSampleStatus.Finite),
            new FunctionSample(1d, 2d, FunctionSampleStatus.Finite)
        });

        var geometry = CurveGeometryBuilder.Build(samples,
            new PlotViewport(new NumericRange(-1d, 1d),
                new NumericRange(-1d, 1d), new CanvasSize(200d, 100d)));

        var line = Assert.Single(geometry.Polylines);
        Assert.Equal(2, line.Points.Count);
        Assert.Equal(50d, line.Points[0].X, 12);
        Assert.Equal(100d, line.Points[0].Y, 12);
        Assert.Equal(150d, line.Points[1].X, 12);
        Assert.Equal(0d, line.Points[1].Y, 12);
    }

    [Fact]
    public void CartesianTransformRoundTripsContinuousCoordinates()
    {
        var viewport = new PlotViewport(new NumericRange(-10d, 10d),
            new NumericRange(-4d, 4d), new CanvasSize(1200d, 720d));

        var canvas = CartesianTransform.ToCanvas(2.5d, -1.25d, viewport);
        var data = CartesianTransform.ToData(canvas.X, canvas.Y, viewport);

        Assert.Equal(2.5d, data.X, 12);
        Assert.Equal(-1.25d, data.Y, 12);
    }

    [Fact]
    public void ExpressionEvaluationComposesWithParserIndependentSampler()
    {
        var parse = MathParser.Default.Parse("sin(x)");
        Assert.True(parse.Success);

        var binding = MathBinder.Bind(parse.Root!, MathSymbolCatalog.Standard);
        Assert.True(binding.Success);

        var function = binding.Expression!.BindUnary("x");
        var samples = FunctionSampler.Sample(function.Evaluate,
            new NumericRange(0d, Math.PI), 3);

        Assert.Equal(0d, samples[0].Y, 12);
        Assert.Equal(1d, samples[1].Y, 12);
        Assert.Equal(0d, samples[2].Y, 12);
    }

    [Fact]
    public void InvalidRangesAndSampleCountsAreRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new NumericRange(1d, 1d));
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            FunctionSampler.Sample(x => x, new NumericRange(0d, 1d), 1));
    }
}
