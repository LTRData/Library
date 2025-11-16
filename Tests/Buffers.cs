using LTRData.Extensions.Native;
using LTRData.Extensions.Numeric;
using System.Runtime.CompilerServices;
using Xunit;

namespace LTRData.Extensions.Tests;

public class Buffers
{
    [Fact]
    public void ArrayWithData()
    {
        var buffer = new byte[] { 0, 0, 0, 1, 2, 3 };

        Assert.False(buffer.IsBufferZero());
    }

    [Fact]
    public void ArrayEmpty()
    {
        var buffer = Array.Empty<byte>();

        Assert.True(buffer.IsBufferZero());
    }

    [Fact]
    public void ArrayZeros()
    {
        var buffer = new byte[] { 0, 0, 0, 0, 0, 0 };

        Assert.True(buffer.IsBufferZero());
    }

    [Fact]
    public void SpanWithData()
    {
        ReadOnlySpan<byte> buffer = [0, 0, 0, 1, 2, 3];

        Assert.False(buffer.IsBufferZero());
    }

    [Fact]
    public void SpanEmpty()
    {
        ReadOnlySpan<byte> buffer = [];

        Assert.True(buffer.IsBufferZero());
    }

    [Fact]
    public void SpanZeros()
    {
        ReadOnlySpan<byte> buffer = [0, 0, 0, 0, 0, 0];

        Assert.True(buffer.IsBufferZero());
    }

    [Fact]
    public void BE_LE_Conversions()
    {
        Assert.Equal(500, (short)(BEInt16)500);
        Assert.Equal(500, (ushort)(BEUInt16)500);
        Assert.Equal(500_000, (int)(BEInt32)500_000);
        Assert.Equal(500_000u, (uint)(BEUInt32)500_000u);
        Assert.Equal(500_000_000, (long)(BEInt64)500_000_000);
        Assert.Equal(500_000_000u, (ulong)(BEUInt64)500_000_000u);
#if NET7_0_OR_GREATER
        Assert.Equal((((Int128)0x16) << 120) + 0x3c, (Int128)(BEInt128)(((Int128)0x16) << 120) + 0x3c);
        Assert.Equal((((UInt128)0x16) << 120) + 0x3c, (UInt128)(BEUInt128)(((UInt128)0x16) << 120) + 0x3c);
#endif

        Assert.Equal(500, (short)(LEInt16)500);
        Assert.Equal(500, (ushort)(LEUInt16)500);
        Assert.Equal(500_000, (int)(LEInt32)500_000);
        Assert.Equal(500_000u, (uint)(LEUInt32)500_000u);
        Assert.Equal(500_000_000, (long)(LEInt64)500_000_000);
        Assert.Equal(500_000_000u, (ulong)(LEUInt64)500_000_000u);
#if NET7_0_OR_GREATER
        Assert.Equal((((Int128)0x16) << 120) + 0x3c, (Int128)(LEInt128)(((Int128)0x16) << 120) + 0x3c);
        Assert.Equal((((UInt128)0x16) << 120) + 0x3c, (UInt128)(LEUInt128)(((UInt128)0x16) << 120) + 0x3c);
#endif
    }

    [Fact]
    public void BE_LE_Storage()
    {
        BEInt16 beint16 = 500;
        BEUInt16 beuint16 = 500;
        BEInt32 beint32 = 500_000_025;
        BEUInt32 beuint32 = 500_000_025u;
        BEInt64 beint64 = 500_000_000_000_000_025;
        BEUInt64 beuint64 = 500_000_000_000_000_025u;
#if NET7_0_OR_GREATER
        BEInt128 beint128 = (((Int128)0x16) << 120) + 0x3c;
        BEUInt128 beuint128 = (((UInt128)0x16) << 120) + 0x3c;
#endif

        Assert.Equal(0x01, Unsafe.As<BEInt16, byte>(ref beint16));
        Assert.Equal(0x01, Unsafe.As<BEUInt16, byte>(ref beuint16));
        Assert.Equal(0x1d, Unsafe.As<BEInt32, byte>(ref beint32));
        Assert.Equal(0x1d, Unsafe.As<BEUInt32, byte>(ref beuint32));
        Assert.Equal(0x06, Unsafe.As<BEInt64, byte>(ref beint64));
        Assert.Equal(0x06, Unsafe.As<BEUInt64, byte>(ref beuint64));
#if NET7_0_OR_GREATER
        Assert.Equal(0x16, Unsafe.As<BEInt128, byte>(ref beint128));
        Assert.Equal(0x16, Unsafe.As<BEUInt128, byte>(ref beuint128));
#endif

        LEInt16 leint16 = 500;
        LEUInt16 leuint16 = 500;
        LEInt32 leint32 = 500_000;
        LEUInt32 leuint32 = 500_000u;
        LEInt64 leint64 = 500_000_025;
        LEUInt64 leuint64 = 500_000_025u;
#if NET7_0_OR_GREATER
        LEInt128 leint128 = (((Int128)0x16) << 120) + 0x3c;
        LEUInt128 leuint128 = (((UInt128)0x16) << 120) + 0x3c;
#endif

        Assert.Equal(0xf4, Unsafe.As<LEInt16, byte>(ref leint16));
        Assert.Equal(0xf4, Unsafe.As<LEUInt16, byte>(ref leuint16));
        Assert.Equal(0x20, Unsafe.As<LEInt32, byte>(ref leint32));
        Assert.Equal(0x20, Unsafe.As<LEUInt32, byte>(ref leuint32));
        Assert.Equal(0x19, Unsafe.As<LEInt64, byte>(ref leint64));
        Assert.Equal(0x19, Unsafe.As<LEUInt64, byte>(ref leuint64));
#if NET7_0_OR_GREATER
        Assert.Equal(0x3c, Unsafe.As<LEInt128, byte>(ref leint128));
        Assert.Equal(0x3c, Unsafe.As<LEUInt128, byte>(ref leuint128));
#endif
    }
}
