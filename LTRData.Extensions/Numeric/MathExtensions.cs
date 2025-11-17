// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace LTRData.Extensions.Numeric;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS9191 // The 'ref' modifier for an argument corresponding to 'in' parameter is equivalent to 'in'. Consider using 'in' instead.
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class MathExtensions
{
    public static int GetNumberOfDecimalsSafe(this decimal dec) => (decimal.GetBits(dec)[3] & 0xFF0000) >> 16;

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static int GetNumberOfDecimals(this decimal dec)
    {
        Span<byte> bytes = stackalloc byte[sizeof(decimal)];

        MemoryMarshal.Write(bytes, ref dec);

        var flags = MemoryMarshal.Read<int>(bytes);

        return (flags & 0xFF0000) >> 16;
    }
#endif

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static double? Median(this IEnumerable<double?> sequence)
    {
        var a = (from n in sequence
                 where n.HasValue
                 let v = n.Value
                 orderby v
                 select n).ToArray();

        if (a.Length == 0)
        {
            return default;
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2d + a[(a.Length >> 1) - 1] / 2d;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

    public static decimal? Median(this IEnumerable<decimal?> sequence)
    {

        var a = (from n in sequence
                 where n.HasValue
                 let v = n.Value
                 orderby v
                 select n).ToArray();

        if (a.Length == 0)
        {
            return default;
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2m + a[(a.Length >> 1) - 1] / 2m;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

    public static double Median(this IEnumerable<double> sequence)
    {

        var a = (from v in sequence
                 orderby v
                 select v).ToArray();

        if (a.Length == 0)
        {
            throw new InvalidOperationException("Collection is empty");
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2d + a[(a.Length >> 1) - 1] / 2d;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

    public static decimal Median(this IEnumerable<decimal> sequence)
    {

        var a = (from v in sequence
                 orderby v
                 select v).ToArray();

        if (a.Length == 0)
        {
            throw new InvalidOperationException("Collection is empty");
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2m + a[(a.Length >> 1) - 1] / 2m;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

    public static double? Median<TSource>(this IEnumerable<TSource> sequence, Func<TSource, double?> selector)
    {

        var a = (from n in sequence.Select(selector)
                 where n.HasValue
                 let v = n.Value
                 orderby v
                 select n).ToArray();

        if (a.Length == 0)
        {
            return default;
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2d + a[(a.Length >> 1) - 1] / 2d;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

    public static decimal? Median<TSource>(this IEnumerable<TSource> sequence, Func<TSource, decimal?> selector)
    {

        var a = (from n in sequence.Select(selector)
                 where n.HasValue
                 let v = n.Value
                 orderby v
                 select n).ToArray();

        if (a.Length == 0)
        {
            return default;
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2m + a[(a.Length >> 1) - 1] / 2m;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

    public static double Median<TSource>(this IEnumerable<TSource> sequence, Func<TSource, double> selector)
    {

        var a = sequence.Select(selector).OrderBy(v => v).ToArray();

        if (a.Length == 0)
        {
            throw new InvalidOperationException("Collection is empty");
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2d + a[(a.Length >> 1) - 1] / 2d;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

    public static decimal Median<TSource>(this IEnumerable<TSource> sequence, Func<TSource, decimal> selector)
    {

        var a = sequence.Select(selector).OrderBy(v => v).ToArray();

        if (a.Length == 0)
        {
            throw new InvalidOperationException("Collection is empty");
        }
        else if ((a.Length & 1) == 0)
        {
            return a[a.Length >> 1] / 2m + a[(a.Length >> 1) - 1] / 2m;
        }
        else
        {
            return a[a.Length >> 1];
        }

    }

#endif

    /// <summary>
    /// Finds greatest common divisor for two values.
    /// </summary>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    /// <returns>Greatest common divisor for values a and b.</returns>
    public static int GreatestCommonDivisor(int a, int b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

    /// <summary>
    /// Finds greatest common divisor for two values.
    /// </summary>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    /// <returns>Greatest common divisor for values a and b.</returns>
    public static long GreatestCommonDivisor(long a, long b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

#if NET7_0_OR_GREATER
    /// <summary>
    /// Finds greatest common divisor for two values.
    /// </summary>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    /// <returns>Greatest common divisor for values a and b.</returns>
    public static TNumber GreatestCommonDivisor<TNumber>(TNumber a, TNumber b) where TNumber : INumber<TNumber>
    {
        while (b != TNumber.Zero)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }
#endif

    /// <summary>
    /// Finds greatest common divisor for a sequence of values.
    /// </summary>
    /// <param name="values">Sequence of values</param>
    /// <returns>Greatest common divisor for values.</returns>
    public static int GreatestCommonDivisor(this IEnumerable<int> values) =>
        values.Aggregate(GreatestCommonDivisor);

    /// <summary>
    /// Finds greatest common divisor for a sequence of values.
    /// </summary>
    /// <param name="values">Sequence of values</param>
    /// <returns>Greatest common divisor for values.</returns>
    public static long GreatestCommonDivisor(this IEnumerable<long> values) =>
        values.Aggregate(GreatestCommonDivisor);

#if NET7_0_OR_GREATER
    /// <summary>
    /// Finds greatest common divisor for a sequence of values.
    /// </summary>
    /// <param name="values">Sequence of values</param>
    /// <returns>Greatest common divisor for values.</returns>
    public static TNumber GreatestCommonDivisor<TNumber>(this IEnumerable<TNumber> values) where TNumber : INumber<TNumber> =>
        values.Aggregate(GreatestCommonDivisor);
#endif

    /// <summary>
    /// Finds least common multiple for two values.
    /// </summary>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    /// <returns>Least common multiple for values a and b.</returns>
    public static int LeastCommonMultiple(int a, int b)
        => a / GreatestCommonDivisor(a, b) * b;

    /// <summary>
    /// Finds least common multiple for two values.
    /// </summary>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    /// <returns>Least common multiple for values a and b.</returns>
    public static long LeastCommonMultiple(long a, long b)
        => a / GreatestCommonDivisor(a, b) * b;

#if NET7_0_OR_GREATER
    /// <summary>
    /// Finds least common multiple for two values.
    /// </summary>
    /// <param name="a">First value</param>
    /// <param name="b">Second value</param>
    /// <returns>Least common multiple for values a and b.</returns>
    public static TNumber LeastCommonMultiple<TNumber>(TNumber a, TNumber b) where TNumber : INumber<TNumber>
        => a / GreatestCommonDivisor(a, b) * b;
#endif

    /// <summary>
    /// Finds least common multiple for a sequence of values.
    /// </summary>
    /// <param name="values">Sequence of values</param>
    /// <returns>Least common multiple for values.</returns>
    public static int LeastCommonMultiple(this IEnumerable<int> values) =>
        values.Aggregate(LeastCommonMultiple);

    /// <summary>
    /// Finds least common multiple for a sequence of values.
    /// </summary>
    /// <param name="values">Sequence of values</param>
    /// <returns>Least common multiple for values.</returns>
    public static long LeastCommonMultiple(this IEnumerable<long> values) =>
        values.Aggregate(LeastCommonMultiple);

#if NET7_0_OR_GREATER
    /// <summary>
    /// Finds least common multiple for a sequence of values.
    /// </summary>
    /// <param name="values">Sequence of values</param>
    /// <returns>Least common multiple for values.</returns>
    public static TNumber LeastCommonMultiple<TNumber>(this IEnumerable<TNumber> values) where TNumber : INumber<TNumber> =>
        values.Aggregate(LeastCommonMultiple);
#endif

    /// <summary>
    /// Returns a sequence of prime factors for a value.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Sequence of prime factors</returns>
    public static IEnumerable<int> PrimeFactors(this int value)
    {
        var z = 2;

        while (checked(z * z) <= value)
        {
            if (value % z == 0)
            {
                yield return z;
                value /= z;
            }
            else
            {
                z++;
            }
        }

        if (value > 1)
        {
            yield return value;
        }
    }

    /// <summary>
    /// Returns a sequence of prime factors for a value.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Sequence of prime factors</returns>
    public static IEnumerable<long> PrimeFactors(this long value)
    {
        var z = 2L;

        while (checked(z * z) <= value)
        {
            if (value % z == 0)
            {
                yield return z;
                value /= z;
            }
            else
            {
                z++;
            }
        }

        if (value > 1)
        {
            yield return value;
        }
    }

#if NET7_0_OR_GREATER
    /// <summary>
    /// Returns a sequence of prime factors for a value.
    /// </summary>
    /// <param name="value">Value</param>
    /// <returns>Sequence of prime factors</returns>
    public static IEnumerable<TNumber> PrimeFactors<TNumber>(this TNumber value) where TNumber : INumber<TNumber>
    {
        var z = TNumber.One + TNumber.One;

        while (checked(z * z) <= value)
        {
            if (value % z == TNumber.Zero)
            {
                yield return z;
                value /= z;
            }
            else
            {
                z++;
            }
        }

        if (value > TNumber.One)
        {
            yield return value;
        }
    }
#endif

    private static readonly object _syncPrim64 = new();

    private static List<long> PrimetableInt64 => field ??= [2, 3];

    /// <summary>
    /// Calculates a sequence of prime numbers as long as the sequence is
    /// iterated through. If an overflow occurs, an exception is thrown.
    /// </summary>
    /// <returns>A sequence of prime numbers.</returns>
    public static IEnumerable<long> EnumeratePrimeNumbers()
    {
        lock (_syncPrim64)
        {
            var value = 0L;

            for (var i = 0; i < PrimetableInt64.Count; i++)
            {
                value = PrimetableInt64[i];

                yield return value;
            }

            for (; ; )
            {
                value = checked(value + 2);

                if (!PrimetableInt64
                    .Skip(1)
                    .TakeWhile(prime => prime * prime <= value)
                    .Any(prime => value % prime == 0))
                {
                    PrimetableInt64.Add(value);

                    yield return value;
                }
            }
        }
    }

#if NET7_0_OR_GREATER
    private static readonly object _syncPrim128 = new();

    private static List<Int128> PrimetableInt128 => field ??= [2, 3];

    /// <summary>
    /// Calculates a sequence of prime numbers as long as the sequence is
    /// iterated through. If an overflow occurs, an exception is thrown.
    /// </summary>
    /// <returns>A sequence of prime numbers.</returns>
    public static IEnumerable<Int128> EnumeratePrimeNumbersInt128()
    {
        lock (_syncPrim64)
        {
            var value = default(Int128);

            for (var i = 0; i < PrimetableInt128.Count; i++)
            {
                value = PrimetableInt128[i];

                yield return value;
            }

            for (; ; )
            {
                value = checked(value + 2);

                if (!PrimetableInt128
                    .Skip(1)
                    .TakeWhile(prime => prime * prime <= value)
                    .Any(prime => value % prime == 0))
                {
                    PrimetableInt128.Add(value);

                    yield return value;
                }
            }
        }
    }
#endif

    /// <summary>
    /// Multiplies a series of factors and returns result.
    /// </summary>
    /// <param name="factors">Series of factors</param>
    /// <returns></returns>
    public static int Multiply(this IEnumerable<int> factors) =>
        factors.Aggregate((prod, factor) => checked(prod * factor));

    /// <summary>
    /// Multiplies a series of factors and returns result.
    /// </summary>
    /// <param name="factors">Series of factors</param>
    /// <returns></returns>
    public static long Multiply(this IEnumerable<long> factors) =>
        factors.Aggregate((prod, factor) => checked(prod * factor));

#if NET7_0_OR_GREATER
    /// <summary>
    /// Multiplies a series of factors and returns result.
    /// </summary>
    /// <param name="factors">Series of factors</param>
    /// <returns></returns>
    public static TNumber Multiply<TNumber>(this IEnumerable<TNumber> factors) where TNumber : INumber<TNumber> =>
        factors.Aggregate((prod, factor) => checked(prod * factor));
#endif

    // 
    // Summary:
    // Rounds a decimal value to a specified number of fractional digits.
    // 
    // Parameters:
    // d:
    // A decimal number to be rounded.
    // 
    // decimals:
    // The number of decimal places in the return value.
    // 
    // Returns:
    // The number nearest to d that contains a number of fractional digits equal to
    // decimals.
    // 
    // Exceptions:
    // T:System.ArgumentOutOfRangeException:
    // decimals is less than 0 or greater than 28.
    // 
    // T:System.OverflowException:
    // The result is outside the range of a System.Decimal.
    public static decimal RoundValue(this decimal d, int decimals) => Math.Round(d, decimals);
    // 
    // Summary:
    // Rounds a decimal value to the nearest integral value.
    // 
    // Parameters:
    // d:
    // A decimal number to be rounded.
    // 
    // Returns:
    // The integer nearest parameter d. If the fractional component of d is halfway
    // between two integers, one of which is even and the other odd, the even number
    // is returned. Note that this method returns a System.Decimal instead of an integral
    // type.
    // 
    // Exceptions:
    // T:System.OverflowException:
    // The result is outside the range of a System.Decimal.
    public static decimal RoundValue(this decimal d) => Math.Round(d);

    // 
    // Summary:
    // Rounds a decimal value to a specified number of fractional digits. A parameter
    // specifies how to round the value if it is midway between two numbers.
    // 
    // Parameters:
    // d:
    // A decimal number to be rounded.
    // 
    // decimals:
    // The number of decimal places in the return value.
    // 
    // mode:
    // Specification for how to round d if it is midway between two other numbers.
    // 
    // Returns:
    // The number nearest to d that contains a number of fractional digits equal to
    // decimals. If d has fewer fractional digits than decimals, d is returned unchanged.
    // 
    // Exceptions:
    // T:System.ArgumentOutOfRangeException:
    // decimals is less than 0 or greater than 28.
    // 
    // T:System.ArgumentException:
    // mode is not a valid value of System.MidpointRounding.
    // 
    // T:System.OverflowException:
    // The result is outside the range of a System.Decimal.
    public static decimal RoundValue(this decimal d, int decimals, MidpointRounding mode) => Math.Round(d, decimals, mode);
    // 
    // Summary:
    // Rounds a decimal value to the nearest integer. A parameter specifies how to round
    // the value if it is midway between two numbers.
    // 
    // Parameters:
    // d:
    // A decimal number to be rounded.
    // 
    // mode:
    // Specification for how to round d if it is midway between two other numbers.
    // 
    // Returns:
    // The integer nearest d. If d is halfway between two numbers, one of which is even
    // and the other odd, then mode determines which of the two is returned.
    // 
    // Exceptions:
    // T:System.ArgumentException:
    // mode is not a valid value of System.MidpointRounding.
    // 
    // T:System.OverflowException:
    // The result is outside the range of a System.Decimal.
    public static decimal RoundValue(this decimal d, MidpointRounding mode) => Math.Round(d, mode);
    // 
    // Summary:
    // Rounds a double-precision floating-point value to the nearest integral value.
    // 
    // Parameters:
    // a:
    // A double-precision floating-point number to be rounded.
    // 
    // Returns:
    // The integer nearest a. If the fractional component of a is halfway between two
    // integers, one of which is even and the other odd, then the even number is returned.
    // Note that this method returns a System.Double instead of an integral type.
    public static double RoundValue(this double a) => Math.Round(a);
    // 
    // Summary:
    // Rounds a double-precision floating-point value to a specified number of fractional
    // digits.
    // 
    // Parameters:
    // value:
    // A double-precision floating-point number to be rounded.
    // 
    // digits:
    // The number of fractional digits in the return value.
    // 
    // Returns:
    // The number nearest to value that contains a number of fractional digits equal
    // to digits.
    // 
    // Exceptions:
    // T:System.ArgumentOutOfRangeException:
    // digits is less than 0 or greater than 15.
    public static double RoundValue(this double value, int digits) => Math.Round(value, digits);

    // 
    // Summary:
    // Rounds a double-precision floating-point value to the nearest integer. A parameter
    // specifies how to round the value if it is midway between two numbers.
    // 
    // Parameters:
    // value:
    // A double-precision floating-point number to be rounded.
    // 
    // mode:
    // Specification for how to round value if it is midway between two other numbers.
    // 
    // Returns:
    // The integer nearest value. If value is halfway between two integers, one of which
    // is even and the other odd, then mode determines which of the two is returned.
    // 
    // Exceptions:
    // T:System.ArgumentException:
    // mode is not a valid value of System.MidpointRounding.
    public static double RoundValue(this double value, MidpointRounding mode) => Math.Round(value, mode);
    // 
    // Summary:
    // Rounds a double-precision floating-point value to a specified number of fractional
    // digits. A parameter specifies how to round the value if it is midway between
    // two numbers.
    // 
    // Parameters:
    // value:
    // A double-precision floating-point number to be rounded.
    // 
    // digits:
    // The number of fractional digits in the return value.
    // 
    // mode:
    // Specification for how to round value if it is midway between two other numbers.
    // 
    // Returns:
    // The number nearest to value that has a number of fractional digits equal to digits.
    // If value has fewer fractional digits than digits, value is returned unchanged.
    // 
    // Exceptions:
    // T:System.ArgumentOutOfRangeException:
    // digits is less than 0 or greater than 15.
    // 
    // T:System.ArgumentException:
    // mode is not a valid value of System.MidpointRounding.
    public static double RoundValue(this double value, int digits, MidpointRounding mode) => Math.Round(value, digits, mode);

}

