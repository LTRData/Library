using System;
using System.Collections;
using System.Collections.Generic;

namespace LTRData.Extensions.Buffers;

/// <summary>
/// Provides allocation-free enumerables for a single item
/// </summary>
public static class SingleValueEnumerable
{
    /// <summary>
    /// Returns an allocation-free enumerable for a single item
    /// </summary>
    /// <param name="value">Item to enumerate once</param>
    /// <returns>Enumerable that will enumerate supplied item once</returns>
    public static SingleValueEnumerable<T?> Get<T>(T? value) => new(value);
}

/// <summary>
/// An allocation-free enumerable for a single item
/// </summary>
/// <typeparam name="T">Type of item</typeparam>
/// <param name="value">Value to enumerate once</param>
public readonly struct SingleValueEnumerable<T>(T value) : IEnumerable<T?>
{
    /// <summary>
    /// Value to enumerate once
    /// </summary>
    public T? Value { get; } = value;

    /// <summary>
    /// Returns enumerator for this enumerable
    /// </summary>
    /// <returns>Enumerator for this enumerable</returns>
    public IEnumerator<T?> GetEnumerator() => new SingleValueEnumerator(Value);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// An allocation-free enumerator for a single item
    /// </summary>
    public struct SingleValueEnumerator : IEnumerator<T?>
    {
        /// <summary>
        /// Initializes a new enumerator with an item
        /// </summary>
        /// <param name="value"></param>
        public SingleValueEnumerator(T? value) : this()
        {
            Value = value;
        }

        /// <summary>
        /// Value to enumerate once
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Indicates whether enumeration has started
        /// </summary>
        public bool Started { get; private set; }

        /// <summary>
        /// Current value in enumeration
        /// </summary>
        public readonly T? Current => Started ? Value : default;

        readonly object? IEnumerator.Current => Current;

        readonly void IDisposable.Dispose() { }

        /// <summary>
        /// Moves to next value in enumeration
        /// </summary>
        /// <returns>True if a new item is available from enumeration, false if end has been reached</returns>
        public bool MoveNext()
        {
            if (Started)
            {
                return false;
            }

            Started = true;
            return true;
        }

        /// <summary>
        /// Restarts enumeration from beginning
        /// </summary>
        public void Reset() => Started = false;
    }
}
