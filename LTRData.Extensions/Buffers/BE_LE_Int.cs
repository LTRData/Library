#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Runtime.CompilerServices;

namespace LTRData.Extensions.Buffers;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public readonly struct BEInt16 : IEquatable<BEInt16>, IFormattable
#if NET6_0_OR_GREATER
, ISpanFormattable
#endif
{
    private readonly byte _byte0;
    private readonly byte _byte1;

    public BEInt16(short value)
    {
        if (BitConverter.IsLittleEndian)
        {
            _byte1 = (byte)value;
            _byte0 = (byte)(value >> (8 * 1));
        }
        else
        {
            Unsafe.As<byte, short>(ref _byte0) = value;
        }
    }

    public static implicit operator BEInt16(short value) => new(value);

    public static implicit operator short(BEInt16 value)
        => BitConverter.IsLittleEndian
        ? (short)((value._byte0 << (8 * 1)) | value._byte1)
        : Unsafe.As<byte, short>(ref Unsafe.AsRef(in value._byte0));

    public readonly bool Equals(BEInt16 other)
        => Unsafe.As<byte, short>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, short>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is BEInt16 value && Equals(value);

    public override readonly int GetHashCode() => this;

    public static bool operator ==(BEInt16 left, BEInt16 right) => left.Equals(right);

    public static bool operator !=(BEInt16 left, BEInt16 right) => !(left == right);

    public override readonly string ToString() => ((short)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((short)this).ToString(format, formatProvider);

#if NET6_0_OR_GREATER
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((short)this).TryFormat(destination, out charsWritten, format, provider);
#endif
}

public readonly struct BEUInt16 : IEquatable<BEUInt16>, IFormattable
#if NET6_0_OR_GREATER
, ISpanFormattable
#endif
{
    private readonly byte _byte0;
    private readonly byte _byte1;

    public BEUInt16(ushort value)
    {
        if (BitConverter.IsLittleEndian)
        {
            _byte1 = (byte)value;
            _byte0 = (byte)(value >> (8 * 1));
        }
        else
        {
            Unsafe.As<byte, ushort>(ref _byte0) = value;
        }
    }

    public static implicit operator BEUInt16(ushort value) => new(value);

    public static implicit operator ushort(BEUInt16 value)
        => BitConverter.IsLittleEndian
        ? (ushort)((value._byte0 << (8 * 1)) | value._byte1)
        : Unsafe.As<byte, ushort>(ref Unsafe.AsRef(in value._byte0));

    public readonly bool Equals(BEUInt16 other)
        => Unsafe.As<byte, ushort>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, ushort>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is BEUInt16 value && Equals(value);

    public override readonly int GetHashCode() => this;

    public static bool operator ==(BEUInt16 left, BEUInt16 right) => left.Equals(right);

    public static bool operator !=(BEUInt16 left, BEUInt16 right) => !(left == right);

    public override readonly string ToString() => ((ushort)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((ushort)this).ToString(format, formatProvider);

#if NET6_0_OR_GREATER
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((ushort)this).TryFormat(destination, out charsWritten, format, provider);
#endif
}

public readonly struct LEInt16 : IEquatable<LEInt16>, IFormattable
#if NET6_0_OR_GREATER
, ISpanFormattable
#endif
{
    private readonly byte _byte0;
    private readonly byte _byte1;

    public LEInt16(short value)
    {
        if (BitConverter.IsLittleEndian)
        {
            Unsafe.As<byte, short>(ref _byte0) = value;
        }
        else
        {
            _byte0 = (byte)value;
            _byte1 = (byte)(value >> (8 * 1));
        }
    }

    public static implicit operator LEInt16(short value) => new(value);

    public static implicit operator short(LEInt16 value)
        => BitConverter.IsLittleEndian
        ? Unsafe.As<byte, short>(ref Unsafe.AsRef(in value._byte0))
        : (short)((value._byte1 << (8 * 1)) | value._byte0);

    public readonly bool Equals(LEInt16 other)
        => Unsafe.As<byte, short>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, short>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is LEInt16 value && Equals(value);

    public override readonly int GetHashCode() => this;

    public static bool operator ==(LEInt16 left, LEInt16 right) => left.Equals(right);

    public static bool operator !=(LEInt16 left, LEInt16 right) => !(left == right);

    public override readonly string ToString() => ((short)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((short)this).ToString(format, formatProvider);

#if NET6_0_OR_GREATER
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((short)this).TryFormat(destination, out charsWritten, format, provider);
#endif
}

public readonly struct LEUInt16 : IEquatable<LEUInt16>, IFormattable
#if NET6_0_OR_GREATER
, ISpanFormattable
#endif
{
    private readonly byte _byte0;
    private readonly byte _byte1;

    public LEUInt16(ushort value)
    {
        if (BitConverter.IsLittleEndian)
        {
            Unsafe.As<byte, ushort>(ref _byte0) = value;
        }
        else
        {
            _byte0 = (byte)value;
            _byte1 = (byte)(value >> (8 * 1));
        }
    }

    public static implicit operator LEUInt16(ushort value) => new(value);

    public static implicit operator ushort(LEUInt16 value)
        => BitConverter.IsLittleEndian
        ? Unsafe.As<byte, ushort>(ref Unsafe.AsRef(in value._byte0))
        : (ushort)((value._byte1 << (8 * 1)) | value._byte0);

    public readonly bool Equals(LEUInt16 other)
        => Unsafe.As<byte, ushort>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, ushort>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is LEUInt16 value && Equals(value);

    public override readonly int GetHashCode() => this;

    public static bool operator ==(LEUInt16 left, LEUInt16 right) => left.Equals(right);

    public static bool operator !=(LEUInt16 left, LEUInt16 right) => !(left == right);

    public override readonly string ToString() => ((ushort)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((ushort)this).ToString(format, formatProvider);

#if NET6_0_OR_GREATER
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((ushort)this).TryFormat(destination, out charsWritten, format, provider);
#endif
}

public readonly struct BEInt32 : IEquatable<BEInt32>, IFormattable
#if NET6_0_OR_GREATER
, ISpanFormattable
#endif
{
    private readonly byte _byte0;
    private readonly byte _byte1;
    private readonly byte _byte2;
    private readonly byte _byte3;

    public BEInt32(int value)
    {
        if (BitConverter.IsLittleEndian)
        {
            _byte3 = (byte)value;
            _byte2 = (byte)(value >> (8 * 1));
            _byte1 = (byte)(value >> (8 * 2));
            _byte0 = (byte)(value >> (8 * 3));
        }
        else
        {
            Unsafe.As<byte, int>(ref _byte0) = value;
        }
    }

    public static implicit operator BEInt32(int value) => new(value);

    public static implicit operator int(BEInt32 value)
        => BitConverter.IsLittleEndian
        ? (value._byte0 << (8 * 3))
        | (value._byte1 << (8 * 2))
        | (value._byte2 << (8 * 1))
        | value._byte3
        : Unsafe.As<byte, int>(ref Unsafe.AsRef(in value._byte0));

    public readonly bool Equals(BEInt32 other)
        => Unsafe.As<byte, int>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, int>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is BEInt32 value && Equals(value);

    public override readonly int GetHashCode() => this;

    public static bool operator ==(BEInt32 left, BEInt32 right) => left.Equals(right);

    public static bool operator !=(BEInt32 left, BEInt32 right) => !(left == right);

    public override readonly string ToString() => ((int)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((int)this).ToString(format, formatProvider);

#if NET6_0_OR_GREATER
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((ushort)this).TryFormat(destination, out charsWritten, format, provider);
#endif
}

public readonly struct BEUInt32 : IEquatable<BEUInt32>, IFormattable
#if NET6_0_OR_GREATER
, ISpanFormattable
#endif
{
    private readonly byte _byte0;
    private readonly byte _byte1;
    private readonly byte _byte2;
    private readonly byte _byte3;

    public BEUInt32(uint value)
    {
        if (BitConverter.IsLittleEndian)
        {
            _byte3 = (byte)value;
            _byte2 = (byte)(value >> (8 * 1));
            _byte1 = (byte)(value >> (8 * 2));
            _byte0 = (byte)(value >> (8 * 3));
        }
        else
        {
            Unsafe.As<byte, uint>(ref _byte0) = value;
        }
    }

    public static implicit operator BEUInt32(uint value) => new(value);

    public static implicit operator uint(BEUInt32 value)
        => BitConverter.IsLittleEndian
        ? ((uint)value._byte0 << (8 * 3))
        | ((uint)value._byte1 << (8 * 2))
        | ((uint)value._byte2 << (8 * 1))
        | value._byte3
        : Unsafe.As<byte, uint>(ref Unsafe.AsRef(in value._byte0));

    public readonly bool Equals(BEUInt32 other)
        => Unsafe.As<byte, uint>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, uint>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is BEUInt32 value && Equals(value);

    public override readonly int GetHashCode() => ((uint)this).GetHashCode();

    public static bool operator ==(BEUInt32 left, BEUInt32 right) => left.Equals(right);

    public static bool operator !=(BEUInt32 left, BEUInt32 right) => !(left == right);

    public override readonly string ToString() => ((uint)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((uint)this).ToString(format, formatProvider);

#if NET6_0_OR_GREATER
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((uint)this).TryFormat(destination, out charsWritten, format, provider);
#endif
}

public readonly struct LEInt32 : IEquatable<LEInt32>, IFormattable
#if NET6_0_OR_GREATER
, ISpanFormattable
#endif
{
    private readonly byte _byte0;
    private readonly byte _byte1;
    private readonly byte _byte2;
    private readonly byte _byte3;

    public LEInt32(int value)
    {
        if (BitConverter.IsLittleEndian)
        {
            Unsafe.As<byte, int>(ref _byte0) = value;
        }
        else
        {
            _byte0 = (byte)value;
            _byte1 = (byte)(value >> (8 * 1));
            _byte2 = (byte)(value >> (8 * 2));
            _byte3 = (byte)(value >> (8 * 3));
        }
    }

    public static implicit operator LEInt32(int value) => new(value);

    public static implicit operator int(LEInt32 value)
        => BitConverter.IsLittleEndian
        ? Unsafe.As<byte, int>(ref Unsafe.AsRef(in value._byte0))
        : (value._byte3 << (8 * 3))
        | (value._byte2 << (8 * 2))
        | (value._byte1 << (8 * 1))
        | value._byte0;

    public readonly bool Equals(LEInt32 other)
        => Unsafe.As<byte, int>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, int>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is LEInt32 value && Equals(value);

    public override readonly int GetHashCode() => this;

    public static bool operator ==(LEInt32 left, LEInt32 right) => left.Equals(right);

    public static bool operator !=(LEInt32 left, LEInt32 right) => !(left == right);

    public override readonly string ToString() => ((int)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((int)this).ToString(format, formatProvider);

#if NET6_0_OR_GREATER
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((int)this).TryFormat(destination, out charsWritten, format, provider);
#endif
}

public readonly struct LEUInt32 : IEquatable<LEUInt32>, IFormattable
#if NET6_0_OR_GREATER
, ISpanFormattable
#endif
{
    private readonly byte _byte0;
    private readonly byte _byte1;
    private readonly byte _byte2;
    private readonly byte _byte3;

    public LEUInt32(uint value)
    {
        if (BitConverter.IsLittleEndian)
        {
            Unsafe.As<byte, uint>(ref _byte0) = value;
        }
        else
        {
            _byte0 = (byte)value;
            _byte1 = (byte)(value >> (8 * 1));
            _byte2 = (byte)(value >> (8 * 2));
            _byte3 = (byte)(value >> (8 * 3));
        }
    }

    public static implicit operator LEUInt32(uint value) => new(value);

    public static implicit operator uint(LEUInt32 value)
        => BitConverter.IsLittleEndian
        ? Unsafe.As<byte, uint>(ref Unsafe.AsRef(in value._byte0))
        : ((uint)value._byte3 << (8 * 3))
        | ((uint)value._byte2 << (8 * 2))
        | ((uint)value._byte1 << (8 * 1))
        | value._byte0;

    public readonly bool Equals(LEUInt32 other)
        => Unsafe.As<byte, uint>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, uint>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is LEUInt32 value && Equals(value);

    public override readonly int GetHashCode() => ((uint)this).GetHashCode();

    public static bool operator ==(LEUInt32 left, LEUInt32 right) => left.Equals(right);

    public static bool operator !=(LEUInt32 left, LEUInt32 right) => !(left == right);

    public override readonly string ToString() => ((uint)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((uint)this).ToString(format, formatProvider);

#if NET6_0_OR_GREATER
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((uint)this).TryFormat(destination, out charsWritten, format, provider);
#endif
}

public readonly struct BEInt64 : IEquatable<BEInt64>, IFormattable
#if NET6_0_OR_GREATER
, ISpanFormattable
#endif
{
    private readonly byte _byte0;
    private readonly byte _byte1;
    private readonly byte _byte2;
    private readonly byte _byte3;
    private readonly byte _byte4;
    private readonly byte _byte5;
    private readonly byte _byte6;
    private readonly byte _byte7;

    public BEInt64(long value)
    {
        if (BitConverter.IsLittleEndian)
        {
            _byte7 = (byte)value;
            _byte6 = (byte)(value >> (8 * 1));
            _byte5 = (byte)(value >> (8 * 2));
            _byte4 = (byte)(value >> (8 * 3));
            _byte3 = (byte)(value >> (8 * 4));
            _byte2 = (byte)(value >> (8 * 5));
            _byte1 = (byte)(value >> (8 * 6));
            _byte0 = (byte)(value >> (8 * 7));
        }
        else
        {
            Unsafe.As<byte, long>(ref _byte0) = value;
        }
    }

    public static implicit operator BEInt64(long value) => new(value);

    public static implicit operator long(BEInt64 value)
        => BitConverter.IsLittleEndian
        ? ((long)value._byte0 << (8 * 7))
        | ((long)value._byte1 << (8 * 6))
        | ((long)value._byte2 << (8 * 5))
        | ((long)value._byte3 << (8 * 4))
        | ((long)value._byte4 << (8 * 3))
        | ((long)value._byte5 << (8 * 2))
        | ((long)value._byte6 << (8 * 1))
        | value._byte7
        : Unsafe.As<byte, long>(ref Unsafe.AsRef(in value._byte0));

    public readonly bool Equals(BEInt64 other)
        => Unsafe.As<byte, long>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, long>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is BEInt64 value && Equals(value);

    public override readonly int GetHashCode() => ((long)this).GetHashCode();

    public static bool operator ==(BEInt64 left, BEInt64 right) => left.Equals(right);

    public static bool operator !=(BEInt64 left, BEInt64 right) => !(left == right);

    public override readonly string ToString() => ((long)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((long)this).ToString(format, formatProvider);

#if NET6_0_OR_GREATER
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((long)this).TryFormat(destination, out charsWritten, format, provider);
#endif
}

public readonly struct BEUInt64 : IEquatable<BEUInt64>, IFormattable
#if NET6_0_OR_GREATER
, ISpanFormattable
#endif
{
    private readonly byte _byte0;
    private readonly byte _byte1;
    private readonly byte _byte2;
    private readonly byte _byte3;
    private readonly byte _byte4;
    private readonly byte _byte5;
    private readonly byte _byte6;
    private readonly byte _byte7;

    public BEUInt64(ulong value)
    {
        if (BitConverter.IsLittleEndian)
        {
            _byte7 = (byte)value;
            _byte6 = (byte)(value >> (8 * 1));
            _byte5 = (byte)(value >> (8 * 2));
            _byte4 = (byte)(value >> (8 * 3));
            _byte3 = (byte)(value >> (8 * 4));
            _byte2 = (byte)(value >> (8 * 5));
            _byte1 = (byte)(value >> (8 * 6));
            _byte0 = (byte)(value >> (8 * 7));
        }
        else
        {
            Unsafe.As<byte, ulong>(ref _byte0) = value;
        }
    }

    public static implicit operator BEUInt64(ulong value) => new(value);

    public static implicit operator ulong(BEUInt64 value)
        => BitConverter.IsLittleEndian
        ? ((ulong)value._byte0 << (8 * 7))
        | ((ulong)value._byte1 << (8 * 6))
        | ((ulong)value._byte2 << (8 * 5))
        | ((ulong)value._byte3 << (8 * 4))
        | ((ulong)value._byte4 << (8 * 3))
        | ((ulong)value._byte5 << (8 * 2))
        | ((ulong)value._byte6 << (8 * 1))
        | value._byte7
        : Unsafe.As<byte, ulong>(ref Unsafe.AsRef(in value._byte0));

    public readonly bool Equals(BEUInt64 other)
        => Unsafe.As<byte, ulong>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, ulong>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is BEUInt64 value && Equals(value);

    public override readonly int GetHashCode() => ((ulong)this).GetHashCode();

    public static bool operator ==(BEUInt64 left, BEUInt64 right) => left.Equals(right);

    public static bool operator !=(BEUInt64 left, BEUInt64 right) => !(left == right);

    public override readonly string ToString() => ((ulong)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((ulong)this).ToString(format, formatProvider);

#if NET6_0_OR_GREATER
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((ulong)this).TryFormat(destination, out charsWritten, format, provider);
#endif
}

public readonly struct LEInt64 : IEquatable<LEInt64>, IFormattable
#if NET6_0_OR_GREATER
, ISpanFormattable
#endif
{
    private readonly byte _byte0;
    private readonly byte _byte1;
    private readonly byte _byte2;
    private readonly byte _byte3;
    private readonly byte _byte4;
    private readonly byte _byte5;
    private readonly byte _byte6;
    private readonly byte _byte7;

    public LEInt64(long value)
    {
        if (BitConverter.IsLittleEndian)
        {
            Unsafe.As<byte, long>(ref _byte0) = value;
        }
        else
        {
            _byte0 = (byte)value;
            _byte1 = (byte)(value >> (8 * 1));
            _byte2 = (byte)(value >> (8 * 2));
            _byte3 = (byte)(value >> (8 * 3));
            _byte4 = (byte)(value >> (8 * 4));
            _byte5 = (byte)(value >> (8 * 5));
            _byte6 = (byte)(value >> (8 * 6));
            _byte7 = (byte)(value >> (8 * 7));
        }
    }

    public static implicit operator LEInt64(long value) => new(value);

    public static implicit operator long(LEInt64 value)
        => BitConverter.IsLittleEndian
        ? Unsafe.As<byte, long>(ref Unsafe.AsRef(in value._byte0))
        : ((long)value._byte7 << (8 * 7))
        | ((long)value._byte6 << (8 * 6))
        | ((long)value._byte5 << (8 * 5))
        | ((long)value._byte4 << (8 * 4))
        | ((long)value._byte3 << (8 * 3))
        | ((long)value._byte2 << (8 * 2))
        | ((long)value._byte1 << (8 * 1)) |
        value._byte0;

    public readonly bool Equals(LEInt64 other)
        => Unsafe.As<byte, long>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, long>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is LEInt64 value && Equals(value);

    public override readonly int GetHashCode() => ((long)this).GetHashCode();

    public static bool operator ==(LEInt64 left, LEInt64 right) => left.Equals(right);

    public static bool operator !=(LEInt64 left, LEInt64 right) => !(left == right);

    public override readonly string ToString() => ((long)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((long)this).ToString(format, formatProvider);

#if NET6_0_OR_GREATER
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((long)this).TryFormat(destination, out charsWritten, format, provider);
#endif
}

public readonly struct LEUInt64 : IEquatable<LEUInt64>, IFormattable
#if NET6_0_OR_GREATER
, ISpanFormattable
#endif
{
    private readonly byte _byte0;
    private readonly byte _byte1;
    private readonly byte _byte2;
    private readonly byte _byte3;
    private readonly byte _byte4;
    private readonly byte _byte5;
    private readonly byte _byte6;
    private readonly byte _byte7;

    public LEUInt64(ulong value)
    {
        if (BitConverter.IsLittleEndian)
        {
            Unsafe.As<byte, ulong>(ref _byte0) = value;
        }
        else
        {
            _byte0 = (byte)value;
            _byte1 = (byte)(value >> (8 * 1));
            _byte2 = (byte)(value >> (8 * 2));
            _byte3 = (byte)(value >> (8 * 3));
            _byte4 = (byte)(value >> (8 * 4));
            _byte5 = (byte)(value >> (8 * 5));
            _byte6 = (byte)(value >> (8 * 6));
            _byte7 = (byte)(value >> (8 * 7));
        }
    }

    public static implicit operator LEUInt64(ulong value) => new(value);

    public static implicit operator ulong(LEUInt64 value)
        => BitConverter.IsLittleEndian
        ? Unsafe.As<byte, ulong>(ref Unsafe.AsRef(in value._byte0))
        : ((ulong)value._byte7 << (8 * 7))
        | ((ulong)value._byte6 << (8 * 6))
        | ((ulong)value._byte5 << (8 * 5))
        | ((ulong)value._byte4 << (8 * 4))
        | ((ulong)value._byte3 << (8 * 3))
        | ((ulong)value._byte2 << (8 * 2))
        | ((ulong)value._byte1 << (8 * 1))
        | value._byte0;

    public readonly bool Equals(LEUInt64 other)
        => Unsafe.As<byte, ulong>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, ulong>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is LEUInt64 value && Equals(value);

    public override readonly int GetHashCode() => ((ulong)this).GetHashCode();

    public static bool operator ==(LEUInt64 left, LEUInt64 right) => left.Equals(right);

    public static bool operator !=(LEUInt64 left, LEUInt64 right) => !(left == right);

    public override readonly string ToString() => ((ulong)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((ulong)this).ToString(format, formatProvider);

#if NET6_0_OR_GREATER
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((ulong)this).TryFormat(destination, out charsWritten, format, provider);
#endif
}

#if NET7_0_OR_GREATER

public readonly struct BEInt128 : IEquatable<BEInt128>, IFormattable, ISpanFormattable
{
    private readonly byte _byte0;
    private readonly byte _byte1;
    private readonly byte _byte2;
    private readonly byte _byte3;
    private readonly byte _byte4;
    private readonly byte _byte5;
    private readonly byte _byte6;
    private readonly byte _byte7;
    private readonly byte _byte8;
    private readonly byte _byte9;
    private readonly byte _byte10;
    private readonly byte _byte11;
    private readonly byte _byte12;
    private readonly byte _byte13;
    private readonly byte _byte14;
    private readonly byte _byte15;

    public BEInt128(Int128 value)
    {
        if (BitConverter.IsLittleEndian)
        {
            _byte15 = (byte)value;
            _byte14 = (byte)(value >> (8 * 1));
            _byte13 = (byte)(value >> (8 * 2));
            _byte12 = (byte)(value >> (8 * 3));
            _byte11 = (byte)(value >> (8 * 4));
            _byte10 = (byte)(value >> (8 * 5));
            _byte9 = (byte)(value >> (8 * 6));
            _byte8 = (byte)(value >> (8 * 7));
            _byte7 = (byte)(value >> (8 * 8));
            _byte6 = (byte)(value >> (8 * 9));
            _byte5 = (byte)(value >> (8 * 10));
            _byte4 = (byte)(value >> (8 * 11));
            _byte3 = (byte)(value >> (8 * 12));
            _byte2 = (byte)(value >> (8 * 13));
            _byte1 = (byte)(value >> (8 * 14));
            _byte0 = (byte)(value >> (8 * 15));
        }
        else
        {
            Unsafe.As<byte, Int128>(ref _byte0) = value;
        }
    }

    public static implicit operator BEInt128(Int128 value) => new(value);

    public static implicit operator Int128(BEInt128 value)
        => BitConverter.IsLittleEndian
        ? ((Int128)value._byte0 << (8 * 15))
        | ((Int128)value._byte1 << (8 * 14))
        | ((Int128)value._byte2 << (8 * 13))
        | ((Int128)value._byte3 << (8 * 12))
        | ((Int128)value._byte4 << (8 * 11))
        | ((Int128)value._byte5 << (8 * 10))
        | ((Int128)value._byte6 << (8 * 9))
        | ((Int128)value._byte7 << (8 * 8))
        | ((Int128)value._byte8 << (8 * 7))
        | ((Int128)value._byte9 << (8 * 6))
        | ((Int128)value._byte10 << (8 * 5))
        | ((Int128)value._byte11 << (8 * 4))
        | ((Int128)value._byte12 << (8 * 3))
        | ((Int128)value._byte13 << (8 * 2))
        | ((Int128)value._byte14 << (8 * 1))
        | value._byte15
        : Unsafe.As<byte, Int128>(ref Unsafe.AsRef(in value._byte0));

    public readonly bool Equals(BEInt128 other)
        => Unsafe.As<byte, Int128>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, Int128>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is BEInt128 value && Equals(value);

    public override readonly int GetHashCode() => ((Int128)this).GetHashCode();

    public static bool operator ==(BEInt128 left, BEInt128 right) => left.Equals(right);

    public static bool operator !=(BEInt128 left, BEInt128 right) => !(left == right);

    public override readonly string ToString() => ((Int128)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((Int128)this).ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((Int128)this).TryFormat(destination, out charsWritten, format, provider);
}

public readonly struct BEUInt128 : IEquatable<BEUInt128>, IFormattable, ISpanFormattable
{
    private readonly byte _byte0;
    private readonly byte _byte1;
    private readonly byte _byte2;
    private readonly byte _byte3;
    private readonly byte _byte4;
    private readonly byte _byte5;
    private readonly byte _byte6;
    private readonly byte _byte7;
    private readonly byte _byte8;
    private readonly byte _byte9;
    private readonly byte _byte10;
    private readonly byte _byte11;
    private readonly byte _byte12;
    private readonly byte _byte13;
    private readonly byte _byte14;
    private readonly byte _byte15;

    public BEUInt128(UInt128 value)
    {
        if (BitConverter.IsLittleEndian)
        {
            _byte15 = (byte)value;
            _byte14 = (byte)(value >> (8 * 1));
            _byte13 = (byte)(value >> (8 * 2));
            _byte12 = (byte)(value >> (8 * 3));
            _byte11 = (byte)(value >> (8 * 4));
            _byte10 = (byte)(value >> (8 * 5));
            _byte9 = (byte)(value >> (8 * 6));
            _byte8 = (byte)(value >> (8 * 7));
            _byte7 = (byte)(value >> (8 * 8));
            _byte6 = (byte)(value >> (8 * 9));
            _byte5 = (byte)(value >> (8 * 10));
            _byte4 = (byte)(value >> (8 * 11));
            _byte3 = (byte)(value >> (8 * 12));
            _byte2 = (byte)(value >> (8 * 13));
            _byte1 = (byte)(value >> (8 * 14));
            _byte0 = (byte)(value >> (8 * 15));
        }
        else
        {
            Unsafe.As<byte, UInt128>(ref _byte0) = value;
        }
    }

    public static implicit operator BEUInt128(UInt128 value) => new(value);

    public static implicit operator UInt128(BEUInt128 value)
        => BitConverter.IsLittleEndian
        ? ((UInt128)value._byte0 << (8 * 15))
        | ((UInt128)value._byte1 << (8 * 14))
        | ((UInt128)value._byte2 << (8 * 13))
        | ((UInt128)value._byte3 << (8 * 12))
        | ((UInt128)value._byte4 << (8 * 11))
        | ((UInt128)value._byte5 << (8 * 10))
        | ((UInt128)value._byte6 << (8 * 9))
        | ((UInt128)value._byte7 << (8 * 8))
        | ((UInt128)value._byte8 << (8 * 7))
        | ((UInt128)value._byte9 << (8 * 6))
        | ((UInt128)value._byte10 << (8 * 5))
        | ((UInt128)value._byte11 << (8 * 4))
        | ((UInt128)value._byte12 << (8 * 3))
        | ((UInt128)value._byte13 << (8 * 2))
        | ((UInt128)value._byte14 << (8 * 1))
        | value._byte15
        : Unsafe.As<byte, UInt128>(ref Unsafe.AsRef(in value._byte0));

    public readonly bool Equals(BEUInt128 other)
        => Unsafe.As<byte, UInt128>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, UInt128>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is BEUInt128 value && Equals(value);

    public override readonly int GetHashCode() => ((UInt128)this).GetHashCode();

    public static bool operator ==(BEUInt128 left, BEUInt128 right) => left.Equals(right);

    public static bool operator !=(BEUInt128 left, BEUInt128 right) => !(left == right);

    public override readonly string ToString() => ((UInt128)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((UInt128)this).ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((UInt128)this).TryFormat(destination, out charsWritten, format, provider);
}

public readonly struct LEInt128 : IEquatable<LEInt128>, IFormattable, ISpanFormattable
{
    private readonly byte _byte0;
    private readonly byte _byte1;
    private readonly byte _byte2;
    private readonly byte _byte3;
    private readonly byte _byte4;
    private readonly byte _byte5;
    private readonly byte _byte6;
    private readonly byte _byte7;
    private readonly byte _byte8;
    private readonly byte _byte9;
    private readonly byte _byte10;
    private readonly byte _byte11;
    private readonly byte _byte12;
    private readonly byte _byte13;
    private readonly byte _byte14;
    private readonly byte _byte15;

    public LEInt128(Int128 value)
    {
        if (BitConverter.IsLittleEndian)
        {
            Unsafe.As<byte, Int128>(ref _byte0) = value;
        }
        else
        {
            _byte0 = (byte)value;
            _byte1 = (byte)(value >> (8 * 1));
            _byte2 = (byte)(value >> (8 * 2));
            _byte3 = (byte)(value >> (8 * 3));
            _byte4 = (byte)(value >> (8 * 4));
            _byte5 = (byte)(value >> (8 * 5));
            _byte6 = (byte)(value >> (8 * 6));
            _byte7 = (byte)(value >> (8 * 7));
            _byte8 = (byte)(value >> (8 * 8));
            _byte9 = (byte)(value >> (8 * 9));
            _byte10 = (byte)(value >> (8 * 10));
            _byte11 = (byte)(value >> (8 * 11));
            _byte12 = (byte)(value >> (8 * 12));
            _byte13 = (byte)(value >> (8 * 13));
            _byte14 = (byte)(value >> (8 * 14));
            _byte15 = (byte)(value >> (8 * 15));
        }
    }

    public static implicit operator LEInt128(Int128 value) => new(value);

    public static implicit operator Int128(LEInt128 value)
        => BitConverter.IsLittleEndian
        ? Unsafe.As<byte, Int128>(ref Unsafe.AsRef(in value._byte0))
        : ((Int128)value._byte15 << (8 * 15))
        | ((Int128)value._byte14 << (8 * 14))
        | ((Int128)value._byte13 << (8 * 13))
        | ((Int128)value._byte12 << (8 * 12))
        | ((Int128)value._byte11 << (8 * 11))
        | ((Int128)value._byte10 << (8 * 10))
        | ((Int128)value._byte9 << (8 * 9))
        | ((Int128)value._byte8 << (8 * 8))
        | ((Int128)value._byte7 << (8 * 7))
        | ((Int128)value._byte6 << (8 * 6))
        | ((Int128)value._byte5 << (8 * 5))
        | ((Int128)value._byte4 << (8 * 4))
        | ((Int128)value._byte3 << (8 * 3))
        | ((Int128)value._byte2 << (8 * 2))
        | ((Int128)value._byte1 << (8 * 1))
        | value._byte0;

    public readonly bool Equals(LEInt128 other)
        => Unsafe.As<byte, Int128>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, Int128>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is LEInt128 value && Equals(value);

    public override readonly int GetHashCode() => ((Int128)this).GetHashCode();

    public static bool operator ==(LEInt128 left, LEInt128 right) => left.Equals(right);

    public static bool operator !=(LEInt128 left, LEInt128 right) => !(left == right);

    public override readonly string ToString() => ((Int128)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((Int128)this).ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((Int128)this).TryFormat(destination, out charsWritten, format, provider);
}

public readonly struct LEUInt128 : IEquatable<LEUInt128>, IFormattable, ISpanFormattable
{
    private readonly byte _byte0;
    private readonly byte _byte1;
    private readonly byte _byte2;
    private readonly byte _byte3;
    private readonly byte _byte4;
    private readonly byte _byte5;
    private readonly byte _byte6;
    private readonly byte _byte7;
    private readonly byte _byte8;
    private readonly byte _byte9;
    private readonly byte _byte10;
    private readonly byte _byte11;
    private readonly byte _byte12;
    private readonly byte _byte13;
    private readonly byte _byte14;
    private readonly byte _byte15;

    public LEUInt128(UInt128 value)
    {
        if (BitConverter.IsLittleEndian)
        {
            Unsafe.As<byte, UInt128>(ref _byte0) = value;
        }
        else
        {
            _byte0 = (byte)value;
            _byte1 = (byte)(value >> (8 * 1));
            _byte2 = (byte)(value >> (8 * 2));
            _byte3 = (byte)(value >> (8 * 3));
            _byte4 = (byte)(value >> (8 * 4));
            _byte5 = (byte)(value >> (8 * 5));
            _byte6 = (byte)(value >> (8 * 6));
            _byte7 = (byte)(value >> (8 * 7));
            _byte8 = (byte)(value >> (8 * 8));
            _byte9 = (byte)(value >> (8 * 9));
            _byte10 = (byte)(value >> (8 * 10));
            _byte11 = (byte)(value >> (8 * 11));
            _byte12 = (byte)(value >> (8 * 12));
            _byte13 = (byte)(value >> (8 * 13));
            _byte14 = (byte)(value >> (8 * 14));
            _byte15 = (byte)(value >> (8 * 15));
        }
    }

    public static implicit operator LEUInt128(UInt128 value) => new(value);

    public static implicit operator UInt128(LEUInt128 value)
        => BitConverter.IsLittleEndian
        ? Unsafe.As<byte, UInt128>(ref Unsafe.AsRef(in value._byte0))
        : ((UInt128)value._byte15 << (8 * 15))
        | ((UInt128)value._byte14 << (8 * 14))
        | ((UInt128)value._byte13 << (8 * 13))
        | ((UInt128)value._byte12 << (8 * 12))
        | ((UInt128)value._byte11 << (8 * 11))
        | ((UInt128)value._byte10 << (8 * 10))
        | ((UInt128)value._byte9 << (8 * 9))
        | ((UInt128)value._byte8 << (8 * 8))
        | ((UInt128)value._byte7 << (8 * 7))
        | ((UInt128)value._byte6 << (8 * 6))
        | ((UInt128)value._byte5 << (8 * 5))
        | ((UInt128)value._byte4 << (8 * 4))
        | ((UInt128)value._byte3 << (8 * 3))
        | ((UInt128)value._byte2 << (8 * 2))
        | ((UInt128)value._byte1 << (8 * 1))
        | value._byte0;

    public readonly bool Equals(LEUInt128 other)
        => Unsafe.As<byte, UInt128>(ref Unsafe.AsRef(in _byte0))
        == Unsafe.As<byte, UInt128>(ref Unsafe.AsRef(in other._byte0));

    public override readonly bool Equals(object? obj) => obj is LEUInt128 value && Equals(value);

    public override readonly int GetHashCode() => ((UInt128)this).GetHashCode();

    public static bool operator ==(LEUInt128 left, LEUInt128 right) => left.Equals(right);

    public static bool operator !=(LEUInt128 left, LEUInt128 right) => !(left == right);

    public override readonly string ToString() => ((UInt128)this).ToString();

    public readonly string ToString(string? format, IFormatProvider? formatProvider) => ((UInt128)this).ToString(format, formatProvider);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
        => ((UInt128)this).TryFormat(destination, out charsWritten, format, provider);
}

#endif

#endif
