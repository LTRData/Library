/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
using LTRData.Extensions.Buffers;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace LTRData.PolyGeometry;

[Guid("E0E3A059-9B13-407A-BF15-00A1EF07A374")]
[ClassInterface(ClassInterfaceType.AutoDual)]
public static class PolyGeometry
{
    public static bool MBRContains(this IPolyGeometry geometry, Point point)
    {
        return (point.X >= geometry.MinX) && (point.X <= geometry.MaxX) &&
            (point.Y >= geometry.MinY) && (point.Y <= geometry.MaxY);
    }

    public static Rectangle GetMBR(this IPolyGeometry geometry) =>
        new(new(geometry.MinX, geometry.MinY), new(geometry.MaxX, geometry.MaxY));

    public static List<Line> GetLines(this IList<Point> points)
    {
        if (points is null ||
            points.Count == 0)
        {
            throw new ArgumentException("No points defined.", nameof(points));
        }

        if (points.Count == 1)
        {
            return
                [
                    new Line(points[0], points[0])
                ];
        }

        var lines = new List<Line>(points.Count - 1);

        using (var pointEnum = points.GetEnumerator())
        {
            if (!pointEnum.MoveNext())
            {
                throw new ArgumentException("No points defined.", nameof(points));
            }

            var startPoint = pointEnum.Current;

            while (pointEnum.MoveNext())
            {
                lines.Add(new Line(startPoint, pointEnum.Current));
                startPoint = pointEnum.Current;
            }
        }

        return lines;
    }

    public static IPolyGeometry FromWKT(string WKT)
    {
        var dataPos = WKT.IndexOf('(');
        if (dataPos < 0)
        {
            throw new ArgumentException("Invalid text", nameof(WKT));
        }

#if NET46_OR_GREATER || NETSTANDARD || NETCOREAPP
        var prefix = WKT.AsMemory(0, dataPos).Trim();
        var data = WKT.AsMemory(0, dataPos).Trim([' ', '(', ')']);

        if ("point".Equals(prefix.Span, StringComparison.OrdinalIgnoreCase))
        {
            return Point.Parse(data);
        }
        else if ("polygon".Equals(prefix.Span, StringComparison.OrdinalIgnoreCase))
        {
            return Polygon.Parse(data);
        }
        else if ("multipolygon".Equals(prefix.Span, StringComparison.OrdinalIgnoreCase))
        {
            return MultiPolygon.Parse(data.ToString());
        }
#else
        var prefix = WKT.Remove(dataPos).Trim();
        var data = WKT.Substring(dataPos).Trim(' ', '(', ')');

        if ("point".Equals(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return Point.Parse(data);
        }
        else if ("polygon".Equals(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return Polygon.Parse(data);
        }
        else if ("multipolygon".Equals(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return MultiPolygon.Parse(data);
        }
#endif

        throw new ArgumentException("Invalid text", nameof(WKT));
    }
}
