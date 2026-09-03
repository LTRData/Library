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

## Scope of this experimental slice

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

It deliberately does **not** implement SkiaSharp, PNG output, expression-diagram
rendering, System.Drawing migration, derivative/integral calculation, recurrence,
adaptive sampling, or LINQ-expression compilation.

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
- A future SkiaSharp edge package is justified because it isolates an external native
  rendering dependency and different deployment requirements.
- System.Drawing compatibility remains in `LTRLib.Windows` initially.
- The modern expression language is invariant-culture, source-preserving, and
  case-insensitive for names.
- Power is right-associative and binds more strongly than unary sign, so `2^3^2` is
  `2^(3^2)` and `-2^2` is `-(2^2)`.
- The ordinary plotting contract is unary `f(x) -> y`. Previous-`y` recurrence is not in
  the new primary API because no first-party formula using it was found.

## Remaining design questions

- Whether the first SkiaSharp package should contain both plot and expression-diagram
  renderers or use two edge packages. One combined package is currently preferred because
  the dependency and deployment boundary is the same.
- Whether legacy function aliases should later be offered by a separately named symbol
  catalog in Library or only by an LTRLib adapter.
- Whether plotting needs adaptive sampling after fixed-count sampling and correct clipping
  have been evaluated against the web and Windows applications.
- Whether syntax diagrams need both source-oriented and semantic modes. Both remain the
  likely design, but rendering is outside this slice.
- Whether new projects should eventually drop .NET Framework 3.5/4.0. This branch keeps
  the repository target matrix while compatibility policy is decided.

## Recommended next slice

After this branch is reviewed and the language decisions are accepted:

1. add expression-diagram content and measured-size tree layout inside
   `LTRData.MathExpression`;
2. add one SkiaSharp edge package containing plot and diagram renderers plus PNG encoding;
3. integrate both PNG paths into `ltr-data.se` with input/resource limits and Linux tests;
4. only then evaluate derivative, integral, adaptive sampling, and Windows migration.
