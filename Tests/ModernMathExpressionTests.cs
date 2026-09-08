using LTRData.MathExpression;
using System;
using System.Collections.Generic;
using Xunit;

namespace LTRData.Extensions.Tests;

[Trait("Contract", "Modern")]
public class ModernMathExpressionTests
{
    [Fact]
    public void ParserPreservesSourceTextAndNodeSpan()
    {
        const string source = " SIN(x) ";

        var parse = MathParser.Default.Parse(source);

        Assert.True(parse.Success, FormatDiagnostics(parse.Diagnostics));
        Assert.Equal(source, parse.SourceText);
        Assert.Equal(new SourceSpan(1, 6), parse.Root!.Span);

        var call = Assert.IsType<CallSyntax>(parse.Root);
        Assert.Equal("SIN", call.Name);
        Assert.Equal(new SourceSpan(1, 3), call.NameSpan);
    }

    [Theory]
    [InlineData("2*3^2", 18d)]
    [InlineData("2^3^2", 512d)]
    [InlineData("-2^2", -4d)]
    [InlineData("2^-2", 0.25d)]
    [InlineData("20 mod 6", 2d)]
    [InlineData("2**3", 8d)]
    public void ModernOperatorRulesAreExplicit(string source, double expected)
    {
        Assert.Equal(expected, Bind(source).Evaluate());
    }

    [Fact]
    public void ScientificNotationWithSignedExponentsIsOneNumberToken()
    {
        var expression = Bind("1e-3 + 2.5E+2");

        Assert.Equal(250.001d, expression.Evaluate(), 12);
    }

    [Fact]
    public void ImplicitMultiplicationUsesOrdinaryMultiplicationPrecedence()
    {
        var expression = Bind("2x + 3(x+1)");
        var function = expression.BindUnary("x");

        Assert.Equal(13d, function.Evaluate(2d));
    }

    [Fact]
    public void VariablesUseCaseInsensitiveStableFirstOccurrenceSlots()
    {
        var expression = Bind("b + A*b + a");

        Assert.Equal(2, expression.Variables.Count);
        Assert.Equal("b", expression.Variables[0].Name);
        Assert.Equal(0, expression.Variables[0].Slot);
        Assert.Equal("A", expression.Variables[1].Name);
        Assert.Equal(1, expression.Variables[1].Slot);
        Assert.Equal(11d, expression.Evaluate(2d, 3d));
        Assert.Equal(11d, expression.Evaluate(new Dictionary<string, double>
        {
            ["B"] = 2d,
            ["a"] = 3d
        }));
    }

    [Fact]
    public void StandardSymbolsAreCaseInsensitiveAndExplicit()
    {
        var expression = Bind("SIN(PI/2) + log(e)");

        Assert.Equal(2d, expression.Evaluate(), 12);
    }

    [Fact]
    public void StandardCatalogCanBeExtendedWithoutReflection()
    {
        var symbols = MathSymbolCatalog.Standard.ToBuilder()
            .AddConstant("g", 9.80665d)
            .AddFunction("square", x => x * x)
            .Build();

        var expression = Bind("square(g)", symbols);

        Assert.Equal(9.80665d * 9.80665d, expression.Evaluate(), 12);
    }

    [Fact]
    public void UnknownFunctionIsABindingDiagnosticNotAVariable()
    {
        var parse = MathParser.Default.Parse("missing(2)");
        Assert.True(parse.Success, FormatDiagnostics(parse.Diagnostics));

        var binding = MathBinder.Bind(parse.Root!, MathSymbolCatalog.Standard);

        Assert.False(binding.Success);
        var diagnostic = Assert.Single(binding.Diagnostics);
        Assert.Equal("MATH300", diagnostic.Code);
        Assert.Equal(new SourceSpan(0, 7), diagnostic.Span);
    }

    [Theory]
    [InlineData(0d, 1d)]
    [InlineData(1d, 1d)]
    [InlineData(5d, 120d)]
    public void FactorialHasSpecifiedIntegerSemantics(double value, double expected)
    {
        var expression = Bind(value.ToString(System.Globalization.CultureInfo.InvariantCulture) + "!");

        Assert.Equal(expected, expression.Evaluate());
    }

    [Fact]
    public void InvalidFactorialInputProducesNotANumber()
    {
        Assert.True(double.IsNaN(Bind("(-1)!").Evaluate()));
        Assert.True(double.IsNaN(Bind("2.5!").Evaluate()));
        Assert.Equal(double.PositiveInfinity, Bind("171!").Evaluate());
    }

    [Fact]
    public void EmptyInputIsAParseDiagnostic()
    {
        var parse = MathParser.Default.Parse(string.Empty);

        Assert.False(parse.Success);
        var diagnostic = Assert.Single(parse.Diagnostics);
        Assert.Equal("MATH200", diagnostic.Code);
        Assert.Equal(new SourceSpan(0, 0), diagnostic.Span);
    }

    [Fact]
    public void BitwiseOperatorsAreNotPartOfTheModernLanguage()
    {
        var parse = MathParser.Default.Parse("1 << 10");

        Assert.False(parse.Success);
        Assert.Contains(parse.Diagnostics, diagnostic => diagnostic.Code == "MATH100");
    }

    [Fact]
    public void TextualOperatorSubstringsRemainPartOfIdentifiers()
    {
        var expression = Bind("model + power_level + negative");

        Assert.Equal(6d, expression.Evaluate(1d, 2d, 3d));
    }

    [Fact]
    public void UnaryBindingRejectsOtherVariables()
    {
        var expression = Bind("x+y");

        Assert.Throws<InvalidOperationException>(() => expression.BindUnary("x"));
    }

    [Fact]
    public void ConstantExpressionCanBeUsedAsAUnaryFunction()
    {
        var function = Bind("pi").BindUnary("x");

        Assert.Equal(Math.PI, function.Evaluate(123d));
    }

    private static BoundMathExpression Bind(string source,
        MathSymbolCatalog? symbols = null)
    {
        var parse = MathParser.Default.Parse(source);
        Assert.True(parse.Success, FormatDiagnostics(parse.Diagnostics));

        var binding = MathBinder.Bind(parse.Root!,
            symbols ?? MathSymbolCatalog.Standard);
        Assert.True(binding.Success, FormatDiagnostics(binding.Diagnostics));

        return binding.Expression!;
    }

    private static string FormatDiagnostics(IEnumerable<MathDiagnostic> diagnostics) =>
        string.Join(Environment.NewLine, diagnostics);
}
