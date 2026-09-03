using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LTRData.MathExpression;

#pragma warning disable CS1591

public struct SourceSpan : IEquatable<SourceSpan>
{
    public SourceSpan(int start, int length)
    {
        if (start < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(start));
        }

        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        Start = start;
        Length = length;
    }

    public int Start { get; }

    public int Length { get; }

    public int End => Start + Length;

    public static SourceSpan FromBounds(int start, int end)
    {
        if (end < start)
        {
            throw new ArgumentOutOfRangeException(nameof(end));
        }

        return new SourceSpan(start, end - start);
    }

    public bool Equals(SourceSpan other) => Start == other.Start && Length == other.Length;

    public override bool Equals(object? obj) => obj is SourceSpan other && Equals(other);

    public override int GetHashCode() => unchecked((Start * 397) ^ Length);

    public override string ToString() => $"{Start}..{End}";

    public static bool operator ==(SourceSpan left, SourceSpan right) => left.Equals(right);

    public static bool operator !=(SourceSpan left, SourceSpan right) => !left.Equals(right);
}

public enum MathSyntaxKind
{
    Number,
    Name,
    Parenthesized,
    Prefix,
    Postfix,
    Binary,
    Call
}

public enum MathPrefixOperator
{
    Identity,
    Negate
}

public enum MathPostfixOperator
{
    Factorial
}

public enum MathBinaryOperator
{
    Add,
    Subtract,
    Multiply,
    Divide,
    Remainder,
    Power
}

public abstract class MathSyntax
{
    internal MathSyntax(SourceSpan span)
    {
        Span = span;
    }

    public SourceSpan Span { get; }

    public abstract MathSyntaxKind Kind { get; }
}

public sealed class NumberSyntax : MathSyntax
{
    public NumberSyntax(double value, string lexeme, SourceSpan span)
        : base(span)
    {
        Value = value;
        Lexeme = lexeme ?? throw new ArgumentNullException(nameof(lexeme));
    }

    public override MathSyntaxKind Kind => MathSyntaxKind.Number;

    public double Value { get; }

    public string Lexeme { get; }
}

public sealed class NameSyntax : MathSyntax
{
    public NameSyntax(string name, SourceSpan span)
        : base(span)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public override MathSyntaxKind Kind => MathSyntaxKind.Name;

    public string Name { get; }
}

public sealed class ParenthesizedSyntax : MathSyntax
{
    public ParenthesizedSyntax(MathSyntax expression, SourceSpan span)
        : base(span)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    public override MathSyntaxKind Kind => MathSyntaxKind.Parenthesized;

    public MathSyntax Expression { get; }
}

public sealed class PrefixSyntax : MathSyntax
{
    public PrefixSyntax(MathPrefixOperator @operator, SourceSpan operatorSpan,
        MathSyntax operand, SourceSpan span)
        : base(span)
    {
        Operator = @operator;
        OperatorSpan = operatorSpan;
        Operand = operand ?? throw new ArgumentNullException(nameof(operand));
    }

    public override MathSyntaxKind Kind => MathSyntaxKind.Prefix;

    public MathPrefixOperator Operator { get; }

    public SourceSpan OperatorSpan { get; }

    public MathSyntax Operand { get; }
}

public sealed class PostfixSyntax : MathSyntax
{
    public PostfixSyntax(MathSyntax operand, MathPostfixOperator @operator,
        SourceSpan operatorSpan, SourceSpan span)
        : base(span)
    {
        Operand = operand ?? throw new ArgumentNullException(nameof(operand));
        Operator = @operator;
        OperatorSpan = operatorSpan;
    }

    public override MathSyntaxKind Kind => MathSyntaxKind.Postfix;

    public MathSyntax Operand { get; }

    public MathPostfixOperator Operator { get; }

    public SourceSpan OperatorSpan { get; }
}

public sealed class BinarySyntax : MathSyntax
{
    public BinarySyntax(MathSyntax left, MathBinaryOperator @operator,
        SourceSpan operatorSpan, MathSyntax right, bool isImplicit, SourceSpan span)
        : base(span)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Operator = @operator;
        OperatorSpan = operatorSpan;
        Right = right ?? throw new ArgumentNullException(nameof(right));
        IsImplicit = isImplicit;
    }

    public override MathSyntaxKind Kind => MathSyntaxKind.Binary;

    public MathSyntax Left { get; }

    public MathBinaryOperator Operator { get; }

    public SourceSpan OperatorSpan { get; }

    public MathSyntax Right { get; }

    public bool IsImplicit { get; }
}

public sealed class CallSyntax : MathSyntax
{
    public CallSyntax(string name, SourceSpan nameSpan,
        IEnumerable<MathSyntax> arguments, SourceSpan span)
        : base(span)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        NameSpan = nameSpan;

        if (arguments is null)
        {
            throw new ArgumentNullException(nameof(arguments));
        }

        Arguments = new List<MathSyntax>(arguments).AsReadOnly();
    }

    public override MathSyntaxKind Kind => MathSyntaxKind.Call;

    public string Name { get; }

    public SourceSpan NameSpan { get; }

    public ReadOnlyCollection<MathSyntax> Arguments { get; }
}
