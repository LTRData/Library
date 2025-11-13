using LTRData.Extensions.Buffers;
using System.Text;
using Xunit;

namespace LTRData.Extensions.Tests;

public class StringBuffers
{
    [Fact]
    public void ReadNullTerminatedUnicodeString()
    {
        ReadOnlySpan<byte> bytes = [65, 0, 66, 0, 67, 0, 0, 0, 0, 0];
        var str = bytes.ReadNullTerminatedUnicodeString();
        Assert.Equal(3, str.Length);
        Assert.Equal("ABC", str);
    }

    [Fact]
    public void ReadNullTerminatedUtf8String()
    {
        ReadOnlySpan<byte> bytes = [65, 66, 67, 0, 0, 0, 0, 0];
        var str = bytes.ReadNullTerminatedMultiByteString(Encoding.UTF8);
        Assert.Equal(3, str.Length);
        Assert.Equal("ABC", str);
    }
}
