using System;

namespace Orkestra;

/// <summary>
/// The LR tables for one grammar, as Symphony generates them. All data lives in
/// <see cref="ReadOnlySpan{T}"/> properties compiled into the binary, so loading them costs nothing.
/// </summary>
/// <remarks>
/// <para><b>Terminals</b> are token kinds (<see cref="Token"/>); index 0 is end of file and 1 is error.</para>
/// <para><b>Action</b> is a <c>states × terminals</c> table. Each cell is one of:
/// <c>0</c> for a syntax error, <see cref="Shift"/>(target state), or <see cref="Reduce"/>(production).</para>
/// <para><b>Goto</b> is a <c>states × nonterminals</c> table with the state to go to after a reduction.</para>
/// <para><b>Production 0</b> is always the augmented rule <c>S' → Start</c>; reducing it means the input is accepted.</para>
/// </remarks>
public readonly ref struct ParseTables
{
    /// <summary>Creates the tables and checks that their sizes agree.</summary>
    public ParseTables(
        int terminalCount,
        int nonterminalCount,
        ReadOnlySpan<int> action,
        ReadOnlySpan<int> @goto,
        ReadOnlySpan<ushort> productionLhs,
        ReadOnlySpan<byte> productionLength,
        ReadOnlySpan<string> terminalNames)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(terminalCount, Token.FirstUserKind);
        ArgumentOutOfRangeException.ThrowIfNegative(nonterminalCount);
        if (action.Length == 0 || action.Length % terminalCount != 0)
            throw new ArgumentException("Action table size must be a multiple of the terminal count.", nameof(action));
        int states = action.Length / terminalCount;
        if (@goto.Length != states * nonterminalCount)
            throw new ArgumentException("Goto table must have one row per state.", nameof(@goto));
        if (productionLhs.Length == 0 || productionLhs.Length != productionLength.Length)
            throw new ArgumentException("Production tables must be non-empty and the same length.", nameof(productionLhs));
        if (productionLength[0] != 1)
            throw new ArgumentException("Production 0 must be the augmented start rule S' -> Start.", nameof(productionLength));
        if (terminalNames.Length != terminalCount)
            throw new ArgumentException("There must be one name per terminal.", nameof(terminalNames));

        TerminalCount = terminalCount;
        NonterminalCount = nonterminalCount;
        StateCount = states;
        Action = action;
        Goto = @goto;
        ProductionLhs = productionLhs;
        ProductionLength = productionLength;
        TerminalNames = terminalNames;
    }

    /// <summary>The number of token kinds, including end of file and error.</summary>
    public int TerminalCount { get; }

    /// <summary>The number of nonterminals (rules).</summary>
    public int NonterminalCount { get; }

    /// <summary>The number of parser states.</summary>
    public int StateCount { get; }

    /// <summary>The action table, <c>states × terminals</c>.</summary>
    public ReadOnlySpan<int> Action { get; }

    /// <summary>The goto table, <c>states × nonterminals</c>.</summary>
    public ReadOnlySpan<int> Goto { get; }

    /// <summary>The nonterminal each production reduces to.</summary>
    public ReadOnlySpan<ushort> ProductionLhs { get; }

    /// <summary>The number of symbols on the right side of each production.</summary>
    public ReadOnlySpan<byte> ProductionLength { get; }

    /// <summary>Display names of the terminals, used in error messages.</summary>
    public ReadOnlySpan<string> TerminalNames { get; }

    /// <summary>Encodes "shift and go to <paramref name="state"/>" as an action cell.</summary>
    public static int Shift(int state) => state + 1;

    /// <summary>Encodes "reduce by <paramref name="production"/>" as an action cell.</summary>
    public static int Reduce(int production) => -(production + 1);

    /// <summary>The action cell meaning "accept the input".</summary>
    public const int Accept = -1;
}
