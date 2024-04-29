using LTRData.Extensions.Formatting;
using Xunit;

namespace LTRData.Extensions.Tests;

public class Formatting
{
    [Fact]
    public void FormatHexNumbers()
    {
        byte[] bytes = [0x0a, 0xff, 0xeb];

        Assert.Equal("0affeb", bytes.ToHexString());
    }

    [Fact]
    public void ParseHexString()
    {
        byte[] bytes = [0x0a, 0xff, 0xeb];
        var chars = "0affeb";

        var buffer = HexExtensions.ParseHexString(chars);
        Assert.Equal(bytes, buffer);

        chars = "xyz123";
        Assert.Throws<FormatException>(() => HexExtensions.ParseHexString(chars));
    }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
    [Fact]
    public void FormatHexNumbersNetCore()
    {
        byte[] bytes = [0x0a, 0xff, 0xeb];

        Span<char> buffer = stackalloc char[8];
        Assert.True(bytes.TryFormatHexString(":", buffer, upperCase: true));
        Assert.Equal("0A:FF:EB", buffer);
        Assert.True(bytes.TryFormatHexString(":", buffer, upperCase: false));
        Assert.Equal("0a:ff:eb", buffer);

        buffer = stackalloc char[6];
        Assert.True(bytes.TryFormatHexString("", buffer, upperCase: true));
        Assert.Equal("0AFFEB", buffer);
        Assert.True(bytes.TryFormatHexString("", buffer, upperCase: false));
        Assert.Equal("0affeb", buffer);

        buffer = stackalloc char[2];
        Assert.False(bytes.TryFormatHexString(":", buffer, upperCase: true));
        Assert.False(bytes.TryFormatHexString(":", buffer, upperCase: false));
        Assert.False(bytes.TryFormatHexString("", buffer, upperCase: true));
        Assert.False(bytes.TryFormatHexString("", buffer, upperCase: false));
    }

    [Fact]
    public void ParsetHexStringNetCore()
    {
        byte[] bytes = [0x0a, 0xff, 0xeb];
        var chars = "0affeb";

        Span<byte> buffer = stackalloc byte[3];
        Assert.True(HexExtensions.TryParseHexString(chars, buffer));
        Assert.Equal(bytes, buffer);

        chars = "xyz123";
        Assert.False(HexExtensions.TryParseHexString(chars, buffer));

        buffer = stackalloc byte[2];
        Assert.False(HexExtensions.TryParseHexString(chars, buffer));

        chars = "xyz123";
        Assert.False(HexExtensions.TryParseHexString(chars, buffer));
    }
#endif
}
