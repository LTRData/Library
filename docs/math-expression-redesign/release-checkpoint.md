# Stable package review checkpoint

The architecture and object model have been accepted, and the application owner
has built and tested all consuming applications successfully. This checkpoint
prepares the branches for review; it does not publish packages or deploy a site.

| Package | Version |
| --- | --- |
| LTRData.MathExpression | 1.1.0 |
| LTRData.FunctionPlotting | 1.2.0 |
| LTRData.Graphics.SkiaSharp | 1.1.0 |
| LTR.WebDb.Entity (ltrwebdb) | 1.0.4 |
| webdbcore (ltrwebdb) | 1.0.4 |

The math and plotting APIs use `ArgumentNullException.ThrowIfNull` on .NET 6 or
later, selected by `NET6_0_OR_GREATER`. Existing explicit checks remain for older
targets. Non-negative source-span checks similarly use
`ArgumentOutOfRangeException.ThrowIfNegative` with `NET8_0_OR_GREATER`. Renderer
projects already target supported runtimes. Exception parameter names and public
contracts stay the same.

The database repository confines XML serializer generation to `LTR.WebDb.Entity`,
pins both the generator build package and CLI registration, verifies generation,
and packages a serializer DLL with each framework's entity assembly. Its review
application consumes the real package and checks published XML round trips. The
website consumes database package 1.0.4 and no longer suppresses serialization
assembly generation globally.

Review and merge Library first, then ltrwebdb, then the website. MathTools and
WindowsTools depend only on Library. Each consumer review workflow builds a pinned
Library revision into a shared local NuGet feed before restoring the consumer.
The [local package workflow](../local-package-workflow.md) remains the normal way
to validate the complete chain, including internal-only packages.

FreeBSD compatibility remains deferred. Other deferred features include previous-y
recurrence, adaptive sampling, automatic detection of every finite asymptote,
semantic diagrams and optional expression compilation. The website's UI still
offers the function and expression tree; adding numerical derivative/integral
controls there is a separate feature.
