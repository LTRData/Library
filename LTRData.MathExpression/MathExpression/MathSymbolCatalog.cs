using System;
using System.Collections.Generic;

namespace LTRData.MathExpression;

#pragma warning disable CS1591

public sealed class MathSymbolCatalog
{
    private static readonly MathSymbolCatalog standard = CreateStandard();

    private readonly Dictionary<string, double> constants;
    private readonly Dictionary<string, Dictionary<int, MathFunctionDefinition>> functions;

    internal MathSymbolCatalog(Dictionary<string, double> constants,
        Dictionary<string, Dictionary<int, MathFunctionDefinition>> functions)
    {
        this.constants = new Dictionary<string, double>(constants,
            StringComparer.OrdinalIgnoreCase);
        this.functions = new Dictionary<string, Dictionary<int, MathFunctionDefinition>>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var entry in functions)
        {
            this.functions.Add(entry.Key,
                new Dictionary<int, MathFunctionDefinition>(entry.Value));
        }
    }

    public static MathSymbolCatalog Standard => standard;

    public static MathSymbolCatalogBuilder CreateBuilder() => new();

    public MathSymbolCatalogBuilder ToBuilder() => new(constants, functions);

    internal bool TryGetConstant(string name, out double value) =>
        constants.TryGetValue(name, out value);

    internal bool TryGetFunction(string name, int arity,
        out MathFunctionDefinition? function)
    {
        if (functions.TryGetValue(name, out var overloads) &&
            overloads.TryGetValue(arity, out var result))
        {
            function = result;
            return true;
        }

        function = null;
        return false;
    }

    private static MathSymbolCatalog CreateStandard()
    {
        return CreateBuilder()
            .AddConstant("e", Math.E)
            .AddConstant("pi", Math.PI)
            .AddFunction("abs", x => Math.Abs(x))
            .AddFunction("acos", x => Math.Acos(x))
            .AddFunction("asin", x => Math.Asin(x))
            .AddFunction("atan", x => Math.Atan(x))
            .AddFunction("atan2", (y, x) => Math.Atan2(y, x))
            .AddFunction("ceiling", x => Math.Ceiling(x))
            .AddFunction("cos", x => Math.Cos(x))
            .AddFunction("cosh", x => Math.Cosh(x))
            .AddFunction("exp", x => Math.Exp(x))
            .AddFunction("floor", x => Math.Floor(x))
            .AddFunction("ln", x => Math.Log(x))
            .AddFunction("log", x => Math.Log(x))
            .AddFunction("log", (x, @base) => Math.Log(x, @base))
            .AddFunction("log10", x => Math.Log10(x))
            .AddFunction("max", (left, right) => Math.Max(left, right))
            .AddFunction("min", (left, right) => Math.Min(left, right))
            .AddFunction("pow", (left, right) => Math.Pow(left, right))
            .AddFunction("round", x => Math.Round(x))
            .AddFunction("sign", x => Math.Sign(x))
            .AddFunction("sin", x => Math.Sin(x))
            .AddFunction("sinh", x => Math.Sinh(x))
            .AddFunction("sqrt", x => Math.Sqrt(x))
            .AddFunction("tan", x => Math.Tan(x))
            .AddFunction("tanh", x => Math.Tanh(x))
            .AddFunction("truncate", x => Math.Truncate(x))
            .Build();
    }
}

public sealed class MathSymbolCatalogBuilder
{
    private readonly Dictionary<string, double> constants =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, Dictionary<int, MathFunctionDefinition>> functions =
        new(StringComparer.OrdinalIgnoreCase);

    public MathSymbolCatalogBuilder()
    {
    }

    internal MathSymbolCatalogBuilder(Dictionary<string, double> constants,
        Dictionary<string, Dictionary<int, MathFunctionDefinition>> functions)
    {
        foreach (var constant in constants)
        {
            this.constants.Add(constant.Key, constant.Value);
        }

        foreach (var function in functions)
        {
            this.functions.Add(function.Key,
                new Dictionary<int, MathFunctionDefinition>(function.Value));
        }
    }

    public MathSymbolCatalogBuilder AddConstant(string name, double value)
    {
        ValidateName(name);

        if (constants.ContainsKey(name))
        {
            throw new ArgumentException($"A constant named '{name}' is already registered.",
                nameof(name));
        }

        constants.Add(name, value);
        return this;
    }

    public MathSymbolCatalogBuilder AddFunction(string name, Func<double, double> function)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(function);
#else
        if (function is null)
        {
            throw new ArgumentNullException(nameof(function));
        }
#endif

        AddFunction(name, new MathFunctionDefinition(function));
        return this;
    }

    public MathSymbolCatalogBuilder AddFunction(string name,
        Func<double, double, double> function)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(function);
#else
        if (function is null)
        {
            throw new ArgumentNullException(nameof(function));
        }
#endif

        AddFunction(name, new MathFunctionDefinition(function));
        return this;
    }

    public MathSymbolCatalog Build() => new(constants, functions);

    private void AddFunction(string name, MathFunctionDefinition function)
    {
        ValidateName(name);

        if (!functions.TryGetValue(name, out var overloads))
        {
            overloads = new Dictionary<int, MathFunctionDefinition>();
            functions.Add(name, overloads);
        }

        if (overloads.ContainsKey(function.Arity))
        {
            throw new ArgumentException(
                $"A function named '{name}' with arity {function.Arity} is already registered.",
                nameof(name));
        }

        overloads.Add(function.Arity, function);
    }

    private static void ValidateName(string name)
    {
        if (name is null || name.Trim().Length == 0)
        {
            throw new ArgumentException("A symbol name is required.", nameof(name));
        }

        if (!(char.IsLetter(name[0]) || name[0] == '_'))
        {
            throw new ArgumentException($"Invalid symbol name '{name}'.", nameof(name));
        }

        for (var index = 1; index < name.Length; index++)
        {
            if (!(char.IsLetterOrDigit(name[index]) || name[index] == '_'))
            {
                throw new ArgumentException($"Invalid symbol name '{name}'.", nameof(name));
            }
        }
    }
}

internal sealed class MathFunctionDefinition
{
    private readonly Func<double, double>? unary;
    private readonly Func<double, double, double>? binary;

    public MathFunctionDefinition(Func<double, double> unary)
    {
        this.unary = unary;
        Arity = 1;
    }

    public MathFunctionDefinition(Func<double, double, double> binary)
    {
        this.binary = binary;
        Arity = 2;
    }

    public int Arity { get; }

    public double Invoke(double argument)
    {
        if (unary is null)
        {
            throw new InvalidOperationException("Function is not unary.");
        }

        return unary(argument);
    }

    public double Invoke(double left, double right)
    {
        if (binary is null)
        {
            throw new InvalidOperationException("Function is not binary.");
        }

        return binary(left, right);
    }
}
