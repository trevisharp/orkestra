using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Orkestra.Processings;

using InternalStructure;
using LexicalAnalysis;

public class Text : IEnumerable<Text>
{
    private IEnumerable<Line> originalLines;
    private Text parent = null;
    private UnityType type = UnityType.All;

    public Text(IEnumerable<string> lines)
    {
        this.originalLines = lines
            .Select((ln, index) => new Line()
            {
                Code = ln,
                Token = null,
                Number = index,
                EndLine = true
            });
    }

    public bool Is(string str)
    {
        throw new NotImplementedException();
    }

    public void Break()
    {
        throw new NotImplementedException();
    }

    public void Continue()
    {
        throw new NotImplementedException();
    }

    public void Jump()
    {
        throw new NotImplementedException();
    }

    public void Complete()
    {
        throw new NotImplementedException();
    }

    public void Replace(string text)
    {
        throw new NotImplementedException();
    }

    public void Append(string text)
    {
        throw new NotImplementedException();
    }

    public void Prepend(string text)
    {
        throw new NotImplementedException();
    }

    public void Replace(Token token)
    {
        throw new NotImplementedException();
    }

    public void Append(Token token)
    {
        throw new NotImplementedException();
    }

    public void Prepend(Token token)
    {
        throw new NotImplementedException();
    }

    public Text Lines
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public Text Characters
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public IEnumerator<Text> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
    
    public static Text FromFile(string path)
    {
        Text text = new Text(open());
        return text;

        IEnumerable<string> open()
        {
            StreamReader reader = new StreamReader(path);

            while (!reader.EndOfStream)
                yield return reader.ReadLine();

            reader.Close();
        }
    }
}