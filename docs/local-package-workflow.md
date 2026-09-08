# Build and review a chain through local NuGet packages

Use the same package boundary during local development and after publication.
Producer repositories build packages into a shared directory through
`<PackageOutputPath>$(LocalNuGetPath)</PackageOutputPath>`. Consumer repositories
restore their `PackageReference` dependencies from that directory through a local
`NuGet.Config`. NuGet selects framework assets and resolves transitive dependencies
and version constraints at every repository boundary.

Keep `ProjectReference` relationships within a repository. Building a producer
solution in Release packs its projects; a consumer then builds against those
packages. Building just one project does not replace packing each required
dependency package.

## Configure the shared directory

`Library` and `ltrwebdb` already honor `LocalNuGetPath`. For example, set it in the
build environment so MSBuild imports it and NuGet can expand it in configuration:

```powershell
$env:LocalNuGetPath = 'D:\NuGetLocal'
```

Or in a Unix shell:

```sh
export LocalNuGetPath=/absolute/path/to/local-nuget
```

Add that directory to each repository's existing local `NuGet.Config`. A minimal
standalone configuration is:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="%LocalNuGetPath%" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

NuGet uses `%LocalNuGetPath%` for environment expansion on Windows and Unix.
A literal absolute directory works too. Setting only the MSBuild command-line
property `-p:LocalNuGetPath=...` does not create an environment variable for NuGet;
in that case put the same literal directory in the config. Keep machine-specific
configuration local and retain any other feeds the repository needs.

## Build producers before consumers

For the graph website, the order is `Library`, `ltrwebdb`, then `ltr-data.se`.
The checkouts can live anywhere; each repository uses its own project and package
references. Run these commands from the indicated repository root with the shared
directory configured:

```sh
# Library: Release builds produce the packages for its projects.
dotnet build Library.slnx -c Release

# ltrwebdb: produce both packages, restoring Library dependencies from the feed.
dotnet build LTR.WebDb.Entity/LTR.WebDb.Entity.csproj -c Release
dotnet build webdbcore/webdbcore.csproj -c Release

# ltr-data.se: restore and build against the package chain.
dotnet restore coreweb/coreweb.csproj --force-evaluate
dotnet build coreweb/coreweb.csproj -c Release --no-restore
```

Release builds already enable `GeneratePackageOnBuild` in the producer projects.
If a build explicitly disables it, follow that build with `dotnet pack --no-build`
for each producer project, using the same configuration and framework properties.
The output still uses `LocalNuGetPath`.

The initial website slice uses `LTRData.MathExpression` 1.1.0-preview.1,
`LTRData.FunctionPlotting` 0.1.0, and `LTRData.Graphics.SkiaSharp`
0.1.0-preview.1. The subsequent Windows consumer slice requires
`LTRData.FunctionPlotting` 0.2.0-preview.1 for sample calculus. Keep earlier package
versions available for branches that still reference them. Their other Library
dependencies must also be in the feed.
`ltrwebdb` produces `LTR.WebDb.Entity` and `webdbcore`, currently at 1.0.3.
The website's renderer and database references remain ordinary package references.

`GraphViewer.Review` also consumes the renderer package. After restoring from the
same configured feed, its UI and HTTP checks exercise the packaged parser,
sampling, layout, renderers, and native assets. See the companion website's
`docs/math-rendering-review.md` for commands.

For netexpr and Windows GraphViewer the chain is shorter: build
`LTRData.Extensions`, `LTRData.MathExpression`, and `LTRData.FunctionPlotting` in
Library, then build `netexpr/netexpr.vbproj` in MathTools or
`GraphViewer/GraphViewer.vbproj` in WindowsTools. No database or SkiaSharp packages
are needed. Each consumer branch has a focused review executable and CI workflow;
see [consumer migration](math-expression-redesign/consumer-migration.md).

## Verify the packages you actually built

Package source order does not establish priority for `PackageReference` restores.
For a final local-chain check, use package source mappings to select the local
producer packages, together with a fresh dedicated package cache. For this chain,
add the following section alongside `packageSources` in the local config:

```xml
<packageSourceMapping>
  <clear />
  <packageSource key="local">
    <package pattern="LTRData.*" />
    <package pattern="LTR.WebDb.Entity" />
    <package pattern="webdbcore" />
  </packageSource>
  <packageSource key="nuget.org">
    <package pattern="*" />
  </packageSource>
</packageSourceMapping>
```

These mappings apply to this chain; map only the packages being built locally.
Set `NUGET_PACKAGES` to a fresh directory for the validation run, or use a dedicated
`globalPackagesFolder` in the local config. A package already extracted into the
global cache can be reused without consulting feeds or source mappings.
`--force-evaluate` refreshes dependency resolution; it does not evict cached
package contents. Prefer new package versions for successive builds. When
intentionally replacing the same local version, remove only that version from the
dedicated validation cache before restoring again.

Check `obj/project.assets.json` for package entries, selected framework assets,
versions, and transitive dependencies. The restored package's `.nupkg.metadata`
records its source. The consumer should reference packages throughout this chain.

XML serialization assembly work remains deferred. Local packing and validation
do not publish to NuGet servers; publication remains a later release step.

## Verified graph website chain

The Linux validation built and packed the 11 Library projects required by the
website, followed by both `ltrwebdb` projects. Each of the 13 packages contained
.NET 9 and .NET 10 assemblies and dependency groups. This was a focused build for
the two website targets, not a local validation of every Library project or TFM.

Both website hosts restored with source mappings and a fresh dedicated package
cache. All local package metadata identified the shared output directory as the
source. The full site's resolved graph contained all 13 producer packages, with
matching `lib/net9.0` and `lib/net10.0` assets and no project dependencies. Public
dependencies were restored from an offline mirror of NuGet.org packages.

`coreweb` built for both targets and `GraphViewer.Review` built for .NET 10 with
zero warnings or errors. The review host's HTTP checks passed against the packaged
assemblies, including both native PNG paths, dimensions, diagnostics, formula
limits, cache reuse, gaps and the concurrency limit. Database package generation
retained its existing XML serializer warnings; that work remains deferred.

References: [NuGet configuration](https://learn.microsoft.com/en-us/nuget/reference/nuget-config-file),
[package source mapping](https://learn.microsoft.com/en-us/nuget/consume-packages/package-source-mapping),
and [package caches](https://learn.microsoft.com/en-us/nuget/consume-packages/managing-the-global-packages-and-cache-folders).
