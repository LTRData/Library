#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Buffers;
using System.IO;

namespace LTRData.Extensions.Native.Memory;

/// <summary>
/// Represents unmanaged memory managed by native code
/// </summary>
/// <typeparam name="T">Type of elements in the memory</typeparam>
public readonly struct NativeMemory<T>(nint address, int length) where T : unmanaged
{

    /// <summary>
    /// Unmanaged pointer to memory.
    /// </summary>
    public nint Address { get; } = address;

    /// <summary>
    /// Number of elements at memory address.
    /// </summary>
    public int Length { get; } = length;

    /// <summary>
    /// Return value indicating whether this object represents an unmanaged NULL pointer.
    /// </summary>
    public bool IsNull => Address == 0;

    /// <summary>
    /// Return value indicating whether this object represents an unmanaged NULL pointer
    /// or zero-length memory.
    /// </summary>
    public bool IsEmpty => Length == 0;

    /// <summary>
    /// Gets a <see cref="Span{T}"/> for this memory block.
    /// </summary>
    public unsafe Span<T> Span => new((T*)Address, Length);

    /// <summary>
    /// Gets a disposable <see cref="MemoryManager{T}"/> for this memory block. This can
    /// be used to get a <see cref="Memory{T}"/> that can be sent to asynchronous API or
    /// delegates. Remember though, that the memory could be invalid after return to
    /// native caller, so make sure that no asynchronous operations use the memory after
    /// returning from consuming methods.
    /// </summary>
    public MemoryManager<T> GetMemoryManager()
        => new UnmanagedMemoryManager<T>(Address, Length);

    /// <summary>
    /// Gets a disposable <see cref="UnmanagedMemoryStream"/> for this memory block.
    /// Remember though, that the memory is invalid after return to native caller, so
    /// make sure that no asynchronous operations use the memory after returning from
    /// consuming methods.
    /// </summary>
    public unsafe UnmanagedMemoryStream GetStream()
        => new((byte*)Address, Length * sizeof(T));

    /// <summary>
    /// Rents an array from shared array pool for use when arrays are needed. Note that
    /// returned array could be larger than length of original native buffer, the Length
    /// property of returned <see cref="ArraySegment{T}"/> indicates how much of the array
    /// can be used for later copying back to native buffer.
    /// 
    /// After use, the returned array should be returned to the array pool again by calling
    /// <see cref="ReturnArray" /> method. This will also copy array contents back to native
    /// buffer so that calling native library receives the data written to array.
    /// </summary>
    /// <returns><see cref="ArraySegment{T}"/> containing a reference to rented array
    /// and useful length of the array.</returns>
    public ArraySegment<T> RentArray()
    {
        var array = ArrayPool<T>.Shared.Rent(Length);
        Array.Clear(array, 0, Length);
        return new(array, 0, Length);
    }

    /// <summary>
    /// Returns an array to shared array pool that was previously returned by <see cref="RentArray"/>
    /// and copies array contents to original native buffer so that native caller receives the
    /// data written to the array.
    /// </summary>
    /// <param name="array">Array returned by <see cref="RentArray"/></param>
    /// <param name="clearArray">Indicates whether the contents of the buffer should be cleared before reuse.</param>
    public void ReturnArray(T[] array, bool clearArray = false)
    {
        array.AsSpan(0, Length).CopyTo(Span);
        ArrayPool<T>.Shared.Return(array, clearArray);
    }

    /// <summary>
    /// Returns a string describing the native buffer. If element type is <see cref="char"/>,
    /// a string with the same characters as in original buffer is returned.
    /// </summary>
    /// <returns>String describing the native buffer</returns>
    public override unsafe string ToString()
    {
        if (Address == 0)
        {
            return "<null>";
        }

        if (typeof(T) == typeof(char))
        {
            return NativeMemoryHelper.GetStringFromSpan(new ReadOnlySpan<char>((char*)Address, Length));
        }

        return $"{typeof(T).Name} 0x{Address:x}[{Length}]";
    }

    /// <summary>
    /// Returns a read-only representation of the same memory block as current.
    /// </summary>
    /// <returns>An <see cref="ReadOnlyNativeMemory{T}"/> structure.</returns>
    public ReadOnlyNativeMemory<T> AsReadOnly() => new(Address, Length);

    /// <summary>
    /// Forms a slice out of the current memory block that begins at a specified index.
    /// </summary>
    /// <param name="start">The index at which to begin the slice.</param>
    /// <returns>A span that consists of all elements of the current memory block from start to the end of the memory block.</returns>
    /// <exception cref="ArgumentOutOfRangeException">start is less than zero or greater than <see cref="Length" />.</exception>
    public NativeMemory<T> Slice(int start)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(start, Length);
#else
        if (start < 0 || start > Length)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }
#endif

        return new(Address + start, Length - start);
    }

    /// <summary>
    /// Forms a slice out of the current memory block starting at a specified index for a specified length.
    /// </summary>
    /// <param name="start">The index at which to begin the slice.</param>
    /// <param name="length">The desired length for the slice.</param>
    /// <returns>A memory block that consists of length elements of the current memory block from start to the end of the memory block.</returns>
    /// <exception cref="ArgumentOutOfRangeException">start or start + length is less than zero or greater than <see cref="Length" />.</exception>
    public NativeMemory<T> Slice(int start, int length)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(start, Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(length, Length - start);
#else
        if (start < 0 || length < 0 || start > Length || length > Length - start)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }
#endif

        return new(Address + start, length);
    }
}

/// <summary>
/// Represents read only unmanaged memory managed by native code
/// </summary>
/// <typeparam name="T">Type of elements in the memory</typeparam>
public readonly struct ReadOnlyNativeMemory<T>(nint address, int length) where T : unmanaged
{
    /// <summary>
    /// Converts existing <see cref="NativeMemory{T}"/> to a <see cref="ReadOnlyNativeMemory{T}"/>
    /// </summary>
    /// <param name="origin">Source <see cref="NativeMemory{T}"/></param>
    /// <returns>New <see cref="ReadOnlyNativeMemory{T}"/></returns>
    public static implicit operator ReadOnlyNativeMemory<T>(NativeMemory<T> origin)
        => new(origin.Address, origin.Length);

    /// <summary>
    /// Unmanaged pointer to memory.
    /// </summary>
    public nint Address { get; } = address;

    /// <summary>
    /// Number of elements at memory address.
    /// </summary>
    public int Length { get; } = length;

    /// <summary>
    /// Return value indicating whether this object represents an unmanaged NULL pointer.
    /// </summary>
    public bool IsNull => Address == 0;

    /// <summary>
    /// Return value indicating whether this object represents an unmanaged NULL pointer
    /// or zero-length memory.
    /// </summary>
    public bool IsEmpty => Length == 0;

    /// <summary>
    /// Gets a <see cref="ReadOnlySpan{T}"/> for this memory block.
    /// </summary>
    public unsafe ReadOnlySpan<T> Span => new((T*)Address, Length);

    /// <summary>
    /// Gets a disposable <see cref="MemoryManager{T}"/> for this memory block. This can
    /// be used to get a <see cref="Memory{T}"/> that can be sent to asynchronous API or
    /// delegates. Remember though, that the memory is invalid after return to native
    /// caller, so make sure that no asynchronous operations use the memory after
    /// returning from implementation methods.
    /// </summary>
    public MemoryManager<T> GetMemoryManager()
        => new UnmanagedMemoryManager<T>(Address, Length);

    /// <summary>
    /// Gets a disposable <see cref="UnmanagedMemoryStream"/> for this memory block.
    /// Remember though, that the memory is invalid after return to native caller, so
    /// make sure that no asynchronous operations use the memory after returning from
    /// implementation methods.
    /// </summary>
    public unsafe UnmanagedMemoryStream GetStream()
        => new((byte*)Address, Length * sizeof(T), Length * sizeof(T), FileAccess.Read);

    /// <summary>
    /// Rents an array from shared array pool for use when arrays are needed. Data from
    /// original native buffer is copied into the array. Note that returned array could
    /// be larger than length of original native buffer, the Length property of returned
    /// <see cref="ArraySegment{T}"/> indicates how much of the array contains valid data.
    /// 
    /// After use, the returned array should be returned to the array pool again by calling
    /// <see cref="ReturnArray" /> method.
    /// </summary>
    /// <returns><see cref="ArraySegment{T}"/> containing a reference to rented array
    /// and length of valid data in the array.</returns>
    public ArraySegment<T> RentArray()
    {
        var array = ArrayPool<T>.Shared.Rent(Length);
        Span.CopyTo(array);
        return new(array, 0, Length);
    }

    /// <summary>
    /// Returns an array to shared array pool that was previously returned by <see cref="RentArray"/>
    /// </summary>
    /// <param name="array">Array returned by <see cref="RentArray"/></param>
    /// <param name="clearArray">Indicates whether the contents of the buffer should be cleared before reuse.</param>
    public void ReturnArray(T[] array, bool clearArray = false)
        => ArrayPool<T>.Shared.Return(array, clearArray);

    /// <summary>
    /// Returns a string describing the native buffer. If element type is <see cref="char"/>,
    /// a string with the same characters as in original buffer is returned.
    /// </summary>
    /// <returns>String describing the native buffer</returns>
    public override unsafe string ToString()
    {
        if (Address == 0)
        {
            return "<null>";
        }

        if (typeof(T) == typeof(char))
        {
            return NativeMemoryHelper.GetStringFromSpan(new ReadOnlySpan<char>((char*)Address, Length));
        }

        return $"{typeof(T).Name} 0x{Address:x}[{Length}]";
    }

    /// <summary>
    /// Forms a slice out of the current memory block that begins at a specified index.
    /// </summary>
    /// <param name="start">The index at which to begin the slice.</param>
    /// <returns>A span that consists of all elements of the current memory block from start to the end of the memory block.</returns>
    /// <exception cref="ArgumentOutOfRangeException">start is less than zero or greater than <see cref="Length" />.</exception>
    public ReadOnlyNativeMemory<T> Slice(int start)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(start, Length);
#else
        if (start < 0 || start > Length)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }
#endif

        return new(Address + start, Length - start);
    }

    /// <summary>
    /// Forms a slice out of the current memory block starting at a specified index for a specified length.
    /// </summary>
    /// <param name="start">The index at which to begin the slice.</param>
    /// <param name="length">The desired length for the slice.</param>
    /// <returns>A memory block that consists of length elements of the current memory block from start to the end of the memory block.</returns>
    /// <exception cref="ArgumentOutOfRangeException">start or start + length is less than zero or greater than <see cref="Length" />.</exception>
    public ReadOnlyNativeMemory<T> Slice(int start, int length)
    {
#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(start);
        ArgumentOutOfRangeException.ThrowIfNegative(length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(start, Length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(length, Length - start);
#else
        if (start < 0 || length < 0 || start > Length || length > Length - start)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }
#endif

        return new(Address + start, length);
    }
}

internal sealed class UnmanagedMemoryManager<T>(nint address, int count) : MemoryManager<T> where T : unmanaged
{
    private bool _disposed;

    public override unsafe Span<T> GetSpan()
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(UnmanagedMemoryManager<>));
        }
#endif

        return new((T*)address, count);
    }

    public override unsafe MemoryHandle Pin(int elementIndex = 0)
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed, this);
#else
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(UnmanagedMemoryManager<>));
        }
#endif

#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfLessThan(elementIndex, 0);

        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(elementIndex, count);
#else
        if (elementIndex < 0 || elementIndex >= count)
        {
            throw new ArgumentOutOfRangeException(nameof(elementIndex));
        }
#endif

        var pointer = address + elementIndex;
        return new MemoryHandle((T*)pointer, default, this);
    }

    public override void Unpin()
    {
        // No need to do anything, since we're dealing with unmanaged memory.
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            address = 0;
            count = 0;
            _disposed = true;
        }
    }

    public override unsafe string ToString()
    {
        if (address == 0)
        {
            return "<null>";
        }

        if (typeof(T) == typeof(char))
        {
            return NativeMemoryHelper.GetStringFromSpan(new ReadOnlySpan<char>((char*)address, count));
        }

        return $"{typeof(T).Name} 0x{address:x}[{count}]";
    }
}

#endif
