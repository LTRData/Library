using System;
using System.Collections.Generic;
using System.Globalization;

namespace LTRData.MathExpression;

#pragma warning disable CS1591

public sealed class MathParser
{
    public static MathParser Default { get; } = new();

    public MathParseResult Parse(string sourceText)
    {
        if (sourceText is null)
        {
            throw new ArgumentNullException(nameof(sourceText));
        }

        var diagnostics = new List<MathDiagnostic>();
        var lexer = new MathLexer(sourceText, diagnostics);
        var tokens = lexer.Lex();

        if (diagnostics.Count != 0)
        {
            return new MathParseResult(sourceText, null, diagnostics);
        }

        var parser = new SyntaxParser(tokens, diagnostics);
        var root = parser.ParseRoot();

        return new MathParseResult(sourceText, diagnostics.Count == 0 ? root : null,
            diagnostics);
    }

    private sealed class SyntaxParser
    {
        private readonly List<MathToken> tokens;
        private readonly List<MathDiagnostic> diagnostics;
        private int position;

        public SyntaxParser(List<MathToken> tokens, List<MathDiagnostic> diagnostics)
        {
            this.tokens = tokens;
            this.diagnostics = diagnostics;
        }

        private MathToken Current => tokens[position];

        private MathToken NextToken()
        {
            var current = Current;

            if (current.Kind != MathTokenKind.End)
            {
                position++;
            }

            return current;
        }

        public MathSyntax? ParseRoot()
        {
            try
            {
                var expression = ParseAdditive();

                if (Current.Kind != MathTokenKind.End)
                {
                    Report("MATH203", $"Unexpected token '{Current.Text}'.", Current.Span);
                    return null;
                }

                return expression;
            }
            catch (ParseAbortException)
            {
                return null;
            }
        }

        private MathSyntax ParseAdditive()
        {
            var left = ParseMultiplicative();

            while (Current.Kind == MathTokenKind.Plus ||
                Current.Kind == MathTokenKind.Minus)
            {
                var operatorToken = NextToken();
                var right = ParseMultiplicative();
                var @operator = operatorToken.Kind == MathTokenKind.Plus
                    ? MathBinaryOperator.Add
                    : MathBinaryOperator.Subtract;

                left = new BinarySyntax(left, @operator, operatorToken.Span, right,
                    false, SourceSpan.FromBounds(left.Span.Start, right.Span.End));
            }

            return left;
        }

        private MathSyntax ParseMultiplicative()
        {
            var left = ParseUnary();

            for (; ; )
            {
                MathBinaryOperator @operator;
                MathToken operatorToken;
                var isImplicit = false;

                switch (Current.Kind)
                {
                    case MathTokenKind.Star:
                        operatorToken = NextToken();
                        @operator = MathBinaryOperator.Multiply;
                        break;

                    case MathTokenKind.Slash:
                        operatorToken = NextToken();
                        @operator = MathBinaryOperator.Divide;
                        break;

                    case MathTokenKind.Percent:
                        operatorToken = NextToken();
                        @operator = MathBinaryOperator.Remainder;
                        break;

                    case MathTokenKind.Identifier when
                        string.Equals(Current.Text, "mod", StringComparison.OrdinalIgnoreCase):
                        operatorToken = NextToken();
                        @operator = MathBinaryOperator.Remainder;
                        break;

                    case MathTokenKind.Identifier:
                    case MathTokenKind.OpenParenthesis:
                        operatorToken = new MathToken(MathTokenKind.ImplicitMultiply,
                            string.Empty,
                            SourceSpan.FromBounds(left.Span.End, Current.Span.Start), 0d);
                        @operator = MathBinaryOperator.Multiply;
                        isImplicit = true;
                        break;

                    default:
                        return left;
                }

                var right = ParseUnary();
                left = new BinarySyntax(left, @operator, operatorToken.Span, right,
                    isImplicit, SourceSpan.FromBounds(left.Span.Start, right.Span.End));
            }
        }

        private MathSyntax ParseUnary()
        {
            if (Current.Kind == MathTokenKind.Plus ||
                Current.Kind == MathTokenKind.Minus)
            {
                var operatorToken = NextToken();
                var operand = ParseUnary();
                var @operator = operatorToken.Kind == MathTokenKind.Plus
                    ? MathPrefixOperator.Identity
                    : MathPrefixOperator.Negate;

                return new PrefixSyntax(@operator, operatorToken.Span, operand,
                    SourceSpan.FromBounds(operatorToken.Span.Start, operand.Span.End));
            }

            return ParsePower();
        }

        private MathSyntax ParsePower()
        {
            var left = ParsePostfix();

            if (Current.Kind == MathTokenKind.Caret ||
                Current.Kind == MathTokenKind.DoubleStar)
            {
                var operatorToken = NextToken();
                var right = ParseUnary();

                return new BinarySyntax(left, MathBinaryOperator.Power,
                    operatorToken.Span, right, false,
                    SourceSpan.FromBounds(left.Span.Start, right.Span.End));
            }

            return left;
        }

        private MathSyntax ParsePostfix()
        {
            var expression = ParsePrimary();

            while (Current.Kind == MathTokenKind.Bang)
            {
                var operatorToken = NextToken();
                expression = new PostfixSyntax(expression,
                    MathPostfixOperator.Factorial, operatorToken.Span,
                    SourceSpan.FromBounds(expression.Span.Start, operatorToken.Span.End));
            }

            return expression;
        }

        private MathSyntax ParsePrimary()
        {
            if (Current.Kind == MathTokenKind.Number)
            {
                var number = NextToken();
                return new NumberSyntax(number.Number, number.Text, number.Span);
            }

            if (Current.Kind == MathTokenKind.Identifier)
            {
                var name = NextToken();

                if (Current.Kind != MathTokenKind.OpenParenthesis)
                {
                    return new NameSyntax(name.Text, name.Span);
                }

                NextToken();
                var arguments = new List<MathSyntax>();

                if (Current.Kind != MathTokenKind.CloseParenthesis)
                {
                    for (; ; )
                    {
                        arguments.Add(ParseAdditive());

                        if (Current.Kind != MathTokenKind.Comma)
                        {
                            break;
                        }

                        NextToken();
                    }
                }

                var close = Expect(MathTokenKind.CloseParenthesis,
                    "MATH202", "Expected ',' or ')' after function argument.");

                return new CallSyntax(name.Text, name.Span, arguments,
                    SourceSpan.FromBounds(name.Span.Start, close.Span.End));
            }

            if (Current.Kind == MathTokenKind.OpenParenthesis)
            {
                var open = NextToken();
                var expression = ParseAdditive();
                var close = Expect(MathTokenKind.CloseParenthesis,
                    "MATH201", "Expected ')' to close parenthesized expression.");

                return new ParenthesizedSyntax(expression,
                    SourceSpan.FromBounds(open.Span.Start, close.Span.End));
            }

            Report("MATH200", "Expected an expression.", Current.Span);
            throw new ParseAbortException();
        }

        private MathToken Expect(MathTokenKind kind, string code, string message)
        {
            if (Current.Kind == kind)
            {
                return NextToken();
            }

            Report(code, message, Current.Span);
            throw new ParseAbortException();
        }

        private void Report(string code, string message, SourceSpan span)
        {
            diagnostics.Add(new MathDiagnostic(code, message, span));
        }
    }

    private sealed class ParseAbortException : Exception
    {
    }

    private sealed class MathLexer
    {
        private readonly string sourceText;
        private readonly List<MathDiagnostic> diagnostics;
        private int position;

        public MathLexer(string sourceText, List<MathDiagnostic> diagnostics)
        {
            this.sourceText = sourceText;
            this.diagnostics = diagnostics;
        }

        public List<MathToken> Lex()
        {
            var tokens = new List<MathToken>();

            while (position < sourceText.Length)
            {
                if (char.IsWhiteSpace(sourceText[position]))
                {
                    position++;
                    continue;
                }

                var start = position;
                var current = sourceText[position];

                if (char.IsDigit(current) ||
                    (current == '.' && position + 1 < sourceText.Length &&
                     char.IsDigit(sourceText[position + 1])))
                {
                    tokens.Add(LexNumber());
                    continue;
                }

                if (char.IsLetter(current) || current == '_')
                {
                    position++;

                    while (position < sourceText.Length &&
                        (char.IsLetterOrDigit(sourceText[position]) ||
                         sourceText[position] == '_'))
                    {
                        position++;
                    }

                    tokens.Add(CreateToken(MathTokenKind.Identifier, start, position));
                    continue;
                }

                position++;

                switch (current)
                {
                    case '+':
                        tokens.Add(CreateToken(MathTokenKind.Plus, start, position));
                        break;
                    case '-':
                        tokens.Add(CreateToken(MathTokenKind.Minus, start, position));
                        break;
                    case '*':
                        if (position < sourceText.Length && sourceText[position] == '*')
                        {
                            position++;
                            tokens.Add(CreateToken(MathTokenKind.DoubleStar, start, position));
                        }
                        else
                        {
                            tokens.Add(CreateToken(MathTokenKind.Star, start, position));
                        }
                        break;
                    case '/':
                        tokens.Add(CreateToken(MathTokenKind.Slash, start, position));
                        break;
                    case '%':
                        tokens.Add(CreateToken(MathTokenKind.Percent, start, position));
                        break;
                    case '^':
                        tokens.Add(CreateToken(MathTokenKind.Caret, start, position));
                        break;
                    case '!':
                        tokens.Add(CreateToken(MathTokenKind.Bang, start, position));
                        break;
                    case '(':
                        tokens.Add(CreateToken(MathTokenKind.OpenParenthesis, start, position));
                        break;
                    case ')':
                        tokens.Add(CreateToken(MathTokenKind.CloseParenthesis, start, position));
                        break;
                    case ',':
                        tokens.Add(CreateToken(MathTokenKind.Comma, start, position));
                        break;
                    default:
                        diagnostics.Add(new MathDiagnostic("MATH100",
                            $"Invalid character '{current}'.",
                            new SourceSpan(start, 1)));
                        tokens.Add(CreateToken(MathTokenKind.Invalid, start, position));
                        break;
                }
            }

            tokens.Add(new MathToken(MathTokenKind.End, string.Empty,
                new SourceSpan(sourceText.Length, 0), 0d));

            return tokens;
        }

        private MathToken LexNumber()
        {
            var start = position;

            while (position < sourceText.Length && char.IsDigit(sourceText[position]))
            {
                position++;
            }

            if (position < sourceText.Length && sourceText[position] == '.')
            {
                position++;

                while (position < sourceText.Length && char.IsDigit(sourceText[position]))
                {
                    position++;
                }
            }

            if (position < sourceText.Length &&
                (sourceText[position] == 'e' || sourceText[position] == 'E'))
            {
                position++;

                if (position < sourceText.Length &&
                    (sourceText[position] == '+' || sourceText[position] == '-'))
                {
                    position++;
                }

                var exponentStart = position;

                while (position < sourceText.Length && char.IsDigit(sourceText[position]))
                {
                    position++;
                }

                if (position == exponentStart)
                {
                    var invalidText = sourceText.Substring(start, position - start);
                    var invalidSpan = new SourceSpan(start, position - start);
                    diagnostics.Add(new MathDiagnostic("MATH101",
                        $"Invalid numeric literal '{invalidText}'.", invalidSpan));
                    return new MathToken(MathTokenKind.Invalid, invalidText,
                        invalidSpan, 0d);
                }
            }

            var text = sourceText.Substring(start, position - start);

            if (!double.TryParse(text, NumberStyles.Float,
                CultureInfo.InvariantCulture, out var number))
            {
                var invalidSpan = new SourceSpan(start, position - start);
                diagnostics.Add(new MathDiagnostic("MATH101",
                    $"Invalid numeric literal '{text}'.", invalidSpan));
                return new MathToken(MathTokenKind.Invalid, text, invalidSpan, 0d);
            }

            return new MathToken(MathTokenKind.Number, text,
                new SourceSpan(start, position - start), number);
        }

        private MathToken CreateToken(MathTokenKind kind, int start, int end) =>
            new(kind, sourceText.Substring(start, end - start),
                new SourceSpan(start, end - start), 0d);
    }

    private enum MathTokenKind
    {
        Invalid,
        End,
        Number,
        Identifier,
        Plus,
        Minus,
        Star,
        Slash,
        Percent,
        Caret,
        DoubleStar,
        Bang,
        OpenParenthesis,
        CloseParenthesis,
        Comma,
        ImplicitMultiply
    }

    private struct MathToken
    {
        public MathToken(MathTokenKind kind, string text, SourceSpan span, double number)
        {
            Kind = kind;
            Text = text;
            Span = span;
            Number = number;
        }

        public MathTokenKind Kind { get; }

        public string Text { get; }

        public SourceSpan Span { get; }

        public double Number { get; }
    }
}
