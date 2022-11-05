using System;
using System.Collections.Generic;

namespace Orkestra.Processings;

using InternalStructure;

public class Text
{
    private FastInsertionList<CodeUnity> list = null;
    private Text parent = null;
    private UnityType type = UnityType.All;
    private int pos = -1;

    private Text(string text)
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

    private Text(
        Text parent, 
        UnityType type,
        int pos)
    {
        this.list = parent.list;
        this.pos = pos;
        this.parent = parent;
        this.type = type;
    }

    public UnityType Type => type;

    public Text Parent => parent;

    public Text All 
        => this;

    public IEnumerable<Text> Lines
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public IEnumerable<Text> Characters
    {
        get
        {
            throw new NotImplementedException();
        }
    }
}