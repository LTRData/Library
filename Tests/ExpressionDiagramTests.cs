using LTRData.MathExpression;
using LTRData.MathExpression.Diagrams;
using System;
using System.Linq;
using Xunit;

namespace LTRData.Extensions.Tests;

public class ExpressionDiagramTests
{
    [Fact]
    public void DiagramPreservesOperandOrderSpellingAndSourceLocations()
    {
        var diagram = Diagram("atan2(X, .50) - 2x");
        Assert.Equal(new[] { "−", "atan2", "X", ".50", "×", "2", "x" },
            diagram.Nodes.Select(n => n.Label));
        Assert.Equal(new[] { "left", "right" }, diagram.Nodes[0].Children.Select(c => c.Role));
        Assert.Equal(new[] { "argument 1", "argument 2" }, diagram.Nodes[1].Children.Select(c => c.Role));
        Assert.Equal(new SourceSpan(6, 1), diagram.Nodes[2].Span);
        Assert.Equal(Enumerable.Range(0, diagram.Nodes.Count), diagram.Nodes.Select(n => n.Id));
    }

    [Fact]
    public void ParenthesesAreAnExplicitPresentationChoice()
    {
        var syntax = MathParser.Default.Parse("((x))").Root!;
        Assert.Single(ExpressionDiagram.FromSyntax(syntax).Nodes);
        Assert.Equal(new[] { "( )", "( )", "x" },
            ExpressionDiagram.FromSyntax(syntax, true).Nodes.Select(n => n.Label));
        Assert.Throws<ArgumentException>(() => ExpressionDiagram.FromSyntax(syntax, maxNodes: 2));
    }

    [Fact]
    public void LayoutRespectsMeasuredSizesAndKeepsUnbalancedSubtreesApart()
    {
        var diagram = Diagram("long_variable + sin(x^2 + max(y, 100))");
        var sizes = diagram.Nodes.Select((n, i) => new DiagramSize(30 + n.Label.Length * 12, 30 + i % 3 * 10)).ToArray();
        var layout = TreeDiagramLayout.Create(diagram, sizes);
        Assert.Equal(diagram.Nodes.Count - 1, layout.Connectors.Count);
        foreach (var node in layout.Nodes)
        {
            Assert.Equal(sizes[node.Node.Id].Width, node.Size.Width);
            Assert.InRange(node.X, 0, layout.Size.Width - node.Size.Width);
            Assert.InRange(node.Y, 0, layout.Size.Height - node.Size.Height);
            foreach (var other in layout.Nodes.Where(n => n.Node.Id > node.Node.Id))
                Assert.False(node.X < other.X + other.Size.Width && other.X < node.X + node.Size.Width &&
                    node.Y < other.Y + other.Size.Height && other.Y < node.Y + node.Size.Height);
            var childNodes = node.Node.Children.Select(c => layout.Nodes[c.NodeId]).ToArray();
            for (var i = 1; i < childNodes.Length; i++) Assert.True(childNodes[i - 1].X < childNodes[i].X);
        }
        foreach (var connector in layout.Connectors)
        {
            var parent = layout.Nodes[connector.ParentId];
            var child = layout.Nodes[connector.Child.NodeId];
            Assert.Equal(parent.Y + parent.Size.Height, connector.Start.Y);
            Assert.Equal(child.Y, connector.End.Y);
            Assert.True(connector.End.Y > connector.Start.Y);
        }
        sizes[0] = new DiagramSize(1, 1);
        Assert.NotEqual(1, layout.Nodes[0].Size.Width);
    }

    [Fact]
    public void SingleNodeExtentIncludesMargins()
    {
        var layout = TreeDiagramLayout.Create(Diagram("x"), new[] { new DiagramSize(40, 30) }, margin: 10);
        Assert.Equal(60, layout.Size.Width);
        Assert.Equal(50, layout.Size.Height);
        Assert.Empty(layout.Connectors);
    }

    [Fact]
    public void DeepManualSyntaxDoesNotRequireRecursiveTraversalOrLayout()
    {
        MathSyntax syntax = new NameSyntax("x", new SourceSpan(0, 1));
        for (var i = 0; i < 2000; i++)
            syntax = new PrefixSyntax(MathPrefixOperator.Negate, new SourceSpan(0, 1), syntax, syntax.Span);
        var diagram = ExpressionDiagram.FromSyntax(syntax);
        var layout = TreeDiagramLayout.Create(diagram, Enumerable.Repeat(new DiagramSize(20, 20), diagram.Nodes.Count).ToArray());
        Assert.Equal(2001, layout.Nodes.Count);
        Assert.True(layout.Size.Height > 40000);
        Assert.Throws<ArgumentException>(() => ExpressionDiagram.FromSyntax(syntax, maxNodes: 2000));
    }

    [Fact]
    public void InvalidMeasurementsAndOverflowAreRejectedBeforeRendering()
    {
        var diagram = Diagram("x+x");
        Assert.Throws<ArgumentException>(() => TreeDiagramLayout.Create(diagram, new DiagramSize[3]));
        Assert.Throws<ArgumentException>(() => TreeDiagramLayout.Create(diagram, new[] { new DiagramSize(1, 1) }));
        Assert.Throws<ArgumentOutOfRangeException>(() => TreeDiagramLayout.Create(diagram,
            Enumerable.Repeat(new DiagramSize(double.MaxValue, 20), 3).ToArray()));
    }

    private static ExpressionDiagram Diagram(string source) => ExpressionDiagram.FromSyntax(MathParser.Default.Parse(source).Root!);
}
