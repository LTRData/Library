# LTRData Library

A collection of reusable .NET libraries used by tools and applications by Olof
Lagerkvist, LTR Data. The repository contains independently versioned library
projects covering general utilities, data and networking, geographical
calculations, mathematical expressions, plotting, and platform integration.

## Packages and components

Each library project below uses its project name as its NuGet package ID. Choose
the packages needed by your application; dependencies between them are declared
in the project files. Namespaces can differ from package names.

| Package / source | Purpose |
| --- | --- |
| [LTRData.Extensions](https://github.com/LTRData/Library/blob/master/LTRData.Extensions/LTRData.Extensions.csproj) | General-purpose extensions for buffers and spans, streams, collections, formatting, reflection, numeric operations, and command-line parsing. |
| [LTRData.Extensions.Native](https://github.com/LTRData/Library/blob/master/LTRData.Extensions.Native/LTRData.Extensions.Native.csproj) | Native memory helpers, memory comparison, and native library/symbol access; individual APIs have OS and runtime dependencies. |
| [LTRData.Data](https://github.com/LTRData/Library/blob/master/LTRData.Data/LTRData.Data.csproj) | Collection and query helpers, caching, CSV reading/writing, reflection-based data mapping, and Entity Framework integration. |
| [LTRData.Xml](https://github.com/LTRData/Library/blob/master/LTRData.Xml/LTRData.Xml.csproj) | XML serialization and extensions, configuration, and time-zone-aware date/time helpers. |
| [LTRData.Net](https://github.com/LTRData/Library/blob/master/LTRData.Net/LTRData.Net.csproj) | HTTP and download helpers, IP address ranges, network extensions, and pipeline helpers. |
| [LTRData.Web](https://github.com/LTRData/Library/blob/master/LTRData.Web/LTRData.Web.csproj) | RSS models/downloads and linked XML resources. Server-specific helpers live in WebServerUtils. |
| [LTRData.WebServerUtils](https://github.com/LTRData/Library/blob/master/LTRData.WebServerUtils/LTRData.WebServerUtils.csproj) | HTTP server utilities for classic ASP.NET and ASP.NET Core, including response/file handling and HTTPS-related helpers. |
| [LTRData.Geodesy](https://github.com/LTRData/Library/blob/master/LTRData.Geodesy/LTRData.Geodesy.csproj) | WGS84, RT90 and SWEREF99 coordinates and conversions, grid squares, solar events, and moon-phase calculations. |
| [LTRData.PolyGeometry](https://github.com/LTRData/Library/blob/master/LTRData.PolyGeometry/LTRData.PolyGeometry.csproj) | Point, line, path, polygon and bounding-rectangle types, with WKT parsing. |
| [LTRData.Placemarks](https://github.com/LTRData/Library/blob/master/LTRData.Placemarks/LTRData.Placemarks.csproj) | Simplified KML models, region lookup and geographical geometry helpers; modern targets use NetTopologySuite. |
| [LTRData.Graphics](https://github.com/LTRData/Library/blob/master/LTRData.Graphics/LTRData.Graphics.csproj) | Color conversion/matching and 3D vector extensions. |
| [LTRData.MathExpression](https://github.com/LTRData/Library/blob/master/LTRData.MathExpression/LTRData.MathExpression.csproj) | Math parsing, diagnostics, binding and interpreted evaluation, plus portable expression-tree diagram content/layout. Legacy parser APIs remain available. |
| [LTRData.FunctionPlotting](https://github.com/LTRData/Library/blob/master/LTRData.FunctionPlotting/LTRData.FunctionPlotting.csproj) | Parser-independent function sampling, clipped plot geometry, and numerical differentiation/integration of samples. |
| [LTRData.Graphics.SkiaSharp](https://github.com/LTRData/Library/blob/master/LTRData.Graphics.SkiaSharp/LTRData.Graphics.SkiaSharp.csproj) | SkiaSharp renderers for function plots and expression diagrams, plus PNG encoding. |
| [LTRData.Compat.System.Management](https://github.com/LTRData/Library/blob/master/LTRData.Compat.System.Management/LTRData.Compat.System.Management.csproj) | A System.Management-style API over Microsoft.Management.Infrastructure (CIM), including asynchronous operations; not a complete System.Management replacement. |
| [LTRData.PerformanceCounters](https://github.com/LTRData/Library/blob/master/LTRData.PerformanceCounters/LTRData.PerformanceCounters.csproj) | Thermal-zone temperature enumeration through Windows performance counters and, on applicable targets, Linux sysfs. |

For example, add the general extensions package to an application:

```sh
dotnet add package LTRData.Extensions
```

See [LTRData packages on NuGet](https://www.nuget.org/packages?q=LTRData)
for published versions and their framework assets. Source on the default branch
may be newer than a published package.

## Target frameworks and platform dependencies

Most library projects inherit these build targets from
[Directory.Build.props](https://github.com/LTRData/Library/blob/master/Directory.Build.props):

- .NET Framework 3.5, 4.0, 4.6 and 4.8.
- .NET Standard 2.0 and 2.1.
- .NET 8, 9 and 10.

The exceptions are:

| Package | Targets |
| --- | --- |
| LTRData.Compat.System.Management | .NET Framework 4.6/4.8, .NET Standard 2.0/2.1, .NET 8/9/10 |
| LTRData.WebServerUtils | .NET Framework 4.0/4.6/4.8, .NET Standard 2.0/2.1, .NET 8/9/10 |
| LTRData.Graphics.SkiaSharp | .NET 8/9/10 |

These are compilation targets, not a guarantee that every API is available on
every framework or operating system. Conditional compilation and dependencies
vary by target; consult the linked project files and the API you intend to use.

- The modern expression and plotting code is independent of a graphics backend.
  Rendering through SkiaSharp requires compatible native assets and fonts. Linux
  applications need the matching Linux native package; FreeBSD native deployment
  remains deferred. See the [rendering and deployment notes](https://github.com/LTRData/Library/blob/master/docs/math-expression-redesign/rendering-slice.md).
- WebServerUtils uses System.Web on .NET Framework and ASP.NET Core dependencies
  on modern targets, including the Microsoft.AspNetCore.App shared framework for
  .NET 8/9/10. LTRData.Web is kept separate from these server runtime references.
- Native helpers, CIM operations and temperature readings depend on the relevant
  operating-system libraries, services or hardware interfaces. A .NET Standard
  target alone does not establish platform availability.

## Mathematical expressions and plotting

The modern API separates parsing and binding in LTRData.MathExpression, numerical
sampling and geometry in LTRData.FunctionPlotting, and native rendering in
LTRData.Graphics.SkiaSharp. FunctionPlotting also accepts ordinary numerical
delegates without requiring an expression parser.

Start with the [expression language specification](https://github.com/LTRData/Library/blob/master/docs/math-expression-redesign/language-specification.md),
[rendering examples](https://github.com/LTRData/Library/blob/master/docs/math-expression-redesign/rendering-slice.md),
and [consumer migration and sample calculus notes](https://github.com/LTRData/Library/blob/master/docs/math-expression-redesign/consumer-migration.md).
Fixed-count sampling does not guarantee detection of every discontinuity or
finite asymptote; adaptive sampling and previous-value recurrence remain deferred.

The [design documentation](https://github.com/LTRData/Library/blob/master/docs/math-expression-redesign/README.md)
preserves the investigation and implementation history. Some sections describe
earlier experimental checkpoints; the modern APIs are now part of this repository.

## Building and testing

Use the .NET 10 SDK, as in the [CI workflow](https://github.com/LTRData/Library/blob/master/.github/workflows/build-test.yml).
Building all targets also requires the corresponding reference assemblies;
running tests requires the runtime for the selected target.

From the repository root:

```sh
dotnet restore Library.slnx
dotnet build Library.slnx -c Release --no-restore
dotnet test Library.slnx -c Release --no-build --no-restore -f net10.0
```

CI is configured to build on Windows, Ubuntu and macOS. It runs the configured
test targets on Windows and .NET 10 tests on Ubuntu/macOS. The main test project
targets .NET Framework 4.8 and .NET 8/9/10; Rendering.Tests targets .NET 8/9/10 and
exercises native SkiaSharp rendering.

Release builds generate NuGet packages. Set LocalNuGetPath to a local feed
directory when building packages for other repositories, and build the required
dependency packages as well. The [local package workflow](https://github.com/LTRData/Library/blob/master/docs/local-package-workflow.md)
explains output configuration, consumer restore, and package-origin verification.

## Related code and provenance

Consumers include the
[netexpr command-line tool](https://github.com/LTRData/MathTools) and
[Windows GraphViewer](https://github.com/LTRData/WindowsTools).
The separate [LTRLib repository](https://github.com/LTRData/LTRLib) retains legacy
libraries, including Windows drawing compatibility used by older consumers.
The [migration notes](https://github.com/LTRData/Library/blob/master/docs/math-expression-redesign/consumer-migration.md)
explain how netexpr and GraphViewer use the newer APIs here.

Geodesy includes work derived from Björn Sållarp's
[MightyLittleGeodesy](https://github.com/bjornsallarp/MightyLittleGeodesy), extended
by Olof Lagerkvist. Existing source headers retain authorship and license notices.
