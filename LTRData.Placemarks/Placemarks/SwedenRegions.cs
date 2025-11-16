/*
 * LTRLib
 * 
 * Copyright (c) Olof Lagerkvist, LTR Data
 * http://ltr-data.se   https://github.com/LTRData
 */
#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace LTRData.Placemarks;

[Guid("C1CAD7F3-6110-43B0-A8EC-09166183268C")]
[ClassInterface(ClassInterfaceType.AutoDual)]
public class SwedenRegions(
    string? municipalitiesXmlPath = null,
    string? countiesXmlPath = null,
    string? electoralXmlPath = null)
{
    public Region[]? Municipalities => _Municipalities?.Result;

    public Region[]? Counties => _Counties?.Result;

    public Region[]? Electoral => _Electoral?.Result;

    private readonly Task<Region[]>? _Municipalities = municipalitiesXmlPath is not null ? PlacemarkSupport.ParseKmlRegionsAsync(municipalitiesXmlPath) : null;

    private readonly Task<Region[]>? _Counties = countiesXmlPath is not null ? PlacemarkSupport.ParseKmlRegionsAsync(countiesXmlPath) : null;

    private readonly Task<Region[]>? _Electoral = electoralXmlPath is not null ? PlacemarkSupport.ParseKmlRegionsAsync(electoralXmlPath) : null;
}

#endif
