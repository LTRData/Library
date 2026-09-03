# First-party expression and graph usage corpus

## Search scope

The default branches and available history were searched in:

- `LTRData/Library`;
- `LTRData/LTRLib`;
- `LTRData/WindowsTools`, especially `GraphViewer`;
- `LTRData/MathTools`, especially `netexpr`;
- `LTRData/ltr-data.se` through the GitHub repository connection;
- the historical `richardneish/ltrdata` GraphViewer sources.

Searches covered parser and graph type names, formula/expression settings and assignments,
common mathematical call syntax, documentation, tests, project files, resources, and
historical GraphViewer implementations.

## Checked-in formula corpus

### Application defaults

| Expression | Source | What it demonstrates |
|---|---|---|
| `sin(x)` | Current `WindowsTools/GraphViewer/My Project/Settings.settings` | Unary variable, standard function call, case-insensitive historical input |
| `sin(x)` | Historical GraphViewer settings and application configuration | Long-lived default, unchanged across the scripting, generated-VB, and current parser eras |
| `sin(x)` | Disabled `ltr-data.se/coreweb/Pages/GraphViewer.cshtml.cs` | Same server-side plotting use case with invariant parsing |

No other persisted end-user formula is checked into the first-party applications. The
GraphViewer combo box accepts arbitrary text and stores the most recent value in per-user
settings, but those user values are not repository evidence.

### Current Library tests

| Expression or family | Demonstrated intent |
|---|---|
| `sin(0.4) * 2` | Standard unary function and multiplication |
| `atan2(312,2)` | Two-argument function and comma separator |
| `e ** 2` | Named constant and `**` power spelling |
| `169 - 5 - 3 - 1` | Left-associated subtraction |
| `169 - (5 - 3 - 1)` | Parentheses |
| `169 (5 - 3 - 1)` | Implicit multiplication before parentheses |
| mixed `+`, `-`, `*`, `/` examples | Ordinary arithmetic precedence |
| `.1` and `.01` | Leading-decimal-point numeric literals |
| `1 << 10` | Shift operator was intentionally added to the legacy parser |
| unary sign chains such as `--35`, `+-35`, and `-+35` | Legacy prefix-sign handling |

Tests prove that somebody deliberately added support, but they do not by themselves prove
that a feature belongs in the modern graph-oriented expression language. The shift case is
therefore classified as legacy-only pending a real consumer.

## API and feature usage

### Windows GraphViewer

`GraphView.vb`:

- owns one mutable `ScriptControl` and one retained `Surface`;
- assigns formula text and calls `Surface.Refresh` on recalculation;
- paints the retained function, derivative, integral, and axes;
- creates another surface for printing;
- exports BMP, GIF, JPEG, PNG, and TIFF through System.Drawing;
- lets the user retain old graphs by disabling clear-before-redraw.

The persisted defaults are X `-10..10`, Y `-5..5`, derivative enabled, integral disabled,
and clear-before-redraw enabled. Overlaying prior plots is an intentional UI feature, but
retained `GraphicsPath` mutation is only its historical implementation.

The project targets modern Windows TFMs and .NET Framework 3.5/4.0 from the same VB project.
That is evidence for keeping old-target compatibility in LTRLib until target support is
explicitly reconsidered; it is not a reason to constrain all new APIs indefinitely.

### `netexpr`

`MathTools/netexpr` accepts arbitrary formula text, discovers arbitrary parameters, asks
the user for invariant-culture values, dynamically compiles a LINQ lambda, and invokes it.
Its actual requirements are:

- arbitrary variables;
- stable variable discovery;
- invariant input/output;
- direct evaluation.

It does not require graphics, `x`/`y` special treatment, or a LINQ expression as the public
syntax model. Dynamic compilation is an implementation choice rather than a requirement.

### `ltr-data.se`

The disabled GraphViewer page caches compiled `ScriptControl` objects by formula, builds an
ImageSharp-era `Surface`, optionally draws derivative/integral curves, and encodes a raster
response. The current practical requirement is two server-generated PNGs: a function plot
and an expression diagram.

The unbounded compiled-expression cache and unbounded image dimensions are application
risks to correct during integration, not behavior for a library API to preserve.

## Symbol vocabulary evidence

The current `MathFunctions` and historical `Functions.vb`/`Functions.vbs` provide aliases
such as `sqr`, `sgn`, `atn`, `sec`, `cosec`, `cotan`, arc variants, hyperbolic variants,
`ln`, `logn`, and `fac`. This proves historical availability, not checked-in formula use.
The modern standard catalog therefore starts with conventional names. A separately named
legacy catalog can restore aliases if migration data shows a need.

## Previous-`y` recurrence search

No checked-in formula intentionally reads `y`. The relevant uses of `y` are inside
`Surface.Refresh` and its VB ancestors, where the previous result is passed into the next
evaluation. The only checked-in application formula is unary `sin(x)`.

Conclusion: the modern plotting input is a unary scalar function. Recurrence is category B
at most and should be a separately named compatibility concept if external evidence later
requires it.

## Expression-tree renderer search

No expression-tree renderer was found under names including `ExpressionTree`,
`ExpressionNode`, `DrawExpression`, or “expression tree” in the inspected first-party
default branches or the historical GraphViewer repository. The current parser's LINQ
expression output is a tree representation, but no observed first-party code lays it out or
renders it as an image.

Expression-diagram design is therefore a new requirement, not a renderer port. If another
historical repository is identified, it should be reviewed as additional evidence and
classified rather than adopted automatically.

