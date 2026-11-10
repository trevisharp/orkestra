using System;
using System.Text;

namespace Orkestra;

/// <summary>
/// A table-driven LR parser. It is generic over the lexer and the tree builder, so with
/// <c>struct</c> implementations the JIT specializes the whole loop for each grammar.
/// </summary>
public static class LRParser
{
    /// <summary>Error code for characters no lexer rule matches.</summary>
    public const string InvalidCharacterCode = "ORK0001";

    /// <summary>Error code for a token the grammar does not allow at that point.</summary>
    public const string SyntaxErrorCode = "ORK0002";

    /// <summary>Error code reported when parsing stops after <see cref="MaxErrors"/> errors.</summary>
    public const string TooManyErrorsCode = "ORK0003";

    /// <summary>Parsing stops after this many syntax errors.</summary>
    public const int MaxErrors = 100;

    const int MaxExpectedInMessage = 8;

    /// <summary>
    /// Parses the tokens from <paramref name="lexer"/> and returns the value built for the start rule.
    /// </summary>
    /// <remarks>
    /// Syntax errors go to <paramref name="diagnostics"/>. After an error the parser skips tokens
    /// until one fits a state on its stack, so a single run reports many errors. The result is
    /// <c>default</c> when the input could not be accepted; when errors were reported but the
    /// parser still reached the end, it is a tree with the bad parts left out.
    /// </remarks>
    public static TNode? Parse<TLexer, TNode, TActions>(
        ref TLexer lexer,
        scoped ParseTables tables,
        ref TActions actions,
        DiagnosticBag diagnostics)
        where TLexer : ILexer, allows ref struct
        where TActions : IParseActions<TNode>, allows ref struct
    {
        ArgumentNullException.ThrowIfNull(diagnostics);

        int terminals = tables.TerminalCount;
        int nonterminals = tables.NonterminalCount;
        var action = tables.Action;
        var @goto = tables.Goto;

        var states = new ValueStack<int>(64);
        var values = new ValueStack<TNode>(64);
        try
        {
            states.Push(0);
            var token = lexer.Next();
            int errors = 0;
            // True from a syntax error until the next shift. While it is set, further
            // errors skip tokens silently, so one mistake is reported once.
            bool recovering = false;

            while (true)
            {
                int cell = action[states.Peek() * terminals + token.Kind];

                if (cell > 0)
                {
                    states.Push(cell - 1);
                    values.Push(actions.Shift(in token));
                    token = lexer.Next();
                    recovering = false;
                    continue;
                }

                if (cell < 0)
                {
                    int production = -cell - 1;
                    if (production == 0)
                        return values.Peek();

                    int length = tables.ProductionLength[production];
                    var node = actions.Reduce(production, values.Top(length));
                    values.Pop(length);
                    states.Pop(length);

                    int lhs = tables.ProductionLhs[production];
                    states.Push(@goto[states.Peek() * nonterminals + lhs]);
                    values.Push(node);
                    continue;
                }

                if (!recovering)
                {
                    ReportError(diagnostics, tables, states.Peek(), token);
                    if (++errors >= MaxErrors)
                    {
                        diagnostics.Error(TooManyErrorsCode,
                            $"Parsing stopped after {MaxErrors} errors.", token.Span);
                        return default;
                    }
                    recovering = true;
                }
                else
                {
                    if (token.IsEndOfFile)
                        return default;
                    token = lexer.Next();
                }

                while (!Recover(ref states, ref values, action, terminals, token.Kind))
                {
                    if (token.IsEndOfFile)
                        return default;
                    token = lexer.Next();
                }
            }
        }
        finally
        {
            states.Dispose();
            values.Dispose();
        }
    }

    /// <summary>
    /// Pops states until one has an action for <paramref name="kind"/>.
    /// Returns false, leaving the stacks unchanged, when no state on the stack accepts it.
    /// </summary>
    static bool Recover<TNode>(
        ref ValueStack<int> states, ref ValueStack<TNode> values,
        ReadOnlySpan<int> action, int terminals, int kind)
    {
        for (int depth = 0; depth < states.Count; depth++)
        {
            if (action[states.PeekAt(depth) * terminals + kind] == 0)
                continue;

            // states holds one more entry (the initial state) than values.
            states.Truncate(states.Count - depth);
            values.Truncate(states.Count - 1);
            return true;
        }
        return false;
    }

    static void ReportError(DiagnosticBag diagnostics, scoped ParseTables tables, int state, Token token)
    {
        if (token.Kind == Token.Error)
        {
            diagnostics.Error(InvalidCharacterCode, "Invalid character.", token.Span);
            return;
        }

        var message = new StringBuilder("Unexpected ")
            .Append(tables.TerminalNames[token.Kind])
            .Append('.');

        var row = tables.Action.Slice(state * tables.TerminalCount, tables.TerminalCount);
        int expected = 0;
        for (int kind = 0; kind < row.Length; kind++)
        {
            if (row[kind] == 0 || kind == Token.Error)
                continue;
            if (expected == MaxExpectedInMessage)
            {
                message.Append(", ...");
                expected = -1;
                break;
            }
            message.Append(expected == 0 ? " Expected " : ", ").Append(tables.TerminalNames[kind]);
            expected++;
        }
        if (expected > 0)
            message.Append('.');

        diagnostics.Error(SyntaxErrorCode, message.ToString(), token.Span);
    }
}
