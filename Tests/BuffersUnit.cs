using LTRData.Extensions.Split;
using System.Diagnostics;
using Xunit;

namespace LTRData.Extensions.Tests;

public class BuffersUnit
{
#if NET6_0_OR_GREATER
    [Fact]
    public void TestSpanSplit1()
    {
        var str = "  123 ; 456 ,789; 012  ";

        var i = 0;

        foreach (var result in str.AsSpan().TokenEnum(',', StringSplitOptions.TrimEntries))
        {
            Trace.WriteLine($"'{result}'");

            foreach (var inner in result.TokenEnum(';', StringSplitOptions.TrimEntries))
            {
                i++;

                Trace.WriteLine($"'{inner}'");

                Assert.Equal(3, inner.Length);
            }
        }

        Assert.Equal(4, i);
    }

    [Fact]
    public void TestSpanSplitAny1()
    {
        var str = "  123 ; 456 ,789; 012  ";

        var i = 0;

        foreach (var result in str.AsSpan().TokenEnum(',', ';', StringSplitOptions.TrimEntries))
        {
            Trace.WriteLine($"'{result}'");

            i++;
        }

        Assert.Equal(4, i);
    }

    [Fact]
    public void TestSpanSplit2()
    {
        var str = "  123 xx 456 yyyy789xx 012  ";

        var i = 0;

        foreach (var result in str.AsSpan().TokenEnum("yyyy", StringSplitOptions.TrimEntries))
        {
            Trace.WriteLine($"'{result}'");

            foreach (var inner in result.TokenEnum("xx", StringSplitOptions.TrimEntries))
            {
                i++;

                Trace.WriteLine($"'{inner}'");

                Assert.Equal(3, inner.Length);
            }
        }

        Assert.Equal(4, i);
    }

    [Fact]
    public void TestSpanReversSplit1()
    {
        var str = "  123 ; 456 ,789; 012  ";

        var i = 0;

        foreach (var outer in str.AsSpan().TokenEnumReverse(',', StringSplitOptions.TrimEntries))
        {
            Trace.WriteLine($"'{outer}'");

            foreach (var inner in outer.TokenEnumReverse(';', StringSplitOptions.TrimEntries))
            {
                i++;

                Trace.WriteLine($"'{inner}'");

                Assert.Equal(3, inner.Length);
            }
        }

        Assert.Equal(4, i);

        var result = str.AsSpan().TokenEnumReverse(',', StringSplitOptions.TrimEntries).Last().TokenEnumReverse(';', StringSplitOptions.TrimEntries).First();

        Assert.Equal("456", result.ToString());
    }

    [Fact]
    public void TestSpanReverseSplit2()
    {
        var str = "  123 xx 456 yyyy789xx 012  ";

        var i = 0;

        foreach (var outer in str.AsSpan().TokenEnumReverse("yyyy", StringSplitOptions.TrimEntries))
        {
            Trace.WriteLine($"'{outer}'");

            foreach (var inner in outer.TokenEnumReverse("xx", StringSplitOptions.TrimEntries))
            {
                i++;

                Trace.WriteLine($"'{inner}'");

                Assert.Equal(3, inner.Length);
            }
        }

        Assert.Equal(4, i);

        var result = str.AsSpan().TokenEnum("yyyy", StringSplitOptions.TrimEntries).Last().TokenEnum("xx", StringSplitOptions.TrimEntries).First();

        Assert.Equal("789", result.ToString());
    }

    [Fact]
    public void Test1()
    {
        var str = "123;456 789;012";

        var result = str.AsMemory().TokenEnum(' ', StringSplitOptions.TrimEntries).Last().TokenEnum(';', StringSplitOptions.TrimEntries).First();

        Assert.Equal("789", result.ToString());
    }

    [Fact]
    public void TestReverse1()
    {
        var str = "123;456 789;012";

        var result = str.AsMemory().TokenEnumReverse(' ', StringSplitOptions.TrimEntries).Last().TokenEnumReverse(';', StringSplitOptions.TrimEntries).First();

        Assert.Equal("456", result.ToString());
    }

    [Fact]
    public void TestStrSep1()
    {
        var str = "123xxx456---789xxx012";

        var result = str.AsMemory().TokenEnum("---".AsMemory(), StringSplitOptions.TrimEntries).Last().TokenEnum("xxx".AsMemory(), StringSplitOptions.TrimEntries).First();

        Assert.Equal("789", result.ToString());
    }

    [Fact]
    public void TestReverseStrSep1()
    {
        var str = "123xxx456---789xxx012";

        var result = str.AsMemory().TokenEnumReverse("---".AsMemory(), StringSplitOptions.TrimEntries).First().TokenEnumReverse("xxx".AsMemory(), StringSplitOptions.TrimEntries).Last();

        Assert.Equal("789", result.ToString());
    }
#endif
}
