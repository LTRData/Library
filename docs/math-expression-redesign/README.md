# Math expression and function plotting redesign

This directory records the decisions behind the experimental replacement APIs on
`experimental/math-expression-redesign`.

The historical implementation is evidence, not the specification. Observed behavior is
classified before it becomes a test or a public contract:

- **A — desired modern behavior:** normative for the new API.
- **B — legacy compatibility behavior:** relevant only to explicit compatibility code.
- **C — historical implementation artifact or bug:** intentionally not preserved.

## Documents

- [First-party usage corpus](usage-corpus.md)
- [Legacy behavior classification](legacy-behavior-classification.md)
- [Modern expression language specification](language-specification.md)
- [Architecture, package boundaries, and proposed APIs](architecture.md)
- [Implemented rendering slice, samples, deployment and review](rendering-slice.md)

## Original core slice

The branch validates only these boundaries:

- an immutable math-specific syntax model with source spans;
- a real lexer and parser with diagnostics;
- an explicit immutable symbol catalog;
- binding to constants, functions, and arbitrary variable slots;
- interpreted evaluation without runtime code generation;
- convenient allocation-free unary evaluation for plotting;
- parser-independent fixed-count function sampling;
- explicit sample status for non-finite values and evaluation failures;
- backend-neutral Cartesian polyline geometry with viewport clipping.

That original slice did not implement rendering. The subsequent accepted rendering
slice adds portable diagrams/layout, one SkiaSharp edge package and the companion
website PNG integration. System.Drawing migration, derivative/integral calculation,
recurrence, adaptive sampling and LINQ-expression compilation remain deferred.

The existing `MathExpressionParser`, `IMathExpressionParser`, `MathFunctions`, and
`ScriptControl` remain temporarily so the experimental API can be reviewed without a
broad consumer migration. They do not define the new language.

## Decisions made for the experiment

- The existing package name `LTRData.MathExpression` remains. It is a reasonable domain
  name and changing it would not isolate a dependency.
- Function plotting is a separate package because it is independently useful, must not
  reference the parser, and lets arbitrary numerical callers avoid the expression package
  and its current `LTRData.Extensions` dependency.
- Expression-diagram content and layout are planned inside `LTRData.MathExpression` unless
  a concrete dependency or independent-consumption reason emerges.
- The SkiaSharp edge package is justified because it isolates an external native
  rendering dependency and different deployment requirements.
- System.Drawing compatibility remains in `LTRLib.Windows` initially.
- The modern expression language is invariant-culture, source-preserving, and
  case-insensitive for names.
- Power is right-associative and binds more strongly than unary sign, so `2^3^2` is
  `2^(3^2)` and `-2^2` is `-(2^2)`.
- The ordinary plotting contract is unary `f(x) -> y`. Previous-`y` recurrence is not in
  the new primary API because no first-party formula using it was found.

## Remaining design questions

- The first SkiaSharp package now contains both plot and diagram renderers because
  they share the same dependency and deployment boundary.
- Whether legacy function aliases should later be offered by a separately named symbol
  catalog in Library or only by an LTRLib adapter.
- Whether plotting needs adaptive sampling after fixed-count sampling and correct clipping
  have been evaluated against the web and Windows applications.
- Whether diagrams need a separate bound semantic mode. The first implementation is
  a syntax projection with optional parenthesis nodes.
- Whether new projects should eventually drop .NET Framework 3.5/4.0. This branch keeps
  the repository target matrix while compatibility policy is decided.

## Current review checkpoint

The architecture and object model were accepted. The first rendering slice is now
implemented: diagram content/layout inside the math package,
`LTRData.Graphics.SkiaSharp`, and both PNG paths on the companion website branch.
Review the [API, sample images and deployment notes](rendering-slice.md) and the
website review host before evaluating derivative, integral, adaptive sampling and
Windows migration.
