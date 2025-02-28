using LTRData.Extensions.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}
