# Legacy behavior classification

Each row is deliberately classified as exactly one of:

- **A:** desired modern behavior and eligible for normative new-API tests;
- **B:** legacy compatibility only;
- **C:** discarded artifact or bug.

“Feature” and “mechanism” are separate rows when the feature is wanted but its historical
implementation is not.

## Expression language and evaluation

| Observed behavior | Class | Evidence and decision |
|---|:---:|---|
| Parse ordinary arithmetic, parentheses, functions, constants, and variables | A | Required by all expression consumers. |
| Arbitrary variables | A | `netexpr` discovers and prompts for arbitrary parameters. |
| Case-insensitive names | A | Long-standing behavior and convenient for interactive formula input; source spelling is still preserved. |
| Invariant `.` decimal syntax and comma argument separator | A | Both current applications construct the parser with invariant culture; a stable language must not change with server/thread culture. |
| Leading decimal point such as `.1` | A | Explicit current test and common calculator syntax. |
| Scientific notation with signed exponent | A | Normal numeric syntax; replacement tokenization currently breaks it. |
| `+`, `-`, `*`, `/`, `%`/`mod`, `^`/`**` | A | Ordinary mathematical/calculator operations; `**` and implicit multiplication have direct tests. |
| Left associativity for addition, subtraction, multiplication, division, and remainder | A | Conventional and tested for subtraction/arithmetic. |
| Right-associative power | A | Conventional modern decision: `2^3^2` means `2^(3^2)`. |
| Power binds more strongly than unary sign | A | Conventional modern decision: `-2^2` means `-(2^2)` while `2^-2` remains valid. |
| Implicit multiplication | A | Explicit existing test. It has the same precedence/associativity as `*`; ambiguous forms should use parentheses. |
| Postfix factorial | A | Fits the mathematical domain. Modern semantics require a finite non-negative integer; invalid input yields `NaN`, and overflow yields positive infinity. |
| Preserve original source text and node spans | A | Needed for diagnostics and expression diagrams. |
| Explicit immutable constant/function catalog | A | Supports extension without reflection, trimming surprises, or mutable global/provider state. |
| IEEE `NaN` and infinity from valid floating-point operations | A | Evaluation remains `double`; sampling classifies non-finite results separately. |
| LINQ `Expression` as the parsed representation | C | Runtime-code-generation model leaks into parsing and loses lexical/source structure. |
| Runtime compilation as the baseline evaluator | C | Unnecessary for correctness and unsuitable as the AOT baseline. An optional exporter/compiler may be added later. |
| Binding every expression to exactly `x` and `y` | C | Contradicted by `netexpr`; plotting can explicitly request unary binding. |
| Previous-`y` recurrence in ordinary plotting | B | Implemented historically, but no first-party formula using it was found. If retained, it must be explicitly named. |
| Lower-case the complete input | C | Original spelling is useful source data; comparison can be case-insensitive without mutation. |
| Store `Expression.ToString()` as formula identity | C | Generated representation is neither source nor a language-level canonical format. |
| Reflection scan of mutable `ProviderTypes` including non-public members | C | Accidental extensibility mechanism; explicit catalogs are predictable and AOT-compatible. |
| Reflection-based provider import | B | May be useful as a separately named migration adapter, never as the default catalog. |
| Mutate thread culture while formatting generated expression text | C | Hidden global/thread state with unsafe restoration on failure. |
| Convert every evaluation exception to `null` | B | Legacy graph gaps depend on it, but modern evaluation and sampling have explicit error/status contracts. |
| Current equal-precedence left-to-right power/multiply scan | C | Produces `(2*3)^2` for `2*3^2`; no consumer evidence supports it. |
| Replacement-based tokenization | C | Breaks scientific notation and identifiers containing operator substrings. |
| Empty input becomes numeric zero | C | Hides missing user input; modern parser reports an expected-expression diagnostic. |
| Prefix `neg` and prefix `!` | C | No usage evidence; `!` is especially misleading alongside postfix factorial. |
| Infix textual `pow` | C | No usage evidence; `pow(a,b)`, `^`, and `**` are clear alternatives. |
| Shifts and bitwise operators | B | A shift has a parser test but no application formula; it does not belong in the initial mathematical plotting language. |
| Historical aliases such as `atn`, `sqr`, `sgn`, `cosec`, `cotan`, and `harcsin` | B | Historically exposed but absent from checked-in formulas. A legacy catalog can provide them later. |
| Private `System.Math` methods accepted as symbols | C | Implementation leakage with trimming and versioning risk. |
| Legacy factorial truncation/negative-input result of one | C | Surprising numerical artifact rather than defensible factorial semantics. |
| Apparent integer-operand reflection call bug for bitwise operations | C | A bug must not become a compatibility contract. |

## Plot calculation and rendering

| Observed behavior | Class | Evidence and decision |
|---|:---:|---|
| Plot an arbitrary scalar function independently of its origin | A | Required architectural boundary and useful beyond expression parsing. |
| Explicit finite/non-finite/evaluation-error samples | A | Necessary for robust geometry and server rendering. |
| Explicit overlay composition | A | GraphViewer intentionally lets users retain old graphs. |
| Derivative display as a feature | A | Enabled by default in GraphViewer, but deferred from the validation slice. |
| Numerical integral display as a feature | A | Exposed by GraphViewer, but deferred and disabled by default. |
| Backend-neutral continuous plot geometry | A | Required by both System.Drawing and Unix PNG rendering. |
| Exactly `width + 1` samples tied to rectangle pixels | C | Rectangle-loop artifact. New sampling takes an explicit count independent of rendering. |
| Pass the prior result as the next `y` automatically | B | Same recurrence decision as above. |
| Draw a segment only when an endpoint is strictly within Y bounds | C | Omits crossing segments and conflates visibility with numerical continuity. |
| Clamp coordinates to one pixel outside the viewport | C | Renderer/path workaround. New geometry uses continuous transforms and clipping. |
| Treat exceptions as gaps but let `NaN`/infinity reach conversion/path code | C | Inconsistent accidental behavior; statuses are explicit. |
| Silently swallow path-construction exceptions | C | Hides defects and mixes rendering failures with calculation. |
| Forward-difference derivative between adjacent samples | B | Exact output may matter only to legacy rendering. A future modern derivative should be specified independently. |
| Carry stale derivative state across an invalid gap | C | Clear bug. |
| Right-rectangle cumulative integration | B | Possible compatibility mode if exact old output matters; not selected as modern default. |
| Wrap the integral to the opposite Y boundary | C | Viewport changes mathematical state; clipping must never alter the numerical integral. |
| Reset integral to zero after invalid samples | B | Legacy behavior only; future modern integration needs explicit segment/initial-value semantics. |
| Append paths on `Refresh` until `Clear` | C | Mutation artifact. The desired overlay feature is represented explicitly instead. |
| Always draw axes and clamp off-range axes outside the area | C | Visibility decision should be explicit in plot composition/rendering. |
| `Surface` owns disposable backend paths and a finalizer | C | Portable samples and geometry require no disposal; backend resources are short-lived at the edge. |
| System.Drawing in the Windows adapter | B | Valid compatibility/backend choice, but not part of portable APIs. |
| SkiaSharp in a Unix PNG edge package | A | Selected first backend direction; it must not shape core models. |

## Test policy

New tests use explicit naming/categories:

- `ModernMathExpressionTests` and plotting tests are normative category A tests.
- `LegacyMathExpressionCharacterizationTests` document B/C observations against the
  existing parser only.
- A legacy characterization failure is evidence to revisit documentation; it must not be
  “fixed” by changing the new parser to reproduce the old result.
