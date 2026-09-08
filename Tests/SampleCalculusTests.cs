using LTRData.FunctionPlotting;
using System;
using System.Linq;
using Xunit;

namespace LTRData.Extensions.Tests;

[Trait("Contract", "Modern")]
public class SampleCalculusTests
{
    [Fact]
    public void QuadraticDerivativeIncludesEndpointsAndUnequalSpacing()
    {
        var samples = Series((-2, 4), (-1.5, 2.25), (0, 0), (3, 9), (4, 16));
        var derivative = SampleCalculus.Differentiate(samples);
        for (var i = 0; i < samples.Count; i++)
        {
            Assert.Equal(samples[i].X, derivative[i].X);
            Assert.Equal(2 * samples[i].X, derivative[i].Y, 12);
            Assert.Equal(FunctionSampleStatus.Finite, derivative[i].Status);
        }
    }

    [Fact]
    public void SineDerivativeApproximatesCosineAcrossDomain()
    {
        var samples = FunctionSampler.Sample(Math.Sin, new NumericRange(-Math.PI, Math.PI), 1001);
        var derivative = SampleCalculus.Differentiate(samples);
        foreach (var point in derivative.Samples)
            Assert.InRange(Math.Abs(point.Y - Math.Cos(point.X)), 0, 0.00002);
    }

    [Fact]
    public void DerivativeDoesNotBridgeGapsAndHandlesShortRuns()
    {
        var derivative = SampleCalculus.Differentiate(Series((0, 0), (1, 2), (2, double.NaN),
            (3, 100), (4, 101), (5, double.NaN), (6, 900)));
        Assert.Equal(2, derivative[0].Y);
        Assert.Equal(2, derivative[1].Y);
        Assert.Equal(FunctionSampleStatus.NotANumber, derivative[2].Status);
        Assert.Equal(1, derivative[3].Y);
        Assert.Equal(1, derivative[4].Y);
        Assert.Equal(FunctionSampleStatus.NotANumber, derivative[6].Status);
    }

    [Fact]
    public void IntegralUsesSignedTrapezoidsAndExplicitInitialValue()
    {
        var integral = SampleCalculus.IntegrateFiniteRuns(Series((-2, -2), (-1, -1), (1, 1), (3, 3)), 7);
        Assert.Equal(new[] { 7d, 5.5d, 5.5d, 9.5d }, integral.Samples.Select(p => p.Y));
    }

    [Fact]
    public void IntegralRestartsAtEachFiniteRunAndDoesNotWrap()
    {
        var integral = SampleCalculus.IntegrateFiniteRuns(Series((0, 100), (1, 100),
            (2, double.NaN), (3, 100), (4, 100), (5, 100)));
        Assert.Equal(0, integral[0].Y);
        Assert.Equal(100, integral[1].Y);
        Assert.Equal(FunctionSampleStatus.NotANumber, integral[2].Status);
        Assert.Equal(0, integral[3].Y);
        Assert.Equal(100, integral[4].Y);
        Assert.Equal(200, integral[5].Y);
    }

    [Fact]
    public void LargeFiniteEndpointAverageDoesNotOverflowUnnecessarily()
    {
        var integral = SampleCalculus.IntegrateFiniteRuns(Series((0, 1e308), (0.5, 1e308)));
        Assert.Equal(5e307, integral[1].Y);
        Assert.Equal(FunctionSampleStatus.Finite, integral[1].Status);
        var derivative = SampleCalculus.Differentiate(Series((0, -1e308), (2, 1e308)));
        Assert.Equal(1e308, derivative[0].Y);
    }

    [Fact]
    public void IntegralOverflowIsNotReinterpretedAsNewInitialValue()
    {
        var integral = SampleCalculus.IntegrateFiniteRuns(Series((0, 1e308), (2, 1e308), (3, 0)));
        Assert.Equal(FunctionSampleStatus.PositiveInfinity, integral[1].Status);
        Assert.Equal(FunctionSampleStatus.PositiveInfinity, integral[2].Status);
    }

    [Fact]
    public void EvaluationErrorsAndEmptySeriesRemainExplicit()
    {
        var series = new SampleSeries(new[] { new FunctionSample(0, double.NaN, FunctionSampleStatus.EvaluationError) });
        Assert.Equal(FunctionSampleStatus.EvaluationError, SampleCalculus.Differentiate(series)[0].Status);
        Assert.Equal(FunctionSampleStatus.EvaluationError, SampleCalculus.IntegrateFiniteRuns(series)[0].Status);
        Assert.Equal(0, SampleCalculus.Differentiate(new SampleSeries(new FunctionSample[0])).Count);
    }

    [Fact]
    public void InvalidSampleCoordinatesAreRejected()
    {
        foreach (var series in new[] { Series((0, 1), (0, 2)), Series((1, 1), (0, 2)), Series((double.NaN, 1)) })
        {
            Assert.Throws<ArgumentException>(() => SampleCalculus.Differentiate(series));
            Assert.Throws<ArgumentException>(() => SampleCalculus.IntegrateFiniteRuns(series));
        }
        Assert.Throws<ArgumentOutOfRangeException>(() => SampleCalculus.IntegrateFiniteRuns(Series((0, 1)), double.NaN));
    }

    private static SampleSeries Series(params (double X, double Y)[] values) => new(values.Select(p =>
        new FunctionSample(p.X, p.Y, double.IsNaN(p.Y) ? FunctionSampleStatus.NotANumber : FunctionSampleStatus.Finite)));
}
