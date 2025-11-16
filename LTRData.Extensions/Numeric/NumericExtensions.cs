/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using System;
using System.Collections.Generic;
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.Linq;
using System.Threading;
#endif
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace LTRData.Extensions.Numeric;

/// <summary>
/// Static methods for factorization of integer values.
/// </summary>
public static class NumericExtensions
{
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
}

