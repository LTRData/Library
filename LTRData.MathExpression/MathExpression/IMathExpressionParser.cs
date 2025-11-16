using System.Globalization;
using System.Linq.Expressions;

namespace LTRData.MathExpression;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public interface IMathExpressionParser
{
    CultureInfo FormatInfo { get; }

    Expression ParseExpression(string value, out ParameterExpression[] parameters);
}
