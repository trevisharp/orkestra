using System.Collections.Generic;
using System.Text;

namespace Orkestra.Processings;

public class TextFragment
{
    private List<string> parts;
    private int listIndex = 0;
    private int charIndex = 0;

    public TextFragment(string value)
    {
        parts = new List<string>();

        foreach (var lines in value.Split('\n'))
        {
            parts.Add(lines);
            parts.Add("\n");
        }

        parts.RemoveAt(parts.Count - 1);
    }

    public static TextFragment operator +(TextFragment t, string s)
    {
        t.parts.Add(s);
        return t;
    }

    public static implicit operator TextFragment(string value)
    {
        TextFragment fragment = new TextFragment(value);
        return fragment;
    }
}