using LTRData.Extensions.Buffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LTRData.Extensions.Native;

public static partial class NativeCompareExtensions
{
#if NETCOREAPP
    static NativeCompareExtensions()
    {
        NativeLibrary.SetDllImportResolver(typeof(NativeCompareExtensions).Assembly, NativeLib.CrtDllImportResolver);
    }
#endif

#if NET7_0_OR_GREATER
    [LibraryImport("msvcrt", SetLastError = false)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(CallConvCdecl) })]
    private static partial int memcmp(in byte ptr1, in byte ptr2, nint count);
#else
    [DllImport("msvcrt", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
    private static extern int memcmp(in byte ptr1, in byte ptr2, nint count);
#endif

    /// <summary>
    /// Compares two byte spans using C runtime memcmp function.
    /// </summary>
    /// <param name="first">First span</param>
    /// <param name="second">Second span</param>
    /// <returns>If sequences are both empty, true is returned. If sequences have different lengths, false is returned.
    /// If lengths are equal and byte sequences are equal, true is returned.</returns>
    public static bool BinaryEqual(this ReadOnlySpan<byte> first, ReadOnlySpan<byte> second)
    {
        if (first.IsEmpty && second.IsEmpty)
        {
            return true;
        }

        return first.Length == second.Length
            && (first == second ||
            memcmp(first[0], second[0], first.Length) == 0);
    }

    /// <summary>
    /// Compares two byte spans using C runtime memcmp function.
    /// </summary>
    /// <param name="first">First span</param>
    /// <param name="second">Second span</param>
    /// <returns>If sequences are both empty, true is returned. If sequences have different lengths, false is returned.
    /// If lengths are equal and byte sequences are equal, true is returned.</returns>
    public static bool BinaryEqual(this Span<byte> first, ReadOnlySpan<byte> second)
    {
        if (first.IsEmpty && second.IsEmpty)
        {
            return true;
        }

        return first.Length == second.Length
            && (first == second ||
            memcmp(first[0], second[0], first.Length) == 0);
    }

    /// <summary>
    /// Compares two byte spans using C runtime memcmp function.
    /// </summary>
    /// <param name="first">First span</param>
    /// <param name="second">Second span</param>
    /// <returns>Result of memcmp comparison.</returns>
    public static int BinaryCompare(this ReadOnlySpan<byte> first, ReadOnlySpan<byte> second)
        => (first.IsEmpty && second.IsEmpty) || (first == second)
        ? 0 : memcmp(first[0], second[0], first.Length);

    /// <summary>
    /// Compares two byte spans using C runtime memcmp function.
    /// </summary>
    /// <param name="first">First span</param>
    /// <param name="second">Second span</param>
    /// <returns>Result of memcmp comparison.</returns>
    public static int BinaryCompare(this Span<byte> first, ReadOnlySpan<byte> second)
        => (first.IsEmpty && second.IsEmpty) || (first == second)
        ? 0 : memcmp(first[0], second[0], first.Length);

    /// <summary>
    /// Compares two spans using C runtime memcmp function.
    /// </summary>
    /// <param name="first">First span</param>
    /// <param name="second">Second span</param>
    /// <returns>If sequences are both empty, true is returned. If sequences have different lengths, false is returned.
    /// If lengths are equal and byte sequences are equal, true is returned.</returns>
    public static bool BinaryEqual<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second) where T : unmanaged
        => BinaryEqual(MemoryMarshal.AsBytes(first), MemoryMarshal.AsBytes(second));

    /// <summary>
    /// Compares two spans using C runtime memcmp function.
    /// </summary>
    /// <param name="first">First span</param>
    /// <param name="second">Second span</param>
    /// <returns>Result of memcmp comparison.</returns>
    public static int BinaryCompare<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second) where T : unmanaged
        => BinaryCompare(MemoryMarshal.AsBytes(first), MemoryMarshal.AsBytes(second));

    [return: MarshalAs(UnmanagedType.I1)]
    private delegate bool RtlIsZeroMemoryFunc(in byte buffer, nint length);

    private static readonly RtlIsZeroMemoryFunc FuncRtlIsZeroMemory =
        GetRtlIsZeroMemory() ??
        InternalIsZeroMemory;

    private static unsafe RtlIsZeroMemoryFunc? GetRtlIsZeroMemory()
    {
        nint fptr = 0;

#if NETCOREAPP
        try
        {
            fptr = NativeLib.GetProcAddressNoThrow("ntdll", "RtlIsZeroMemory");
        }
        catch
        {
        }
#endif

        if (fptr == default)
        {
            return null;
        }

        var ptr = (delegate* unmanaged[Stdcall]<byte*, nint, byte>)fptr;

        return (in byte buffer, nint length) =>
        {
            fixed (byte* bytes = &buffer)
            {
                return ptr(bytes, length) != 0;
            }
        };
    }

    /// <summary>
    /// Determines whether all bytes in a buffer are zero. If ntdll.RtlIsZeroMemory is available it is used,
    /// otherwise it falls back to a native method that compares groups of bytes is an optimized way.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns>If all bytes are zero, buffer is empty or buffer is null, true is returned, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBufferZero(this byte[] buffer) => buffer is null || buffer.AsSpan().IsBufferZero();

    /// <summary>
    /// Determines whether all bytes in a buffer are zero. If ntdll.RtlIsZeroMemory is available it is used,
    /// otherwise it falls back to a native method that compares groups of bytes is an optimized way.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns>If all bytes are zero, buffer is empty, true is returned, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBufferZero(this Span<byte> buffer) =>
        FuncRtlIsZeroMemory(MemoryMarshal.GetReference(buffer), buffer.Length);

    /// <summary>
    /// Determines whether all bytes in a buffer are zero. If ntdll.RtlIsZeroMemory is available it is used,
    /// otherwise it falls back to a managed method that compares groups of bytes is an optimized way.
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns>If all bytes are zero, buffer is empty, true is returned, false otherwise.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBufferZero(this ReadOnlySpan<byte> buffer) =>
        FuncRtlIsZeroMemory(MemoryMarshal.GetReference(buffer), buffer.Length);

    private static unsafe bool InternalIsZeroMemory(in byte buffer, nint length)
    {
        if (length == 0)
        {
            return true;
        }

        fixed (byte* ptr = &buffer)
        {
            var pointervalue = (nint)ptr;

            if ((pointervalue & sizeof(long) - 1) == 0 &&
                (length & sizeof(long) - 1) == 0)
            {
                for (var p = (long*)ptr; p < ptr + length; p++)
                {
                    if (*p != 0)
                    {
                        return false;
                    }
                }
            }
            else if ((pointervalue & sizeof(int) - 1) == 0 &&
                (length & sizeof(int) - 1) == 0)
            {
                for (var p = (int*)ptr; p < ptr + length; p++)
                {
                    if (*p != 0)
                    {
                        return false;
                    }
                }
            }
            else if ((pointervalue & sizeof(short) - 1) == 0 &&
                (length & sizeof(short) - 1) == 0)
            {
                for (var p = (short*)ptr; p < ptr + length; p++)
                {
                    if (*p != 0)
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (var p = ptr; p < ptr + length; p++)
                {
                    if (*p != 0)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int BinaryCompare<T>(in T s1, in T s2) where T : unmanaged =>
        BinaryCompare(BufferExtensions.CreateReadOnlySpan(s1, 1), BufferExtensions.CreateReadOnlySpan(s2, 1));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool BinaryEqual<T>(in T s1, in T s2) where T : unmanaged =>
        BinaryEqual(BufferExtensions.CreateReadOnlySpan(s1, 1), BufferExtensions.CreateReadOnlySpan(s2, 1));
}
