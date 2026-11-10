using System.Collections;
using System.Collections.Generic;

namespace Orkestra;

/// <summary>How serious a <see cref="Diagnostic"/> is.</summary>
public enum DiagnosticSeverity : byte
{
    /// <summary>Informational message.</summary>
    Info,
    /// <summary>Suspicious input that still compiles.</summary>
    Warning,
    /// <summary>Input that cannot be compiled.</summary>
    Error,
}

/// <summary>A message about a span of source text, such as a syntax error.</summary>
public readonly record struct Diagnostic(
    DiagnosticSeverity Severity,
    string Code,
    string Message,
    TextSpan Span)
{
    /// <summary>
    /// Formats the diagnostic as <c>path(line,col): error CODE: message</c>,
    /// the MSBuild format that terminals and VSCode turn into clickable links.
    /// </summary>
    public string Format(SourceText source)
    {
        var position = source.GetLinePosition(Span.Start);
        var severity = Severity switch
        {
            DiagnosticSeverity.Error => "error",
            DiagnosticSeverity.Warning => "warning",
            _ => "info",
        };
        return $"{source.Path}({position}): {severity} {Code}: {Message}";
    }
}

/// <summary>Collects the diagnostics reported while compiling one source.</summary>
public sealed class DiagnosticBag : IReadOnlyList<Diagnostic>
{
    readonly List<Diagnostic> items = [];

    /// <summary>The number of errors reported so far.</summary>
    public int ErrorCount { get; private set; }

    /// <summary>True when at least one error was reported.</summary>
    public bool HasErrors => ErrorCount > 0;

    /// <inheritdoc/>
    public int Count => items.Count;

    /// <inheritdoc/>
    public Diagnostic this[int index] => items[index];

    /// <summary>Adds a diagnostic.</summary>
    public void Add(Diagnostic diagnostic)
    {
        items.Add(diagnostic);
        if (diagnostic.Severity == DiagnosticSeverity.Error)
            ErrorCount++;
    }

    /// <summary>Adds an error.</summary>
    public void Error(string code, string message, TextSpan span)
        => Add(new Diagnostic(DiagnosticSeverity.Error, code, message, span));

    /// <summary>Adds a warning.</summary>
    public void Warning(string code, string message, TextSpan span)
        => Add(new Diagnostic(DiagnosticSeverity.Warning, code, message, span));

    /// <inheritdoc/>
    public IEnumerator<Diagnostic> GetEnumerator() => items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
