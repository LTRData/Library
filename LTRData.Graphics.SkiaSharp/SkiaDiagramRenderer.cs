using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LTRData.MathExpression;
using LTRData.MathExpression.Diagrams;
using SkiaSharp;

namespace LTRData.Graphics.SkiaSharp;

/// <summary>Skia-specific node appearance and measurement padding.</summary>
public sealed record SkiaDiagramTheme
{
    /// <summary>Default diagram styling.</summary>
    public static SkiaDiagramTheme Default { get; } = new();
    /// <summary>Text color.</summary>
    public SKColor TextColor { get; init; } = new(24, 37, 57);
    /// <summary>Node outline and connector color.</summary>
    public SKColor LineColor { get; init; } = new(129, 146, 169);
    /// <summary>Operand node fill.</summary>
    public SKColor OperandColor { get; init; } = new(245, 247, 250);
    /// <summary>Operator and function node fill.</summary>
    public SKColor OperationColor { get; init; } = new(225, 238, 255);
    /// <summary>Horizontal padding on each side of a label.</summary>
    public float HorizontalPadding { get; init; } = 16;
    /// <summary>Vertical padding on each side of a label.</summary>
    public float VerticalPadding { get; init; } = 10;
}

/// <summary>Measures and renders expression labels using the caller's actual Skia font.</summary>
public static class SkiaDiagramRenderer
{
    /// <summary>Measures rectangles for portable layout, indexed by diagram node ID.</summary>
    public static ReadOnlyCollection<DiagramSize> MeasureNodes(ExpressionDiagram diagram,
        SKFont font, SkiaDiagramTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(diagram);
        ArgumentNullException.ThrowIfNull(font);
        theme ??= SkiaDiagramTheme.Default;
        if (!float.IsFinite(theme.HorizontalPadding) || theme.HorizontalPadding < 0 ||
            !float.IsFinite(theme.VerticalPadding) || theme.VerticalPadding < 0 ||
            !float.IsFinite(font.Size) || font.Size <= 0)
            throw new ArgumentOutOfRangeException(nameof(theme), "Font size and padding must be finite and valid.");
        var metrics = font.Metrics;
        var sizes = new List<DiagramSize>(diagram.Nodes.Count);
        foreach (var node in diagram.Nodes)
        {
            var advance = font.MeasureText(node.Label, out var bounds);
            sizes.Add(new DiagramSize(
                Math.Max(1, Math.Max(advance, bounds.Width)) + 2 * theme.HorizontalPadding,
                Math.Max(1, Math.Max(metrics.Descent - metrics.Ascent, bounds.Height)) + 2 * theme.VerticalPadding));
        }
        return sizes.AsReadOnly();
    }

    /// <summary>
    /// Draws at the layout's native size; the caller may transform/clip its canvas to fit.
    /// Use the same font for measurement and rendering. The caller owns all supplied objects.
    /// </summary>
    public static void Render(SKCanvas canvas, TreeDiagramLayout layout, SKFont font,
        SkiaDiagramTheme? theme = null)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        ArgumentNullException.ThrowIfNull(layout);
        ArgumentNullException.ThrowIfNull(font);
        if (layout.Size.Width > float.MaxValue || layout.Size.Height > float.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(layout));
        theme ??= SkiaDiagramTheme.Default;
        using var line = new SKPaint { Color = theme.LineColor, Style = SKPaintStyle.Stroke, StrokeWidth = 1.25f, IsAntialias = true };
        using var fill = new SKPaint { IsAntialias = true };
        using var text = new SKPaint { Color = theme.TextColor, IsAntialias = true };
        using var path = new SKPathBuilder();
        foreach (var connector in layout.Connectors)
        {
            var middle = (float)((connector.Start.Y + connector.End.Y) / 2);
            path.Reset();
            path.MoveTo((float)connector.Start.X, (float)connector.Start.Y);
            path.LineTo((float)connector.Start.X, middle);
            path.LineTo((float)connector.End.X, middle);
            path.LineTo((float)connector.End.X, (float)connector.End.Y);
            using var connectorPath = path.Detach();
            canvas.DrawPath(connectorPath, line);
        }
        foreach (var node in layout.Nodes)
        {
            var rect = SKRect.Create((float)node.X, (float)node.Y,
                (float)node.Size.Width, (float)node.Size.Height);
            fill.Color = node.Node.Kind is MathSyntaxKind.Number or MathSyntaxKind.Name
                ? theme.OperandColor : theme.OperationColor;
            canvas.DrawRoundRect(rect, 6, 6, fill);
            canvas.DrawRoundRect(rect, 6, 6, line);
            font.MeasureText(node.Node.Label, out var bounds);
            canvas.DrawText(node.Node.Label,
                rect.MidX - (bounds.Left + bounds.Right) / 2,
                rect.MidY - (bounds.Top + bounds.Bottom) / 2,
                SKTextAlign.Left, font, text);
        }
    }
}
