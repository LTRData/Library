# Third-Party Notices

LTRData Library is distributed under the [MIT License](LICENSE), except where
third-party material is identified separately below. Third-party software and
source code remain subject to their respective licenses.

This document also records licensing context for dependencies where automated
license scanners may report license texts carried in an upstream package's
third-party notices rather than the primary license of that package or of this
repository.

## MightyLittleGeodesy

Portions of `LTRData.Geodesy` are derived from
[MightyLittleGeodesy](https://github.com/bjornsallarp/MightyLittleGeodesy) by
Björn Sållarp and have subsequently been modified and extended by Olof
Lagerkvist, LTR Data.

The affected source files retain the original copyright and license notice:

- `LTRData.Geodesy/Conversion/GaussKreuger.cs`
- `LTRData.Geodesy/Positions/LatLonPosition.cs`
- `LTRData.Geodesy/Positions/RT90Position.cs`
- `LTRData.Geodesy/Positions/SWEREF99Position.cs`
- `LTRData.Geodesy/Positions/WGS84Position.cs`

The original material is licensed as follows:

> Copyright (C) 2009 Björn Sållarp
>
> Permission is hereby granted, free of charge, to any person obtaining a copy of
> this software and associated documentation files (the "Software"), to deal in
> the Software without restriction, including without limitation the rights to
> use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
> the Software, and to permit persons to whom the Software is furnished to do so,
> subject to the following conditions:
>
> The above copyright notice and this permission notice shall be included in all
> copies or substantial portions of the Software.
>
> THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
> IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
> FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
> AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
> LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
> OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
> SOFTWARE.

## SkiaSharp

`LTRData.Graphics.SkiaSharp` references
[SkiaSharp](https://www.nuget.org/packages/SkiaSharp), which is distributed under
the MIT License.

The non-packable `Rendering.Tests` project also references
[`SkiaSharp.NativeAssets.Linux`](https://www.nuget.org/packages/SkiaSharp.NativeAssets.Linux)
to execute rendering tests on Linux. `Rendering.Tests` declares
`IsPackable=false` and is not included in LTRData NuGet packages.

SkiaSharp native-asset packages carry an upstream `THIRD-PARTY-NOTICES.txt`
containing license notices for third-party material associated with SkiaSharp and
its upstream dependencies. That notice includes, among others, MPL, GPL, LGPL
and eCos license text. Automated scanners can therefore report those licenses
against `SkiaSharp.NativeAssets.Linux` even though the package itself is
published under the MIT License.

The presence of those upstream license texts does not make LTRData Library
licensed under each detected license. Applications that redistribute SkiaSharp
native binaries should retain and comply with the third-party notices supplied
with the corresponding SkiaSharp distribution.

### FOSSA scanner context

A FOSSA licensing report generated for this repository on 2026-09-14 reported
MPL-1.1, GPL-2.0-or-later, LGPL-2.1-or-later, eCos-2.0 and LGPL-2.1-only against
`SkiaSharp.NativeAssets.Linux` 4.151.2. Those findings correspond to license text
present in SkiaSharp's bundled third-party notices. The direct
`SkiaSharp.NativeAssets.Linux` reference in this repository is confined to the
non-packable `Rendering.Tests` project.

## Microsoft ASP.NET Core packages

Some target frameworks use Microsoft ASP.NET Core NuGet packages. These remain
subject to their upstream licenses.

In particular,
[`Microsoft.AspNetCore.Mvc.Core`](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.Core)
2.3.13 is licensed under the Apache License 2.0. A FOSSA licensing report
generated for this repository on 2026-09-14 identified that version as
"Unlicensed". That classification does not reflect the upstream package license.

## Other NuGet dependencies

Projects in this repository use additional NuGet packages under their respective
upstream licenses. Package references in project files and restored dependency
metadata determine the dependency versions used by a particular build.

This document is intended to identify embedded third-party source and licensing
situations requiring additional explanation. It is not intended to duplicate a
complete generated software bill of materials.
