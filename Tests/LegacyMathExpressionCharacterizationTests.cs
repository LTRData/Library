using LTRData.MathExpression;
using System;
using System.Globalization;
using Xunit;

namespace LTRData.Extensions.Tests;

/// <summary>
/// Documents selected behavior of the historical parser. These are evidence for migration,
/// not normative tests for <see cref="MathParser"/>.
/// </summary>
[Trait("Contract", "LegacyCharacterization")]
public class LegacyMathExpressionCharacterizationTests
{
    private readonly MathExpressionParser parser = new(CultureInfo.InvariantCulture);

    [Fact]
    public void PowerAndMultiplicationCurrentlyGroupLeftToRightAtOnePrecedence()
    {
        var expression = parser.ParseExpression<Func<double>>("2*3^2");

        Assert.Equal(36d, expression());
    }

    [Fact]
    public void RepeatedPowerIsCurrentlyLeftAssociative()
    {
        var expression = parser.ParseExpression<Func<double>>("2^3^2");

        Assert.Equal(64d, expression());
    }

    [Fact]
    public void EmptyInputCurrentlyBecomesZero()
    {
        var expression = parser.ParseExpression<Func<double>>(string.Empty);

        Assert.Equal(0d, expression());
    }

    [Fact]
    public void ScriptControlTreatsYAsASecondCallerSuppliedVariable()
    {
        var control = new ScriptControl(parser) { Expression = "x+y" };

        Assert.Equal((double?)5d, control.Eval(2d, 3d));
    }
}
