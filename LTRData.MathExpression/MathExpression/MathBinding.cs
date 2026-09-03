using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LTRData.MathExpression;

#pragma warning disable CS1591

public sealed class MathVariable
{
    internal MathVariable(string name, int slot)
    {
        Name = name;
        Slot = slot;
    }

    public string Name { get; }

    public int Slot { get; }
}

public sealed class MathBindingResult
{
    internal MathBindingResult(BoundMathExpression? expression,
        IEnumerable<MathDiagnostic> diagnostics)
    {
        Expression = expression;
        Diagnostics = new List<MathDiagnostic>(diagnostics).AsReadOnly();
    }

    public BoundMathExpression? Expression { get; }

    public ReadOnlyCollection<MathDiagnostic> Diagnostics { get; }

    public bool Success => Expression is not null && Diagnostics.Count == 0;
}

public static class MathBinder
{
    public static MathBindingResult Bind(MathSyntax syntax,
        MathSymbolCatalog symbolCatalog)
    {
        if (syntax is null)
        {
            throw new ArgumentNullException(nameof(syntax));
        }

        if (symbolCatalog is null)
        {
            throw new ArgumentNullException(nameof(symbolCatalog));
        }

        var binder = new Binder(symbolCatalog);
        var root = binder.Bind(syntax);

        if (binder.Diagnostics.Count != 0)
        {
            return new MathBindingResult(null, binder.Diagnostics);
        }

        return new MathBindingResult(
            new BoundMathExpression(root, binder.Variables), binder.Diagnostics);
    }

    private sealed class Binder
    {
        private readonly MathSymbolCatalog symbolCatalog;
        private readonly Dictionary<string, MathVariable> variablesByName =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly List<MathVariable> variables = new();
        private readonly List<MathDiagnostic> diagnostics = new();

        public Binder(MathSymbolCatalog symbolCatalog)
        {
            this.symbolCatalog = symbolCatalog;
        }

        public List<MathVariable> Variables => variables;

        public List<MathDiagnostic> Diagnostics => diagnostics;

        public BoundNode Bind(MathSyntax syntax)
        {
            if (syntax is NumberSyntax number)
            {
                return new BoundConstantNode(number.Value);
            }

            if (syntax is NameSyntax name)
            {
                if (symbolCatalog.TryGetConstant(name.Name, out var value))
                {
                    return new BoundConstantNode(value);
                }

                if (!variablesByName.TryGetValue(name.Name, out var variable))
                {
                    variable = new MathVariable(name.Name, variables.Count);
                    variablesByName.Add(name.Name, variable);
                    variables.Add(variable);
                }

                return new BoundVariableNode(variable.Slot);
            }

            if (syntax is ParenthesizedSyntax parenthesized)
            {
                return Bind(parenthesized.Expression);
            }

            if (syntax is PrefixSyntax prefix)
            {
                return new BoundPrefixNode(prefix.Operator, Bind(prefix.Operand));
            }

            if (syntax is PostfixSyntax postfix)
            {
                return new BoundPostfixNode(postfix.Operator, Bind(postfix.Operand));
            }

            if (syntax is BinarySyntax binary)
            {
                return new BoundBinaryNode(binary.Operator,
                    Bind(binary.Left), Bind(binary.Right));
            }

            if (syntax is CallSyntax call)
            {
                var arguments = new BoundNode[call.Arguments.Count];

                for (var index = 0; index < arguments.Length; index++)
                {
                    arguments[index] = Bind(call.Arguments[index]);
                }

                if (!symbolCatalog.TryGetFunction(call.Name, arguments.Length,
                    out var function) || function is null)
                {
                    diagnostics.Add(new MathDiagnostic("MATH300",
                        $"Unknown function or unsupported arity: '{call.Name}' with " +
                        $"{arguments.Length} argument(s).", call.NameSpan));
                    return new BoundConstantNode(double.NaN);
                }

                return new BoundCallNode(function, arguments);
            }

            throw new NotSupportedException(
                $"Unsupported syntax node '{syntax.GetType().FullName}'.");
        }
    }
}

public sealed class BoundMathExpression
{
    private readonly BoundNode root;

    internal BoundMathExpression(BoundNode root, IEnumerable<MathVariable> variables)
    {
        this.root = root;
        Variables = new List<MathVariable>(variables).AsReadOnly();
    }

    public ReadOnlyCollection<MathVariable> Variables { get; }

    public double Evaluate(params double[] values)
    {
        if (values is null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        if (values.Length != Variables.Count)
        {
            throw new ArgumentException(
                $"Expected {Variables.Count} variable value(s), but received {values.Length}.",
                nameof(values));
        }

        return root.Evaluate(MathEvaluationContext.ForValues(values));
    }

    public double Evaluate(IDictionary<string, double> values)
    {
        if (values is null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        var valuesBySlot = new double[Variables.Count];

        foreach (var variable in Variables)
        {
            if (!TryGetValue(values, variable.Name, out valuesBySlot[variable.Slot]))
            {
                throw new KeyNotFoundException(
                    $"No value was supplied for variable '{variable.Name}'.");
            }
        }

        return root.Evaluate(MathEvaluationContext.ForValues(valuesBySlot));
    }

    public UnaryMathFunction BindUnary(string variableName)
    {
        if (variableName is null)
        {
            throw new ArgumentNullException(nameof(variableName));
        }

        if (Variables.Count == 0)
        {
            return new UnaryMathFunction(root, -1);
        }

        if (Variables.Count != 1 ||
            !string.Equals(Variables[0].Name, variableName,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Expression cannot be bound as a unary function of '{variableName}'.");
        }

        return new UnaryMathFunction(root, Variables[0].Slot);
    }

    private static bool TryGetValue(IDictionary<string, double> values,
        string name, out double value)
    {
        if (values.TryGetValue(name, out value))
        {
            return true;
        }

        foreach (var entry in values)
        {
            if (string.Equals(entry.Key, name, StringComparison.OrdinalIgnoreCase))
            {
                value = entry.Value;
                return true;
            }
        }

        value = 0d;
        return false;
    }
}

public sealed class UnaryMathFunction
{
    private readonly BoundNode root;
    private readonly int variableSlot;

    internal UnaryMathFunction(BoundNode root, int variableSlot)
    {
        this.root = root;
        this.variableSlot = variableSlot;
    }

    public double Evaluate(double value) =>
        root.Evaluate(MathEvaluationContext.ForUnary(variableSlot, value));
}

internal struct MathEvaluationContext
{
    private readonly double[]? values;
    private readonly int unarySlot;
    private readonly double unaryValue;

    private MathEvaluationContext(double[]? values, int unarySlot, double unaryValue)
    {
        this.values = values;
        this.unarySlot = unarySlot;
        this.unaryValue = unaryValue;
    }

    public static MathEvaluationContext ForValues(double[] values) =>
        new(values, -1, 0d);

    public static MathEvaluationContext ForUnary(int variableSlot, double value) =>
        new(null, variableSlot, value);

    public double GetVariable(int slot)
    {
        if (slot == unarySlot)
        {
            return unaryValue;
        }

        if (values is null)
        {
            throw new InvalidOperationException("No value is available for this variable.");
        }

        return values[slot];
    }
}

internal abstract class BoundNode
{
    public abstract double Evaluate(MathEvaluationContext context);
}

internal sealed class BoundConstantNode : BoundNode
{
    private readonly double value;

    public BoundConstantNode(double value)
    {
        this.value = value;
    }

    public override double Evaluate(MathEvaluationContext context) => value;
}

internal sealed class BoundVariableNode : BoundNode
{
    private readonly int slot;

    public BoundVariableNode(int slot)
    {
        this.slot = slot;
    }

    public override double Evaluate(MathEvaluationContext context) =>
        context.GetVariable(slot);
}

internal sealed class BoundPrefixNode : BoundNode
{
    private readonly MathPrefixOperator @operator;
    private readonly BoundNode operand;

    public BoundPrefixNode(MathPrefixOperator @operator, BoundNode operand)
    {
        this.@operator = @operator;
        this.operand = operand;
    }

    public override double Evaluate(MathEvaluationContext context)
    {
        var value = operand.Evaluate(context);

        return @operator == MathPrefixOperator.Negate ? -value : value;
    }
}

internal sealed class BoundPostfixNode : BoundNode
{
    private readonly MathPostfixOperator @operator;
    private readonly BoundNode operand;

    public BoundPostfixNode(MathPostfixOperator @operator, BoundNode operand)
    {
        this.@operator = @operator;
        this.operand = operand;
    }

    public override double Evaluate(MathEvaluationContext context)
    {
        var value = operand.Evaluate(context);

        switch (@operator)
        {
            case MathPostfixOperator.Factorial:
                return Factorial(value);
            default:
                throw new NotSupportedException($"Unsupported postfix operator '{@operator}'.");
        }
    }

    private static double Factorial(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) ||
            value < 0d || value != Math.Floor(value))
        {
            return double.NaN;
        }

        if (value > 170d)
        {
            return double.PositiveInfinity;
        }

        var result = 1d;

        for (var factor = 2; factor <= (int)value; factor++)
        {
            result *= factor;
        }

        return result;
    }
}

internal sealed class BoundBinaryNode : BoundNode
{
    private readonly MathBinaryOperator @operator;
    private readonly BoundNode left;
    private readonly BoundNode right;

    public BoundBinaryNode(MathBinaryOperator @operator, BoundNode left, BoundNode right)
    {
        this.@operator = @operator;
        this.left = left;
        this.right = right;
    }

    public override double Evaluate(MathEvaluationContext context)
    {
        var leftValue = left.Evaluate(context);
        var rightValue = right.Evaluate(context);

        switch (@operator)
        {
            case MathBinaryOperator.Add:
                return leftValue + rightValue;
            case MathBinaryOperator.Subtract:
                return leftValue - rightValue;
            case MathBinaryOperator.Multiply:
                return leftValue * rightValue;
            case MathBinaryOperator.Divide:
                return leftValue / rightValue;
            case MathBinaryOperator.Remainder:
                return leftValue % rightValue;
            case MathBinaryOperator.Power:
                return Math.Pow(leftValue, rightValue);
            default:
                throw new NotSupportedException($"Unsupported binary operator '{@operator}'.");
        }
    }
}

internal sealed class BoundCallNode : BoundNode
{
    private readonly MathFunctionDefinition function;
    private readonly BoundNode[] arguments;

    public BoundCallNode(MathFunctionDefinition function, BoundNode[] arguments)
    {
        this.function = function;
        this.arguments = arguments;
    }

    public override double Evaluate(MathEvaluationContext context)
    {
        switch (function.Arity)
        {
            case 1:
                return function.Invoke(arguments[0].Evaluate(context));
            case 2:
                return function.Invoke(arguments[0].Evaluate(context),
                    arguments[1].Evaluate(context));
            default:
                throw new NotSupportedException(
                    $"Unsupported function arity {function.Arity}.");
        }
    }
}
