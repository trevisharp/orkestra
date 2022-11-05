using System;
using System.Collections.Generic;

namespace Orkestra.Processings;

using InternalStructure;

public class TextFragment
{
    private FastInsertionList<CodeUnity> list = null;
    private TextFragment parent = null;
    private UnityType type = UnityType.All;
    private int pos = -1;

    private TextFragment(string text)
    {
        this.list = new FastInsertionList<CodeUnity>();
        int count = 0;
        foreach (var line in text.Split('\n'))
        {
            count++;
            foreach (var character in line)
            {
                list.Add(new CodeUnity()
                {
                    SourceLine = count,
                    Value = character
                });
            }
        }
    }

    private TextFragment(
        TextFragment parent, 
        UnityType type,
        int pos)
    {
        this.list = parent.list;
        this.pos = pos;
        this.parent = parent;
        this.type = type;
    }

    public UnityType Type => type;

    public TextFragment Parent => parent;

    public TextFragment All 
        => this;

    public IEnumerable<TextFragment> Lines
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public IEnumerable<TextFragment> Characters
    {
        get
        {
            throw new NotImplementedException();
        }
    }
}