using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Orkestra.Processings;

using LexicalAnalysis;
using InternalStructure;
using System.Text.RegularExpressions;

/// <summary>
/// A Tree Pointer for Text elements
/// </summary>
public unsafe class Text : IEnumerable<Text>
{
    private UnityType crrUnity = 0;
    private int crrIndex = -1;
    private Text crrChild = null;

    private Text parent = null;
    private FastList<Text> lines = null;
    
    private Token tokenSource = null;

    private UnityType type;
    private string stringSource = null;
    private int sourceStart = -1;
    private int sourceEnd = -1;

    private Text(string source)
    {
        this.type = UnityType.All;
        this.stringSource = source;
        this.sourceStart = 0;
        this.sourceEnd = source.Length;
    }

    private Text(Text parent, UnityType newUnity, int start, int end)
    {
        this.type = newUnity;
        this.parent = parent;
        this.stringSource = parent.stringSource;
        this.sourceStart = start;
        this.sourceEnd = end;
    }

    /// <summary>
    /// Compare current unity with a expression
    /// </summary>
    /// <param name="str">A regular expression</param>
    /// <returns>Return true if the expression and text match</returns>
    public bool Is(string str)
    {
        Regex regex = new Regex(str);
        var match = regex.Match(
            this.stringSource, this.sourceStart
        );
        return match.Success;
    }

    /// <summary>
    /// End a processing iteration removing the units.
    /// </summary>
    public void Break()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Go to next unity of processing keeping the current unity.
    /// </summary>
    public void Continue()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Go to next unity of processing removing the current unity.
    /// </summary>
    public void Jump()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// End a processing iteration keeping the units.
    /// </summary>
    public void Complete()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Replaces current unity with a text
    /// </summary>
    public void Replace(string text)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Append current unity with a text
    /// </summary>
    public void Append(string text)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Preprend current unity with a text
    /// </summary>
    public void Prepend(string text)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Replace current unity with a token
    /// </summary>
    public void Replace(Token token)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Append current unity with a token
    /// </summary>
    public void Append(Token token)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Preprend current unity with a token
    /// </summary>
    public void Prepend(Token token)
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Return pointers for lines
    /// </summary>
    public IEnumerable<Text> Lines
    {
        get
        {
            if (this.stringSource == null)
                yield break;
            
            if (this.lines != null)
            {
                foreach (var child in this.lines)
                    yield return child;
                
                yield break;
            }

            this.lines = new FastList<Text>();
            
            int j = sourceStart;
            for (int i = sourceStart; i < sourceEnd; i++)
            {
                if (this.stringSource[i] != '\n')
                    continue;

                Text line = new Text(this,
                    UnityType.Line, j, i
                );
                this.lines.Add(line);

                this.crrChild = line;
                this.crrUnity = UnityType.Line; 
                j = i;

                yield return line;
            }
        }
    }

    /// <summary>
    /// Return a Text pointer to characters of current unity
    /// </summary>
    public IEnumerable<Text> Characters
    {
        get
        {
            Text temp = new Text(this, 
                UnityType.Character, this.sourceStart, this.sourceStart + 1
            );

            for (int i = this.sourceStart; i < this.sourceEnd; i++)
            {
                temp.sourceStart = i;
                temp.sourceEnd = i + 1;
                yield return temp;
            }
        }
    }

    public IEnumerator<Text> GetEnumerator()
        => this.type switch
        {
            UnityType.All => Lines.GetEnumerator(),
            _ => Characters.GetEnumerator()
        };

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private void next()
    {
        throw new NotImplementedException();
    }

    private void prev()
    {
        throw new NotImplementedException();
    }

    private void reset()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Return a array of texts splited by possible
    /// inserted tokens.
    /// </summary>
    /// <returns>A object array of string and Token objects</returns>
    public object[] ToSources()
    {
        throw new NotImplementedException();
    }

    public static Text FromFile(string path)
    {
        StreamReader reader = new StreamReader(path);
        string source = reader.ReadToEnd();
        reader.Close();

        Text text = new Text(source);
        return text;
    }

    public static implicit operator string(Text text)
        => text?.ToString() ?? string.Empty;

    public static implicit operator Text(string source)
        => new Text(source);
}