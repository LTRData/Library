using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LTRData.MathExpression.Diagrams;

/// <summary>An ordered child occurrence in an expression diagram.</summary>
public sealed class ExpressionDiagramChild
{
    internal ExpressionDiagramChild(int nodeId, string role)
    {
        NodeId = nodeId;
        Role = role;
    }

    /// <summary>Index of the child in <see cref="ExpressionDiagram.Nodes"/>.</summary>
    public int NodeId { get; }

    /// <summary>The operand or argument's role, independent of its visual position.</summary>
    public string Role { get; }
}

/// <summary>Backend-independent content for one occurrence of a syntax node.</summary>
public sealed class ExpressionDiagramNode
{
    internal ExpressionDiagramNode(int id, MathSyntax syntax, string label,
        IList<ExpressionDiagramChild> children)
    {
        Id = id;
        Kind = syntax.Kind;
        Span = syntax.Span;
        Label = label;
        Children = new List<ExpressionDiagramChild>(children).AsReadOnly();
    }

    /// <summary>Stable zero-based preorder index within this diagram.</summary>
    public int Id { get; }
    /// <summary>Original syntax category.</summary>
    public MathSyntaxKind Kind { get; }
    /// <summary>Original source location.</summary>
    public SourceSpan Span { get; }
    /// <summary>Node label, without renderer markup.</summary>
    public string Label { get; }
    /// <summary>Children in mathematical operand/argument order.</summary>
    public ReadOnlyCollection<ExpressionDiagramChild> Children { get; }
}

/// <summary>Immutable expression-tree content, independent of layout and graphics.</summary>
public sealed class ExpressionDiagram
{
    private ExpressionDiagram(List<ExpressionDiagramNode> nodes) => Nodes = nodes.AsReadOnly();

    /// <summary>Nodes in preorder; the root is always node zero.</summary>
    public ReadOnlyCollection<ExpressionDiagramNode> Nodes { get; }

    /// <summary>
    /// Projects syntax into a tree. Names and numeric spelling are preserved; operators
    /// use conventional labels. Parenthesis nodes may be retained explicitly. This is
    /// a syntax projection, not algebraic simplification or a bound semantic diagram.
    /// </summary>
    /// <param name="syntax">The root syntax.</param>
    /// <param name="includeParentheses">Whether to show grouping as separate nodes.</param>
    /// <param name="maxNodes">Maximum visited syntax occurrences, including omitted grouping.</param>
    public static ExpressionDiagram FromSyntax(MathSyntax syntax,
        bool includeParentheses = false, int maxNodes = 4096)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(syntax);
#else
        if (syntax is null) throw new ArgumentNullException(nameof(syntax));
#endif
        if (maxNodes < 1) throw new ArgumentOutOfRangeException(nameof(maxNodes));

        var syntaxNodes = new List<MathSyntax>();
        var labels = new List<string>();
        var children = new List<List<ExpressionDiagramChild>>();
        var pending = new Stack<PendingNode>();
        pending.Push(new PendingNode(syntax, -1, string.Empty));
        var visited = 0;

        // Iterative traversal also handles deeply nested, manually constructed syntax.
        while (pending.Count != 0)
        {
            var next = pending.Pop();
            var current = next.Syntax;
            if (++visited > maxNodes)
                throw new ArgumentException("The expression exceeds the diagram node limit.", nameof(syntax));

            if (!includeParentheses && current is ParenthesizedSyntax grouping)
            {
                pending.Push(new PendingNode(grouping.Expression, next.Parent, next.Role));
                continue;
            }

            var id = syntaxNodes.Count;
            syntaxNodes.Add(current);
            children.Add(new List<ExpressionDiagramChild>());
            if (next.Parent >= 0)
                children[next.Parent].Add(new ExpressionDiagramChild(id, next.Role));

            switch (current)
            {
                case NumberSyntax number:
                    labels.Add(number.Lexeme);
                    break;
                case NameSyntax name:
                    labels.Add(name.Name);
                    break;
                case ParenthesizedSyntax parenthesized:
                    labels.Add("( )");
                    pending.Push(new PendingNode(parenthesized.Expression, id, "expression"));
                    break;
                case PrefixSyntax prefix:
                    labels.Add(prefix.Operator == MathPrefixOperator.Negate ? "−" : "+");
                    pending.Push(new PendingNode(prefix.Operand, id, "operand"));
                    break;
                case PostfixSyntax postfix:
                    labels.Add("!");
                    pending.Push(new PendingNode(postfix.Operand, id, "operand"));
                    break;
                case BinarySyntax binary:
                    labels.Add(BinaryLabel(binary.Operator));
                    pending.Push(new PendingNode(binary.Right, id, "right"));
                    pending.Push(new PendingNode(binary.Left, id, "left"));
                    break;
                case CallSyntax call:
                    labels.Add(call.Name);
                    for (var i = call.Arguments.Count - 1; i >= 0; i--)
                        pending.Push(new PendingNode(call.Arguments[i], id,
                            "argument " + (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture)));
                    break;
                default:
                    throw new ArgumentException("Unsupported syntax node.", nameof(syntax));
            }
        }

        var nodes = new List<ExpressionDiagramNode>(syntaxNodes.Count);
        for (var i = 0; i < syntaxNodes.Count; i++)
            nodes.Add(new ExpressionDiagramNode(i, syntaxNodes[i], labels[i], children[i]));
        return new ExpressionDiagram(nodes);
    }

    private static string BinaryLabel(MathBinaryOperator op) => op switch
    {
        MathBinaryOperator.Add => "+",
        MathBinaryOperator.Subtract => "−",
        MathBinaryOperator.Multiply => "×",
        MathBinaryOperator.Divide => "÷",
        MathBinaryOperator.Remainder => "mod",
        MathBinaryOperator.Power => "^",
        _ => throw new ArgumentOutOfRangeException(nameof(op))
    };

    private sealed class PendingNode
    {
        public PendingNode(MathSyntax syntax, int parent, string role)
        {
            Syntax = syntax;
            Parent = parent;
            Role = role;
        }
        public MathSyntax Syntax { get; }
        public int Parent { get; }
        public string Role { get; }
    }
}
