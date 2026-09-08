using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LTRData.MathExpression.Diagrams;

/// <summary>A finite positive measured node or diagram size, in continuous canvas units.</summary>
public struct DiagramSize
{
    /// <summary>Creates a validated measured size.</summary>
    public DiagramSize(double width, double height)
    {
        if (!IsPositiveFinite(width)) throw new ArgumentOutOfRangeException(nameof(width));
        if (!IsPositiveFinite(height)) throw new ArgumentOutOfRangeException(nameof(height));
        Width = width;
        Height = height;
    }
    /// <summary>Width in canvas units.</summary>
    public double Width { get; }
    /// <summary>Height in canvas units.</summary>
    public double Height { get; }
    internal static bool IsPositiveFinite(double value) => value > 0 && !double.IsInfinity(value);
}

/// <summary>A point in a diagram's continuous coordinate space.</summary>
public struct DiagramPoint
{
    /// <summary>Creates a point.</summary>
    public DiagramPoint(double x, double y) { X = x; Y = y; }
    /// <summary>Horizontal coordinate.</summary>
    public double X { get; }
    /// <summary>Vertical coordinate, increasing downward.</summary>
    public double Y { get; }
}

/// <summary>The rectangle reserved for a measured node.</summary>
public sealed class DiagramNodeLayout
{
    internal DiagramNodeLayout(ExpressionDiagramNode node, double x, double y, DiagramSize size)
    { Node = node; X = x; Y = y; Size = size; }
    /// <summary>Content being positioned.</summary>
    public ExpressionDiagramNode Node { get; }
    /// <summary>Left edge.</summary>
    public double X { get; }
    /// <summary>Top edge.</summary>
    public double Y { get; }
    /// <summary>Measured rectangle size.</summary>
    public DiagramSize Size { get; }
}

/// <summary>A connector from a parent's bottom center to a child's top center.</summary>
public sealed class DiagramConnector
{
    internal DiagramConnector(int parentId, ExpressionDiagramChild child,
        DiagramPoint start, DiagramPoint end)
    { ParentId = parentId; Child = child; Start = start; End = end; }
    /// <summary>Parent node ID.</summary>
    public int ParentId { get; }
    /// <summary>Child node ID and ordered operand role.</summary>
    public ExpressionDiagramChild Child { get; }
    /// <summary>Start point on the parent.</summary>
    public DiagramPoint Start { get; }
    /// <summary>End point on the child.</summary>
    public DiagramPoint End { get; }
}

/// <summary>Immutable, backend-independent rectangles and connectors for a tree.</summary>
public sealed class TreeDiagramLayout
{
    private TreeDiagramLayout(DiagramSize size, List<DiagramNodeLayout> nodes,
        List<DiagramConnector> connectors)
    { Size = size; Nodes = nodes.AsReadOnly(); Connectors = connectors.AsReadOnly(); }
    /// <summary>Total extent, including margins.</summary>
    public DiagramSize Size { get; }
    /// <summary>Positioned nodes indexed by node ID.</summary>
    public ReadOnlyCollection<DiagramNodeLayout> Nodes { get; }
    /// <summary>Connectors in parent and child order.</summary>
    public ReadOnlyCollection<DiagramConnector> Connectors { get; }

    /// <summary>
    /// Lays out measured nodes in non-overlapping subtrees. Measurement is supplied by
    /// the caller's font backend. Runs iteratively in linear time and preserves child order.
    /// </summary>
    /// <param name="diagram">Tree content.</param>
    /// <param name="nodeSizes">Measured sizes indexed by node ID; copied during layout.</param>
    /// <param name="siblingSpacing">Space between adjacent subtree extents.</param>
    /// <param name="levelSpacing">Space between levels.</param>
    /// <param name="margin">Space around the diagram.</param>
    public static TreeDiagramLayout Create(ExpressionDiagram diagram, IList<DiagramSize> nodeSizes,
        double siblingSpacing = 24, double levelSpacing = 40, double margin = 16)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(diagram);
        ArgumentNullException.ThrowIfNull(nodeSizes);
#else
        if (diagram is null) throw new ArgumentNullException(nameof(diagram));
        if (nodeSizes is null) throw new ArgumentNullException(nameof(nodeSizes));
#endif
        ValidateSpacing(siblingSpacing, nameof(siblingSpacing));
        ValidateSpacing(levelSpacing, nameof(levelSpacing));
        ValidateSpacing(margin, nameof(margin));
        var count = diagram.Nodes.Count;
        if (nodeSizes.Count != count)
            throw new ArgumentException("One measured size per node is required.", nameof(nodeSizes));

        var sizes = new DiagramSize[count];
        var widths = new double[count];
        var depth = new int[count];
        var levelHeights = new double[count];
        var levelTops = new double[count];
        var lefts = new double[count];
        var maxDepth = 0;
        for (var i = 0; i < count; i++)
        {
            var size = nodeSizes[i];
            if (!DiagramSize.IsPositiveFinite(size.Width) || !DiagramSize.IsPositiveFinite(size.Height))
                throw new ArgumentException("Node sizes must be finite and positive.", nameof(nodeSizes));
            sizes[i] = size;
            levelHeights[depth[i]] = Math.Max(levelHeights[depth[i]], size.Height);
            maxDepth = Math.Max(maxDepth, depth[i]);
            foreach (var child in diagram.Nodes[i].Children) depth[child.NodeId] = depth[i] + 1;
        }

        for (var i = count - 1; i >= 0; i--)
        {
            var children = diagram.Nodes[i].Children;
            var childWidth = 0d;
            foreach (var child in children) childWidth += widths[child.NodeId];
            if (children.Count > 1) childWidth += siblingSpacing * (children.Count - 1);
            widths[i] = Math.Max(sizes[i].Width, childWidth);
        }

        levelTops[0] = margin;
        for (var i = 1; i <= maxDepth; i++)
            levelTops[i] = levelTops[i - 1] + levelHeights[i - 1] + levelSpacing;

        // Constructor detects overflow before any coordinates are given to a backend.
        var extent = new DiagramSize(widths[0] + 2 * margin,
            levelTops[maxDepth] + levelHeights[maxDepth] + margin);
        lefts[0] = margin;
        var placed = new List<DiagramNodeLayout>(count);
        for (var i = 0; i < count; i++)
        {
            placed.Add(new DiagramNodeLayout(diagram.Nodes[i],
                lefts[i] + (widths[i] - sizes[i].Width) / 2, levelTops[depth[i]], sizes[i]));
            var children = diagram.Nodes[i].Children;
            var total = 0d;
            foreach (var child in children) total += widths[child.NodeId];
            if (children.Count > 1) total += siblingSpacing * (children.Count - 1);
            var nextLeft = lefts[i] + (widths[i] - total) / 2;
            foreach (var child in children)
            {
                lefts[child.NodeId] = nextLeft;
                nextLeft += widths[child.NodeId] + siblingSpacing;
            }
        }

        var connectors = new List<DiagramConnector>(count - 1);
        foreach (var parent in placed)
            foreach (var child in parent.Node.Children)
            {
                var target = placed[child.NodeId];
                connectors.Add(new DiagramConnector(parent.Node.Id, child,
                    new DiagramPoint(parent.X + parent.Size.Width / 2, parent.Y + parent.Size.Height),
                    new DiagramPoint(target.X + target.Size.Width / 2, target.Y)));
            }
        return new TreeDiagramLayout(extent, placed, connectors);
    }

    private static void ValidateSpacing(double value, string name)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < 0)
            throw new ArgumentOutOfRangeException(name);
    }
}
