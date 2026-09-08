# netexpr and Windows GraphViewer migration

The companion `experimental/math-expression-migration` branches in
[MathTools](https://github.com/LTRData/MathTools/tree/experimental/math-expression-migration)
and [WindowsTools](https://github.com/LTRData/WindowsTools/tree/experimental/math-expression-migration)
consume the modern APIs through NuGet. All existing application targets remain:
.NET Framework 3.5/4.0 and .NET 8/9/10, with Windows targets for GraphViewer.

## netexpr

`MathParser` produces syntax and diagnostics, `MathBinder` binds the standard
catalog and variable slots, and the bound expression evaluates directly. There
is no generated LINQ expression or runtime delegate compilation.

The command line still joins formula arguments, prompts for invariant-culture
`name=value` assignments and prints an invariant-culture number. Names are
case-insensitive; repeated names share one slot. While other values are still
pending, assigning a parameter again updates it. Blank input is ignored and EOF
reports missing values rather than looping or failing with a null reference.

The historical numeric exit code is deliberately retained: VB's `CInt` conversion
rounds to the nearest integer, with ties to even. Invalid input, non-finite results
and conversion overflow return -1. A successfully calculated nonzero answer is
therefore not a conventional process-success exit code; shells may truncate it.

The modern language applies, including right-associative power and power before
unary sign. Shift/bitwise syntax is rejected. See the language specification for
the supported vocabulary; legacy aliases are not silently added to the catalog.

## GraphViewer

Each committed formula is a `PlotDefinition` containing a bound unary function.
`PlotSnapshot` captures the definitions, viewport and portable geometries. Explicit
overlays replace retained GDI+ paths. Redraw adds a definition when overlay mode is
enabled; repaint, visibility toggles and resize do not. Changing the viewport
recalculates all retained formulas into the same coordinate system. Invalid
formula/range input leaves the last successful snapshot intact.

GraphViewer's small `PlotDrawing` module renders the geometry using System.Drawing
at the Windows application boundary. The same renderer serves the screen,
BMP/GIF/JPEG/PNG/TIFF export and printing. Print geometry is rebuilt for the page
size and positioned within the margins. Printing uses all committed overlays;
editing a formula without redrawing does not change the printed graph.

The application accepts only `x` as a variable. Previous-`y` recurrence remains
legacy-only. No historical `Surface`, `ScriptControl` or `LTRLib.Windows` package
is required by this application. Other legacy consumers can still use them.

## Numerical calculus

`LTRData.FunctionPlotting` 0.2.0-preview.1 adds `SampleCalculus`, independent of
both the expression parser and graphics. Its inputs are ordered data-coordinate
samples with finite, strictly increasing X coordinates and finite spacing.

| Operation | Definition |
| --- | --- |
| `Differentiate` | Three-point differences, including one-sided endpoint formulas; unequal X spacing is supported. Two-point runs use their secant slope; isolated points have no derivative. |
| `IntegrateFiniteRuns` | Cumulative signed trapezoidal areas; each finite run starts at the supplied initial value (zero by default) at its leftmost sample. |
| Gaps | Neither operation crosses an invalid sample. Evaluation-error status remains explicit. Integration starts again at the initial value after a gap. |
| Overflow | A calculated non-finite value stays non-finite; it is not a display wrap or an inferred new integration origin. |

GraphViewer samples at twice the canvas width plus one, bounded to 3–32769 points,
then transforms and clips the original curve, derivative and integral separately.
Integral values have no dependence on the Y viewport. The menu/tooltips identify
the numerical calculation and its initial-value convention.

These are sampled approximations. An unsampled discontinuity may still be joined;
the code does not prove continuity, detect every finite asymptote, or evaluate an
improper integral. Numerical derivatives and integral values can change with
sampling density. Legacy wrapped integrals and stale derivative state across gaps
are intentionally not reproduced.

## Package chain and review

Build `LTRData.Extensions`, `LTRData.MathExpression` and `LTRData.FunctionPlotting`
in Release with `LocalNuGetPath` set, then restore consumers from that shared feed.
netexpr needs the first two packages; GraphViewer needs all three, with Extensions
arriving transitively. MathExpression remains at 1.1.0-preview.1. Use a fresh cache
and package source mapping for a reproducible check of unpublished builds, as in
the [local package workflow](../local-package-workflow.md).

Each consumer branch includes a focused review executable and a CI workflow that
builds its producer packages first. netexpr covers variables, culture, diagnostics,
precedence and numeric exit codes. GraphViewer's Windows checks exercise GDI+
rendering, all five image encoders, margin/graphics-state handling and form overlay,
visibility and resize behavior. Library's calculus tests cover unequal spacing,
short runs, gaps, error status, overflow and analytic comparison with sine/cosine.

Manual Windows review should cover actual print preview/printer selection, DPI
scaling, saved preferences and the visual appearance of curves at chosen ranges.
Compiling the old framework targets does not claim execution on an old Windows
installation. The website can be hosted on Linux or Windows Server for now;
FreeBSD native SkiaSharp work, XML serialization assemblies, adaptive sampling and
the broad final optimization pass remain deferred.
