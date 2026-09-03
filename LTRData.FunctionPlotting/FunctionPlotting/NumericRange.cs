using System;

namespace LTRData.FunctionPlotting;

#pragma warning disable CS1591

public struct NumericRange : IEquatable<NumericRange>
{
    public NumericRange(double minimum, double maximum)
    {
        if (!IsFinite(minimum))
        {
            throw new ArgumentOutOfRangeException(nameof(minimum),
                "Range minimum must be finite.");
        }

        if (!IsFinite(maximum))
        {
            throw new ArgumentOutOfRangeException(nameof(maximum),
                "Range maximum must be finite.");
        }

        if (maximum <= minimum)
        {
            throw new ArgumentOutOfRangeException(nameof(maximum),
                "Range maximum must be greater than its minimum.");
        }

        Minimum = minimum;
        Maximum = maximum;
    }

    public double Minimum { get; }

    public double Maximum { get; }

    public double Length => Maximum - Minimum;

    internal bool IsValid => IsFinite(Minimum) && IsFinite(Maximum) && Maximum > Minimum;

    public bool Contains(double value) => value >= Minimum && value <= Maximum;

    public bool Equals(NumericRange other) =>
        Minimum.Equals(other.Minimum) && Maximum.Equals(other.Maximum);

    public override bool Equals(object? obj) => obj is NumericRange other && Equals(other);

    public override int GetHashCode() =>
        unchecked((Minimum.GetHashCode() * 397) ^ Maximum.GetHashCode());

    public override string ToString() => $"{Minimum}..{Maximum}";

    public static bool operator ==(NumericRange left, NumericRange right) => left.Equals(right);

    public static bool operator !=(NumericRange left, NumericRange right) => !left.Equals(right);

    internal static bool IsFinite(double value) =>
        !double.IsNaN(value) && !double.IsInfinity(value);
}
