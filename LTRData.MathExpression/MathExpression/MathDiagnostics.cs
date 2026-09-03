using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LTRData.MathExpression;

#pragma warning disable CS1591

public sealed class MathDiagnostic
{
    public MathDiagnostic(string code, string message, SourceSpan span)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Span = span;
    }

    public string Code { get; }

    public string Message { get; }

    public SourceSpan Span { get; }

    public override string ToString() => $"{Code} at {Span}: {Message}";
}

public sealed class MathParseResult
{
    internal MathParseResult(string sourceText, MathSyntax? root,
        IEnumerable<MathDiagnostic> diagnostics)
    {
        SourceText = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
        Root = root;
        Diagnostics = new List<MathDiagnostic>(diagnostics).AsReadOnly();
    }

    public string SourceText { get; }

    public MathSyntax? Root { get; }

    public ReadOnlyCollection<MathDiagnostic> Diagnostics { get; }

    public bool Success => Root is not null && Diagnostics.Count == 0;
}
