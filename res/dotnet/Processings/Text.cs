using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Orkestra.Processings;

using LexicalAnalysis;
using InternalStructure;

/// <summary>
/// A Tree Pointer for Text elements
/// </summary>
public unsafe class Text : IEnumerable<Text>
{
    private Text parent = null;
    private FastList<Text> children = null;
    private Text current = null;
    private void* source = null;

    private Text(IEnumerable<string> lines)
    {
        throw new NotImplementedException();
    }

    private Text(string text)
    {
        throw new NotImplementedException();
    }

    private Text(Text parent, UnityType type)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Compare current unity with a expression
    /// </summary>
    /// <param name="str">A regular expression</param>
    /// <returns>Return true if the expression and text match</returns>
    public bool Is(string str)
    {
        throw new NotImplementedException();
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
    /// Reset processing.
    /// </summary>
    public void Reset()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Return a Text pointer to lines of current unity
    /// </summary>
    public Text Lines =>
        throw new NotImplementedException();

    /// <summary>
    /// Return a Text pointer to characters of current unity
    /// </summary>
    public Text Characters =>
        throw new NotImplementedException();

    public IEnumerator<Text> GetEnumerator()
    {
        throw new NotImplementedException();
    }

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

    public static implicit operator string(Text text)
        => text?.ToString() ?? string.Empty;

    public static implicit operator Text(string source)
        => new Text(source);
}