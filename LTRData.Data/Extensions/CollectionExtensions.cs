using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
using LTRData.Reflection;
#endif

namespace LTRData.Extensions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class CollectionExtensions
{
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> Dict, IEnumerable<KeyValuePair<TKey, TValue>> itemPairs)
    {
        foreach (var itemPair in itemPairs)
        {
            Dict.Add(itemPair);
        }
    }

#if NET45_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> Dict, IEnumerable<(TKey Key, TValue Value)> itemPairs)
    {
        foreach (var (Key, Value) in itemPairs)
        {
            Dict.Add(Key, Value);
        }
    }
#endif

    /// <summary>
    /// Adds an element to an ICollection(Of T) if that element is not already present in
    /// the list.
    /// </summary>
    /// <typeparam name="T">Type of the elements in the list.</typeparam>
    /// <param name="List">List to add element to.</param>
    /// <param name="item">Item to add to ICollection(Of T) object.</param>
    /// <returns>Returns True if element was added or False if already present in list.</returns>
    public static bool TryAddNew<T>(this ICollection<T> List, T item)
    {
        if (List.Contains(item))
        {
            return false;
        }
        else
        {
            List.Add(item);
            return true;
        }
    }

    /// <summary>
    /// Returns value at specified index in an IDictionary(Of TKey, TValue).
    /// </summary>
    /// <typeparam name="TKey">Type of keys in IDictionary(Of TKey, TValue)</typeparam>
    /// <typeparam name="TValue">Type of values in IDictionary(Of TKey, TValue)</typeparam>
    /// <param name="list">IDictionary(Of TKey, TValue) to search.</param>
    /// <param name="index">Index within IDictionary(Of TKey, TValue)</param>
    /// <returns>Value at specified index.</returns>
    /// <remarks>Throws an exception if index is out of bounds.</remarks>
    public static TValue ValueAt<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, int index)
    {
        if (index >= list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        else
        {
            return list[index].Value;
        }
    }

    /// <summary>
    /// Returns value at specified index in an IDictionary(Of TKey, TValue).
    /// </summary>
    /// <typeparam name="TKey">Type of keys in IDictionary(Of TKey, TValue)</typeparam>
    /// <typeparam name="TValue">Type of values in IDictionary(Of TKey, TValue)</typeparam>
    /// <param name="list">IDictionary(Of TKey, TValue) to search.</param>
    /// <param name="index">Index within IDictionary(Of TKey, TValue)</param>
    /// <returns>Value at specified index.</returns>
    /// <remarks>Returns default value of TValue if index is out of bounds.</remarks>
    public static TValue? ValueAtOrDefault<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, int index)
    {
        if (index >= list.Count)
        {
            return default;
        }
        else
        {
            return list[index].Value;
        }
    }

    /// <summary>
    /// Returns key at specified index in an IDictionary(Of TKey, TValue).
    /// </summary>
    /// <typeparam name="TKey">Type of keys in IDictionary(Of TKey, TValue)</typeparam>
    /// <typeparam name="TValue">Type of values in IDictionary(Of TKey, TValue)</typeparam>
    /// <param name="list">IDictionary(Of TKey, TValue) to search.</param>
    /// <param name="index">Index within IDictionary(Of TKey, TValue)</param>
    /// <returns>Value at specified index.</returns>
    /// <remarks>Throws an exception if index is out of bounds.</remarks>
    public static TKey KeyAt<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, int index)
    {
        if (index >= list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        else
        {
            return list[index].Key;
        }
    }

    /// <summary>
    /// Returns key at specified index in an IDictionary(Of TKey, TValue).
    /// </summary>
    /// <typeparam name="TKey">Type of keys in IDictionary(Of TKey, TValue)</typeparam>
    /// <typeparam name="TValue">Type of values in IDictionary(Of TKey, TValue)</typeparam>
    /// <param name="list">IDictionary(Of TKey, TValue) to search.</param>
    /// <param name="index">Index within IDictionary(Of TKey, TValue)</param>
    /// <returns>Key at specified index.</returns>
    /// <remarks>Returns default value of TKey if index is out of bounds.</remarks>
    public static TKey? KeyAtOrDefault<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, int index)
    {
        if (index >= list.Count)
        {
            return default;
        }
        else
        {
            return list[index].Key;
        }
    }

    /// <summary>
    /// Returns KeyValuePair(Of TKey, TValue) at specified index in an IDictionary(Of TKey, TValue).
    /// </summary>
    /// <typeparam name="TKey">Type of keys in IDictionary(Of TKey, TValue)</typeparam>
    /// <typeparam name="TValue">Type of values in IDictionary(Of TKey, TValue)</typeparam>
    /// <param name="list">IDictionary(Of TKey, TValue) to search.</param>
    /// <param name="index">Index within IDictionary(Of TKey, TValue)</param>
    /// <returns>KeyValuePair(Of TKey, TValue) at specified index.</returns>
    /// <remarks>Throws an exception if index is out of bounds.</remarks>
    public static KeyValuePair<TKey, TValue> KeyValuePairAt<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, int index)
    {
        if (index >= list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        else
        {
            return list[index];
        }
    }

    /// <summary>
    /// Returns KeyValuePair(Of TKey, TValue) at specified index in an IDictionary(Of TKey, TValue).
    /// </summary>
    /// <typeparam name="TKey">Type of keys in IDictionary(Of TKey, TValue)</typeparam>
    /// <typeparam name="TValue">Type of values in IDictionary(Of TKey, TValue)</typeparam>
    /// <param name="list">IDictionary(Of TKey, TValue) to search.</param>
    /// <param name="index">Index within IDictionary(Of TKey, TValue)</param>
    /// <returns>KeyValuePair(Of TKey, TValue) at specified index.</returns>
    /// <remarks>Returns default value of KeyValuePair(Of TKey, TValue) if index is out of bounds.</remarks>
    public static KeyValuePair<TKey, TValue> KeyValuePairAtOrDefault<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> list, int index)
    {
        if (index >= list.Count)
        {
            return default;
        }
        else
        {
            return list[index];
        }
    }

    /// <summary>
    /// Merges two arrays to a new array.
    /// </summary>
    public static T[] Extend<T>(this T[] a1, T[] a2)
    {
        if (a1 is null)
        {
            return a2;
        }
        else if (a2 is null)
        {
            return a1;
        }
        else
        {
            var a2Index = a1.Length;
            Array.Resize(ref a1, a1.Length + a2.Length);
            Array.Copy(a2, 0, a1, a2Index, a2.Length);
            return a1;
        }
    }

    /// <summary>
    /// Copies elements to another IList(Of T).
    /// </summary>
    /// <typeparam name="T">Type of elements</typeparam>
    /// <param name="source">List to copy from.</param>
    /// <param name="target">List to copy to.</param>
    public static void CopyTo<T>(this List<T> source, IList<T> target)
        => source.ForEach(target.Add);

    /// <summary>
    /// Copies elements to another IList(Of T).
    /// </summary>
    /// <typeparam name="T">Type of elements</typeparam>
    /// <param name="source">List to copy from.</param>
    /// <param name="target">List to copy to.</param>
    public static void CopyTo<T>(this T[] source, IList<T> target)
        => Array.ForEach(source, target.Add);

    /// <summary>
    /// Adds a value to a dictionary and returns key selected for value. Keys are automatically
    /// selected for added values.
    /// </summary>
    /// <typeparam name="T">Type of values in dictionary.</typeparam>
    /// <param name="dict">Dictionary object.</param>
    /// <param name="value">Value to add to dictionary.</param>
    /// <returns>Selected key for value.</returns>
    public static int AddWithNewKey<T>(this IDictionary<int, T> dict, T value)
    {
        for (var i = 1; i <= int.MaxValue; i++)
        {
            if (!dict.ContainsKey(i))
            {
                dict.Add(i, value);
                return i;
            }
        }

        throw new IOException("No more keys available.");
    }

    /// <summary>
    /// Adds a value to a dictionary and returns key selected for value. Keys are automatically
    /// selected for added values.
    /// </summary>
    /// <typeparam name="T">Type of values in dictionary.</typeparam>
    /// <param name="dict">Dictionary object.</param>
    /// <param name="value">Value to add to dictionary.</param>
    /// <returns>Selected key for value.</returns>
    public static long AddWithNewKey<T>(this IDictionary<long, T> dict, T value)
    {
        for (var i = 1L; i <= long.MaxValue; i++)
        {
            if (!dict.ContainsKey(i))
            {
                dict.Add(i, value);
                return i;
            }
        }

        throw new IOException("No more keys available.");
    }

#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static T Second<T>(this IEnumerable<T> seq) => seq.ElementAt(1);

    public static T? SecondOrDefault<T>(this IEnumerable<T> seq) => seq.ElementAtOrDefault(1);

#endif

    public static void Dispose<T>(this ICollection<T> list) where T : IDisposable
    {
        foreach (var obj in list)
        {
            obj.Dispose();
        }

        if (!list.IsReadOnly)
        {
            list.Clear();
        }
    }

    public static void Dispose<TKey, TValue>(this IDictionary<TKey, TValue> dict) where TValue : IDisposable
    {
        foreach (var obj in dict.Values)
        {
            obj.Dispose();
        }

        if (!dict.IsReadOnly)
        {
            dict.Clear();
        }
    }

#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP

    public static T? MemberwiseMerge<T>(params T[] sequence) where T : class => MemberwiseMerger<T>.MergeSequence(sequence);

    public static T? MemberwiseMerge<T>(this IEnumerable<T> sequence) where T : class => MemberwiseMerger<T>.MergeSequence(sequence);

#endif
}