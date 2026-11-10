namespace Orkestra.Tests;

/// <summary>
/// Tests the driver with the grammar <c>E → E '+' n | n</c>, whose tables are written by hand
/// below in the same layout Symphony will generate.
/// </summary>
public class LRParserTests
{
    const int Plus = Token.FirstUserKind;
    const int Number = Token.FirstUserKind + 1;

    // States:
    //   0: S' → .E      E → .E + n   E → .n
    //   1: S' → E.      E → E. + n
    //   2: E → n.
    //   3: E → E + .n
    //   4: E → E + n.
    // Columns: end of file, error, '+', number.
    static ReadOnlySpan<int> Action =>
    [
        0,  0, 0, 3,   // 0: n → shift 2
        -1, 0, 4, 0,   // 1: eof → accept, '+' → shift 3
        -3, 0, -3, 0,  // 2: reduce E → n
        0,  0, 0, 5,   // 3: n → shift 4
        -2, 0, -2, 0,  // 4: reduce E → E + n
    ];

    static ReadOnlySpan<int> Goto => [1, 0, 0, 0, 0];
    static ReadOnlySpan<ushort> ProductionLhs => [0, 0, 0];
    static ReadOnlySpan<byte> ProductionLength => [1, 3, 1];
    static readonly string[] TerminalNames = ["end of file", "invalid character", "'+'", "number"];

    static ParseTables Tables => new(4, 1, Action, Goto, ProductionLhs, ProductionLength, TerminalNames);

    /// <summary>Digits are numbers, '+' is plus, spaces are skipped, anything else is an error.</summary>
    ref struct ExpressionLexer(ReadOnlySpan<char> text) : ILexer
    {
        readonly ReadOnlySpan<char> text = text;
        int position;

        public Token Next()
        {
            while (position < text.Length && text[position] == ' ')
                position++;
            if (position == text.Length)
                return new Token(Token.EndOfFile, new TextSpan(position, 0));

            int start = position++;
            int kind = text[start] switch
            {
                '+' => Plus,
                >= '0' and <= '9' => Number,
                _ => Token.Error,
            };
            return new Token(kind, new TextSpan(start, 1));
        }
    }

    /// <summary>Builds a fully parenthesized string, e.g. "((1+2)+3)".</summary>
    ref struct StringActions(ReadOnlySpan<char> text) : IParseActions<string>
    {
        readonly ReadOnlySpan<char> text = text;

        public string Shift(in Token token) => text.Slice(token.Span.Start, token.Span.Length).ToString();

        public string Reduce(int production, ReadOnlySpan<string> children) => production switch
        {
            1 => $"({children[0]}+{children[2]})",
            2 => children[0],
            _ => throw new InvalidOperationException(),
        };
    }

    /// <summary>Counts reductions; keeps everything on the stack as ints so nothing is allocated.</summary>
    struct CountingActions : IParseActions<int>
    {
        public int Reductions;
        public int Shift(in Token token) => 0;
        public int Reduce(int production, ReadOnlySpan<int> children) { Reductions++; return 0; }
    }

    static (string? Tree, DiagnosticBag Diagnostics) Parse(string input)
    {
        var lexer = new ExpressionLexer(input);
        var actions = new StringActions(input);
        var diagnostics = new DiagnosticBag();
        var tree = LRParser.Parse<ExpressionLexer, string, StringActions>(ref lexer, Tables, ref actions, diagnostics);
        return (tree, diagnostics);
    }

    [Theory]
    [InlineData("7", "7")]
    [InlineData("1+2", "(1+2)")]
    [InlineData("1 + 2 + 3", "((1+2)+3)")]
    public void Parses_valid_input(string input, string expected)
    {
        var (tree, diagnostics) = Parse(input);
        Assert.Empty(diagnostics);
        Assert.Equal(expected, tree);
    }

    [Fact]
    public void Empty_input_reports_what_was_expected()
    {
        var (tree, diagnostics) = Parse("");
        Assert.Null(tree);
        var error = Assert.Single(diagnostics);
        Assert.Equal(LRParser.SyntaxErrorCode, error.Code);
        Assert.Equal("Unexpected end of file. Expected number.", error.Message);
    }

    [Fact]
    public void Recovers_from_an_extra_token()
    {
        var (tree, diagnostics) = Parse("1++2");
        var error = Assert.Single(diagnostics);
        Assert.Equal("Unexpected '+'. Expected number.", error.Message);
        Assert.Equal(new TextSpan(2, 1), error.Span);
        Assert.Equal("(1+2)", tree);
    }

    [Fact]
    public void Reports_invalid_characters_from_the_lexer()
    {
        var (tree, diagnostics) = Parse("1+x");
        var error = Assert.Single(diagnostics);
        Assert.Equal(LRParser.InvalidCharacterCode, error.Code);
        Assert.Equal(new TextSpan(2, 1), error.Span);
        Assert.Equal("1", tree);
    }

    [Fact]
    public void Reports_separate_mistakes_separately()
    {
        var (_, diagnostics) = Parse("1++2++3");
        Assert.Equal(2, diagnostics.Count);
        Assert.Equal(new TextSpan(2, 1), diagnostics[0].Span);
        Assert.Equal(new TextSpan(5, 1), diagnostics[1].Span);
    }

    [ReleaseOnlyFact]
    public void Steady_state_parsing_does_not_allocate()
    {
        var input = "1" + string.Concat(Enumerable.Repeat("+2", 10_000));
        var diagnostics = new DiagnosticBag();

        for (int i = 0; i < 50; i++)
            RunOnce(input, diagnostics); // warm up the JIT (through tiering) and the array pool
        long before = GC.GetAllocatedBytesForCurrentThread();
        int reductions = RunOnce(input, diagnostics);
        long allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.False(diagnostics.HasErrors);
        Assert.Equal(10_001, reductions);
        Assert.Equal(0, allocated);

        static int RunOnce(string input, DiagnosticBag diagnostics)
        {
            var lexer = new ExpressionLexer(input);
            var actions = new CountingActions();
            LRParser.Parse<ExpressionLexer, int, CountingActions>(ref lexer, Tables, ref actions, diagnostics);
            return actions.Reductions;
        }
    }
}
