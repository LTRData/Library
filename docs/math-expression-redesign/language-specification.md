# Modern mathematical expression language — experimental specification

This document is normative for the experimental `MathParser`/`MathBinder` API. It is not a
description of `MathExpressionParser`.

## 1. Source and names

- Source text is preserved exactly in `MathParseResult.SourceText`.
- Every syntax node and diagnostic has a zero-based source span.
- Whitespace separates or surrounds tokens and is otherwise insignificant.
- Identifiers use a Unicode letter or `_` as the first character, followed by Unicode
  letters, digits, or `_`.
- Identifier lookup is ordinal case-insensitive. Original spelling remains in syntax.
- A bare identifier resolves first as a catalog constant and otherwise becomes a variable.
- An identifier followed immediately by `(` is a function call; unknown functions and
  wrong arities are binding diagnostics.

## 2. Numeric literals and culture

The language is culture-invariant:

- decimal separator is `.`;
- function arguments are separated by `,`;
- group/thousands separators are not accepted;
- literals may contain digits before and/or after `.`, but at least one digit is required;
- scientific notation uses `e` or `E` and an optional `+`/`-` exponent sign.

Examples: `12`, `12.`, `.5`, `1.25`, `1e-3`, `2.5E+6`.

User interfaces may localize entry separately, but thread culture never changes the
formula language.

## 3. Primary expressions and calls

Primary expressions are:

- numeric literals;
- names;
- function calls `name(argument, ...)`;
- parenthesized expressions `(expression)`.

Zero-argument calls are syntactically valid so a catalog may define them. The standard
catalog currently defines unary and binary functions only.

## 4. Operators

From highest to lowest binding strength:

| Level | Operators | Associativity |
|---:|---|---|
| 1 | postfix factorial `!` | left/repeated |
| 2 | power `^`, `**` | right |
| 3 | prefix identity/negation `+`, `-` | right |
| 4 | multiplication `*`, division `/`, remainder `%`/`mod`, implicit multiplication | left |
| 5 | addition `+`, subtraction `-` | left |

Power binds more strongly than unary sign:

- `-2^2` is `-(2^2)` and evaluates to `-4`;
- `2^-2` is `2^(-2)` and evaluates to `0.25`;
- `2^3^2` is `2^(3^2)` and evaluates to `512`;
- `2*3^2` is `2*(3^2)` and evaluates to `18`.

`**` is an alias for `^`. `mod` is a case-insensitive alias for `%`.

### Implicit multiplication

Implicit multiplication is supported when an already parsed operand is followed by an
identifier or `(` without an explicit binary operator. It has exactly the precedence and
left associativity of `*`.

Examples:

- `2x` means `2*x`;
- `2(x+1)` means `2*(x+1)`;
- `(x+1)(x-1)` means `(x+1)*(x-1)`;
- `2sin(x)` means `2*sin(x)`.

An identifier directly followed by `(` is always a function call. Write `x*(y)` rather
than `x(y)` when multiplication is intended. Adjacent numeric literals are rejected rather
than multiplied.

## 5. Factorial

Postfix factorial accepts a finite, non-negative integer-valued `double`:

- `0!` and `1!` are `1`;
- a negative, fractional, or `NaN` operand produces `NaN`;
- an operand greater than `170` produces positive infinity because the exact factorial
  exceeds the finite `double` range.

This intentionally differs from the legacy loop, which returned one for negative inputs
and effectively truncated some fractional inputs.

## 6. Standard symbol catalog

The experimental standard catalog contains constants:

- `e`;
- `pi`.

It contains conventional functions available consistently for `double`:

- unary: `abs`, `acos`, `asin`, `atan`, `ceiling`, `cos`, `cosh`, `exp`, `floor`, `ln`,
  `log`, `log10`, `round`, `sign`, `sin`, `sinh`, `sqrt`, `tan`, `tanh`, `truncate`;
- binary: `atan2`, `log`, `max`, `min`, `pow`.

`log(x)` and `ln(x)` are natural logarithms. `log(x, b)` uses base `b`.

Additional constants and unary/binary functions can be supplied through an explicit
catalog builder. Catalogs are immutable after construction. Reflection is not part of the
standard lookup process.

Historical aliases and bitwise/shift operations are not in the modern standard catalog.
They may be offered later by an explicitly named legacy catalog.

## 7. Binding and variables

Parsing does not decide whether a name is a constant or variable. Binding resolves names
against a supplied symbol catalog:

1. a bare name matching a constant binds to that constant;
2. another bare name becomes a variable;
3. a call must match a function name and arity or binding fails.

Variables receive stable zero-based slots in first-occurrence traversal order. Evaluation
can supply values by slot or by case-insensitive name.

Unary function binding succeeds when the expression has no variables or has exactly one
variable matching the requested name. It fails explicitly when other variables remain.

## 8. Errors and floating-point semantics

- Lexical and syntactic errors are returned as source-span diagnostics; invalid or empty
  input does not become zero.
- Binding errors such as an unknown function or wrong arity are diagnostics.
- Direct evaluation uses normal IEEE `double` arithmetic.
- Division by zero and domain behavior from `System.Math` therefore produce infinity or
  `NaN` where .NET does so.
- Exceptions thrown by explicitly registered custom functions propagate from direct
  evaluation.
- Function sampling classifies `NaN`, positive infinity, negative infinity, and evaluation
  exceptions as distinct sample statuses. It does not confuse those states with zero.

## 9. Formatting and diagrams

The exact source and the syntax tree are distinct from a future canonical formatter.
`Expression.ToString()` output is not formula identity.

Parenthesized syntax is preserved, allowing a future source-oriented diagram. Binding
removes syntactic parentheses, allowing a semantic operation diagram. Formatting and
diagram rendering are not implemented in this experimental slice.

## 10. Intentional omissions

The initial modern language omits:

- shifts `<<`/`>>`;
- bitwise `&`, `|`, `xor`, and prefix `!`;
- prefix `neg`;
- textual infix `pow`;
- reflection-discovered methods/fields;
- assignments, comparisons, booleans, conditionals, and statements.

These can be reconsidered as language features only with a use case and explicit semantics.

