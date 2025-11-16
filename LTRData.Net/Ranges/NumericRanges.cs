// 
// LTRLib
// 
// Copyright (c) Olof Lagerkvist, LTR Data
// http://ltr-data.se   https://github.com/LTRData
// 

#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Collections.Generic;

namespace LTRData.Ranges;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public partial class NumericRanges<T> : List<(T Start, T End)> where T : IComparable<T>
{
    public NumericRanges()
    {
    }

    public NumericRanges(int capacity) : base(capacity)
    {
    }

    public NumericRanges(IEnumerable<(T, T)> collection) : base(collection)
    {
    }

    public void Add(T start, T end) => Add((start, end));

    public bool Encompasses(T value) => FindIndex(value) >= 0;

    public int FindIndex(T value) => FindIndex(range => value.CompareTo(range.Start) >= 0 && value.CompareTo(range.End) <= 0);

    public (T Start, T End) Find(T value) => Find(range => value.CompareTo(range.Start) >= 0 && value.CompareTo(range.End) <= 0);
}

#endif
