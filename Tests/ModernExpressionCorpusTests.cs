using System;
using System.Collections.Generic;
using LTRData.MathExpression;
using Xunit;

namespace LTRData.Extensions.Tests;

/// <summary>The applicable expressions from the legacy tests, through the modern pipeline.</summary>
public class ModernExpressionCorpusTests
{
    public static IEnumerable<object[]> Expressions => new[]
    {
        new object[] { "sin(0.4) * 2", Math.Sin(.4) * 2 },
        new object[] { "atan2(312,2)", Math.Atan2(312, 2) },
        new object[] { "e ** 2", Math.Pow(Math.E, 2) },
        new object[] { "169 - 5 - 3 - 1", 160d },
        new object[] { "169 - (5 - 3 - 1)", 168d },
        new object[] { "169 (5 - 3 - 1)", 169d },
        new object[] { "169 - 5 * 3 - 1", 153d },
        new object[] { "169 - 5 - 3 * 1", 161d },
        new object[] { "169 * 5 - 3 - 1", 841d },
        new object[] { "2.5 + 400 / (.1 - .01) * 2", 2.5 + 400 / (.1 - .01) * 2 },
        new object[] { "-35-(-35)", 0d },
        new object[] { "-(35-(-35))", -70d },
        new object[] { "-(35)-(-35)", 0d },
        new object[] { "--35", 35d },
        new object[] { "+35", 35d },
        new object[] { "+-35", -35d },
        new object[] { "-+35", -35d },
        new object[] { "+-+35", -35d }
    };

    [Theory]
    [MemberData(nameof(Expressions))]
    public void ExistingMathematicalExpressionsKeepTheirExpectedValues(string source, double expected)
    {
        var parsed = MathParser.Default.Parse(source);
        Assert.True(parsed.Success);
        var binding = MathBinder.Bind(parsed.Root!, MathSymbolCatalog.Standard);
        Assert.True(binding.Success);
        Assert.Equal(expected, binding.Expression!.Evaluate(), 12);
    }
}
