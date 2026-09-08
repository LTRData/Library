using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LTRData.MathExpression;

#pragma warning disable CS1591

public sealed class MathDiagnostic
{
    public MathDiagnostic(string code, string message, SourceSpan span)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(code);
#else
        if (code is null) throw new ArgumentNullException(nameof(code));
#endif
        Code = code;
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(message);
#else
        if (message is null) throw new ArgumentNullException(nameof(message));
#endif
        Message = message;
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
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(sourceText);
#else
        if (sourceText is null) throw new ArgumentNullException(nameof(sourceText));
#endif
        SourceText = sourceText;
        Root = root;
        Diagnostics = new List<MathDiagnostic>(diagnostics).AsReadOnly();
    }

    public string SourceText { get; }

    public MathSyntax? Root { get; }

    public ReadOnlyCollection<MathDiagnostic> Diagnostics { get; }

    public bool Success => Root is not null && Diagnostics.Count == 0;
}
