using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LTRData.FunctionPlotting;

#pragma warning disable CS1591

public delegate double ScalarFunction(double x);

public enum FunctionSampleStatus
{
    Finite,
    NotANumber,
    PositiveInfinity,
    NegativeInfinity,
    EvaluationError
}

public struct FunctionSample : IEquatable<FunctionSample>
{
    public FunctionSample(double x, double y, FunctionSampleStatus status)
    {
        X = x;
        Y = y;
        Status = status;
    }

    public double X { get; }

    public double Y { get; }

    public FunctionSampleStatus Status { get; }

    public bool Equals(FunctionSample other) =>
        X.Equals(other.X) && Y.Equals(other.Y) && Status == other.Status;

    public override bool Equals(object? obj) => obj is FunctionSample other && Equals(other);

    public override int GetHashCode() =>
        unchecked((((X.GetHashCode() * 397) ^ Y.GetHashCode()) * 397) ^
            (int)Status);

    public static bool operator ==(FunctionSample left, FunctionSample right) =>
        left.Equals(right);

    public static bool operator !=(FunctionSample left, FunctionSample right) =>
        !left.Equals(right);
}

public sealed class SampleSeries
{
    private readonly ReadOnlyCollection<FunctionSample> samples;

    public SampleSeries(IEnumerable<FunctionSample> samples)
    {
        if (samples is null)
        {
            throw new ArgumentNullException(nameof(samples));
        }

        this.samples = new List<FunctionSample>(samples).AsReadOnly();
    }

    internal SampleSeries(FunctionSample[] samples)
    {
        this.samples = Array.AsReadOnly(samples);
    }

    public ReadOnlyCollection<FunctionSample> Samples => samples;

    public int Count => samples.Count;

    public FunctionSample this[int index] => samples[index];
}

public static class FunctionSampler
{
    public static SampleSeries Sample(ScalarFunction function, NumericRange domain,
        int sampleCount)
    {
        if (function is null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        if (!domain.IsValid)
        {
            throw new ArgumentException("A valid sampling domain is required.",
                nameof(domain));
        }

        if (sampleCount < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(sampleCount),
                "At least two samples are required.");
        }

        var samples = new FunctionSample[sampleCount];
        var step = domain.Length / (sampleCount - 1);

        for (var index = 0; index < samples.Length; index++)
        {
            var x = index == samples.Length - 1
                ? domain.Maximum
                : domain.Minimum + step * index;

            double y;
            FunctionSampleStatus status;

            try
            {
                y = function(x);
                status = Classify(y);
            }
            catch (Exception exception)
            {
                if (IsFatal(exception))
                {
                    throw;
                }

                y = double.NaN;
                status = FunctionSampleStatus.EvaluationError;
            }

            samples[index] = new FunctionSample(x, y, status);
        }

        return new SampleSeries(samples);
    }

    internal static FunctionSampleStatus Classify(double value)
    {
        if (double.IsNaN(value))
        {
            return FunctionSampleStatus.NotANumber;
        }

        if (double.IsPositiveInfinity(value))
        {
            return FunctionSampleStatus.PositiveInfinity;
        }

        if (double.IsNegativeInfinity(value))
        {
            return FunctionSampleStatus.NegativeInfinity;
        }

        return FunctionSampleStatus.Finite;
    }

    private static bool IsFatal(Exception exception) =>
        exception is OutOfMemoryException ||
        exception is StackOverflowException ||
        exception is AccessViolationException ||
        exception is AppDomainUnloadedException ||
        exception is BadImageFormatException ||
        exception is CannotUnloadAppDomainException ||
        exception is InvalidProgramException;
}
