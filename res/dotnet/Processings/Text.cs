using System;
using System.Linq;
using System.Collections.Generic;

namespace Orkestra.Processings;

using InternalStructure;

public class Text
{
    private FastInsertionList<CodeUnity> list = null;
    private Text parent = null;
    private UnityType type = UnityType.All;
    private IEnumerable<CodeUnity> data = null;

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
            list.Add(new CodeUnity()
            {
                SourceLine = count,
                Value = '\n'
            });
        }
        data = list;
    }

    private Text(
        Text parent, 
        UnityType type,
        IEnumerable<CodeUnity> data)
    {
        this.list = parent.list;
        this.data = data;
        this.parent = parent;
        this.type = type;
    }

    public UnityType Type => type;

    public Text Parent => parent;

    public Text All => this;

    public IEnumerable<Text> Lines
    {
        get
        {
            if (this.type < UnityType.All)
            {
                yield return new Text(
                    this, UnityType.Line, data);
                yield break;
            }

            var all = this.All;
            var it = all.list.GetEnumerator();

            foreach (var bk in list.BreakPoints)
            {   
                Text text = new Text(
                    this, UnityType.Line, bk);
                yield return text;
            }
        }
    }

    public IEnumerable<Text> Characters
    {
        get
        {
            CodeUnity[] vec = new CodeUnity[1];
            foreach (var x in data)
            {
                vec[0] = x;
                yield return new Text(
                    this, UnityType.Character,
                    vec);
            }
        }
    }

    public bool Is(string str)
    {
        throw new NotImplementedException();
    }
}