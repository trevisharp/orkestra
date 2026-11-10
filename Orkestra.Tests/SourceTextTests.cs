namespace Orkestra.Tests;

public class SourceTextTests
{
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(2, 0, 2)]
    [InlineData(3, 0, 3)]  // the '\n' itself is the last column of its line
    [InlineData(4, 1, 0)]
    [InlineData(6, 1, 2)]  // the '\r' of a "\r\n" pair
    [InlineData(8, 2, 0)]
    [InlineData(9, 2, 1)]  // end of text
    public void GetLinePosition_handles_lf_and_crlf(int offset, int line, int column)
    {
        var text = new SourceText("abc\nde\r\nf");
        Assert.Equal(new LinePosition(line, column), text.GetLinePosition(offset));
    }

    [Fact]
    public void Empty_text_has_one_line()
    {
        var text = new SourceText("");
        Assert.Equal(1, text.LineCount);
        Assert.Equal(new LinePosition(0, 0), text.GetLinePosition(0));
    }

    [Fact]
    public void GetLineSpan_excludes_line_breaks()
    {
        var text = new SourceText("abc\nde\r\nf");
        Assert.Equal("abc", text.ToString(text.GetLineSpan(0)));
        Assert.Equal("de", text.ToString(text.GetLineSpan(1)));
        Assert.Equal("f", text.ToString(text.GetLineSpan(2)));
    }

    [Fact]
    public void Diagnostic_format_is_one_based_msbuild_style()
    {
        var text = new SourceText("ab\ncd", "file.sy");
        var diagnostic = new Diagnostic(DiagnosticSeverity.Error, "ORK0002", "Bad.", new TextSpan(4, 1));
        Assert.Equal("file.sy(2,2): error ORK0002: Bad.", diagnostic.Format(text));
    }
}
