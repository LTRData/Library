using System;

namespace LTRData.FunctionPlotting;

/// <summary>Numerical calculus on ordered samples in data coordinates.</summary>
public static class SampleCalculus
{
    /// <summary>
    /// Differentiates each finite run using three-point differences, including
    /// one-sided endpoint differences. Two-point runs use their secant slope;
    /// isolated points have no derivative. Unequal X spacing is supported.
    /// </summary>
    /// <remarks>Invalid samples split runs; no difference crosses a gap.</remarks>
    public static SampleSeries Differentiate(SampleSeries series)
    {
        Validate(series);
        var result = new FunctionSample[series.Count];
        var start = 0;

        while (start < series.Count)
        {
            if (!IsFinite(series[start]))
            {
                result[start] = Gap(series[start]);
                start++;
                continue;
            }

            var end = start + 1;
            while (end < series.Count && IsFinite(series[end])) end++;

            for (var index = start; index < end; index++)
            {
                double derivative;
                if (end - start == 1) derivative = double.NaN;
                else if (end - start == 2) derivative = Slope(series[start], series[end - 1]);
                else
                {
                    var middle = Math.Max(start + 1, Math.Min(index, end - 2));
                    var left = series[middle - 1];
                    var center = series[middle];
                    var right = series[middle + 1];
                    var leftStep = center.X - left.X;
                    var rightStep = right.X - center.X;
                    var scale = Math.Max(leftStep, rightStep);
                    var leftWeight = (leftStep / scale) / (leftStep / scale + rightStep / scale);
                    var rightWeight = 1d - leftWeight;
                    var leftSlope = Slope(left, center);
                    var rightSlope = Slope(center, right);

                    if (index == start)
                        derivative = leftSlope + leftWeight * (leftSlope - rightSlope);
                    else if (index == end - 1)
                        derivative = rightSlope + rightWeight * (rightSlope - leftSlope);
                    else
                        derivative = rightWeight * leftSlope + leftWeight * rightSlope;
                }

                result[index] = Value(series[index].X, derivative);
            }
            start = end;
        }

        return new SampleSeries(result);
    }

    /// <summary>
    /// Computes cumulative trapezoidal integrals. Each finite run starts at
    /// <paramref name="initialValue"/> at its leftmost sample, including after a gap.
    /// </summary>
    /// <remarks>
    /// No area is inferred across missing samples. Values are independent of the
    /// viewport and never wrap for display. Overflow remains non-finite until a
    /// new finite run begins. This does not evaluate improper integrals.
    /// </remarks>
    public static SampleSeries IntegrateFiniteRuns(SampleSeries series, double initialValue = 0d)
    {
        Validate(series);
        if (!NumericRange.IsFinite(initialValue))
            throw new ArgumentOutOfRangeException(nameof(initialValue), "The initial value must be finite.");

        var result = new FunctionSample[series.Count];
        var inRun = false;
        var integral = initialValue;

        for (var index = 0; index < series.Count; index++)
        {
            var sample = series[index];
            if (!IsFinite(sample))
            {
                result[index] = Gap(sample);
                inRun = false;
                continue;
            }

            if (!inRun) integral = initialValue;
            else
            {
                var previous = series[index - 1];
                // Halving first avoids overflowing a finite endpoint average.
                var average = previous.Y * 0.5d + sample.Y * 0.5d;
                integral += average * (sample.X - previous.X);
            }

            result[index] = Value(sample.X, integral);
            inRun = true;
        }

        return new SampleSeries(result);
    }

    private static double Slope(FunctionSample first, FunctionSample second)
    {
        var difference = second.Y - first.Y;
        var step = second.X - first.X;
        return NumericRange.IsFinite(difference)
            ? difference / step
            : (second.Y * 0.5d - first.Y * 0.5d) / (step * 0.5d);
    }

    private static bool IsFinite(FunctionSample sample) =>
        sample.Status == FunctionSampleStatus.Finite && NumericRange.IsFinite(sample.Y);

    private static FunctionSample Gap(FunctionSample sample) => new(sample.X, double.NaN,
        sample.Status == FunctionSampleStatus.EvaluationError
            ? FunctionSampleStatus.EvaluationError : FunctionSampleStatus.NotANumber);

    private static FunctionSample Value(double x, double y) =>
        new(x, y, FunctionSampler.Classify(y));

    private static void Validate(SampleSeries series)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(series);
#else
        if (series is null) throw new ArgumentNullException(nameof(series));
#endif
        for (var index = 0; index < series.Count; index++)
        {
            var x = series[index].X;
            if (!NumericRange.IsFinite(x) || (index > 0 &&
                (x <= series[index - 1].X || !NumericRange.IsFinite(x - series[index - 1].X))))
                throw new ArgumentException("Sample X coordinates must be finite and strictly increasing with finite spacing.", nameof(series));
        }
    }
}
