/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using LTRData.Extensions.Formatting;
#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP
using LTRData.Extensions.Split;
#endif
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable IDE0056 // Use index operator

namespace LTRData.PolyGeometry;

[Guid("19844407-A5CA-4ED9-AC72-C86B1862446F")]
[ClassInterface(ClassInterfaceType.None)]
[ComDefaultInterface(typeof(IPolyGeometry))]
[SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
public sealed class Polygon(IList<Point> points) : ReadOnlyCollection<Line>(Validate(points.GetLines())), IPolyGeometry
{
    public Polygon(params Point[] points)
        : this(points as IList<Point>)
    {
    }

    private static IList<Line> Validate(IList<Line> lines)
    {
        if (lines is null ||
            lines.Count == 0)
        {
            throw new ArgumentException("No lines defined.", nameof(lines));
        }

        // Close the polygon boundaries if necessary
        if (lines[0].X0Y0 != lines[lines.Count - 1].X1Y1)
        {
            var new_lines = new Line[lines.Count + 1];
            lines.CopyTo(new_lines, 0);
            new_lines[new_lines.Length - 1] = new Line(lines[lines.Count - 1].X1Y1, lines[0].X0Y0);
            lines = new_lines;
        }

        return lines;
    }

    public static Polygon Parse(string text) => new(Array.ConvertAll(text.Split(','), Point.Parse));

#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static Polygon Parse(ReadOnlyMemory<char> text) => new([.. text.TokenEnum(',').Select(Point.Parse)]);
#endif

    public override string ToString() => Corners.Select(point => point.ToString()).Join(", ");

    public bool Contains(Point point)
    {
        if (!this.MBRContains(point))
        {
            return false;
        }

        var intersectingLines = this
            .Count(line =>
                point.Y >= line.MinY &&
                point.Y <= line.MaxY &&
                point.X <= line.GetXForY(point.Y));

        return (intersectingLines & 1) == 1;
    }

    public double MinX => this.Min(line => line.MinX);

    public double MinY => this.Min(line => line.MinY);

    public double MaxX => this.Max(line => line.MaxX);

    public double MaxY => this.Max(line => line.MaxY);

    public double Length => this.Sum(line => line.Length);

    public double Area
    {
        get
        {
            double area = 0;         // Accumulates area in the loop
            var j = Count - 1;  // The last vertex is the 'previous' one to the first

            for (var i = 0; i < Count; i++)
            {
                area += (this[j].X0Y0.X + this[i].X0Y0.X) *
                    (this[j].X0Y0.Y - this[i].X0Y0.Y);

                j = i;  //j is previous vertex to i
            }

            return area / 2;
        }
    }

    public IEnumerable<Point> Corners => this.Select(line => line.X0Y0);

    public IEnumerable<Line> Bounds => this;
}
