#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace LTRData.Extensions.Buffers;

internal sealed class SafeBufferMemoryManager<T>(SafeBuffer safeBuffer, bool ownsBuffer) : MemoryManager<T> where T : unmanaged
{
    private bool _disposed;

    public unsafe int Count => (int)(safeBuffer.ByteLength / (ulong)sizeof(T));

    public override unsafe Span<T> GetSpan()
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed || safeBuffer.IsClosed, this);
#else
        if (_disposed || safeBuffer.IsClosed)
        {
            throw new ObjectDisposedException(nameof(SafeBufferMemoryManager<T>));
        }
#endif

        return new((T*)safeBuffer.DangerousGetHandle(), Count);
    }

    public override unsafe MemoryHandle Pin(int elementIndex = 0)
    {
#if NET7_0_OR_GREATER
        ObjectDisposedException.ThrowIf(_disposed || safeBuffer.IsClosed, this);
#else
        if (_disposed || safeBuffer.IsClosed)
        {
            throw new ObjectDisposedException(nameof(SafeBufferMemoryManager<T>));
        }
#endif

#if NET8_0_OR_GREATER
        ArgumentOutOfRangeException.ThrowIfNegative(elementIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(elementIndex, Count);
#else
        if (elementIndex < 0 || elementIndex >= Count)
        {
            throw new ArgumentOutOfRangeException(nameof(elementIndex));
        }
#endif

        byte* pointer = null;

        safeBuffer.AcquirePointer(ref pointer);

        return new(pointer: ((T*)pointer) + elementIndex, handle: default, pinnable: this);
    }

    public override void Unpin()
    {
        safeBuffer.ReleasePointer();
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing && ownsBuffer)
            {
                safeBuffer.Dispose();
            }

            _disposed = true;
        }
    }

    public override unsafe string ToString()
        => $"{typeof(T).Name} 0x{safeBuffer.DangerousGetHandle():x}[{Count}]";
}

#endif
