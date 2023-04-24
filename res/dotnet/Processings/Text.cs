using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Orkestra.Processings;

using LexicalAnalysis;
using InternalStructure;

/// <summary>
/// A Tree Pointer for Text elements
/// </summary>
public class Text
{
    private struct ProcessStep
    {
        public int Index { get; set; }
        public UnityType Type { get; set; }
    }

    private struct Data
    {
        public char Character { get; set; }
        public Token Token { get; set; }
    }

    private readonly Data newline = new Data()
    {
        Character = '\n',
        Token = null
    };

    private readonly Data tab = new Data()
    {
        Character = '\t',
        Token = null
    };

    private Stack<ProcessStep> pointerStack;
    private FastList<Data> source;

    private Text(string source)
    {
        initFastList(source);
        this.pointerStack = new Stack<ProcessStep>();
        addStep(-1, UnityType.All);
    }

    private int getStartLineIndex(ProcessStep step)
    {
        int i = step.Index;
        while (i >= 0 && this.source[i].Character != '\n')
            i--;
        
        return i + 1;
    }

    private int getEndLineIndex(ProcessStep step)
    {
        int i = step.Index;
        while (i < this.source.Count && this.source[i].Character != '\n')
            i++;
        
        return i;
    }

    private void initFastList(string text)
    {
        this.source = new FastList<Data>();

        char[] characters = text.ToCharArray();
        Data[] data = new Data[characters.Length];
        for (int i = 0; i < data.Length; i++)
            data[i] = new Data()
            {
                Character = characters[i],
                Token = null
            };
        
        this.source.AddRange(data);
    }
    
    private void addStep(int index, UnityType type)
    {
        this.pointerStack.Push(new ProcessStep
        {
            Index = index,
            Type = type,
        });
    }

    private void updateStep(int index, UnityType type)
    {
        var step = this.pointerStack.Pop();
        step.Index = index;
        step.Type = type;
        this.pointerStack.Push(step);
    }

    public bool NextLine()
    {
        var step = this.pointerStack.Peek();

        if (step.Type == UnityType.All)
        {
            addStep(0, UnityType.Line);
            return true;
        }

        if (step.Type != UnityType.Line)
            throw new Exception("Invalid processing: Line processing need start in all processing.");

        var end = getEndLineIndex(step);

        if (end == this.source.Count)
            return false;
        
        updateStep(end + 1, UnityType.Line);
        return true;
    }

    public bool NextCharacter()
    {
        var step = this.pointerStack.Peek();

        if (step.Type == UnityType.All || step.Type == UnityType.Line)
        {
            addStep(0, UnityType.Character);
            return true;
        }

        if (step.Type != UnityType.Character)
            throw new Exception("Invalid processing: Character processing need start in all or line processing.");

        int index = step.Index;
        index++;
        if (index >= source.Count)
            return false;
        
        updateStep(index, UnityType.Character);
        return true;
    }

    public bool NextCharacterLine()
    {
        var step = this.pointerStack.Peek();

        if (step.Type == UnityType.Line)
        {
            addStep(step.Index, UnityType.Character);
            return true;
        }

        if (step.Type != UnityType.Character)
            throw new Exception("Invalid processing: Character processing need start in all or line processing.");

        int index = step.Index;
        index++;
        if (index >= source.Count)
            return false;
        
        if (source[index].Character == '\n')
            return false;

        updateStep(index, UnityType.Character);
        return true;
    }

    public void PopProcessing()
        => this.pointerStack.Pop();

    public bool Is(string comparation)
    {
        var step = this.pointerStack.Peek();
        var index = step.Index;
        int i = 0;

        while (i < comparation.Length && index < this.source.Count)
        {
            if (this.source[index++].Character == comparation[i++])
                continue;
            
            return false;
        }

        if (index == this.source.Count && i < comparation.Length)
            return false;

        return true;
    }

    private void append(Data data)
    {
        var step = this.pointerStack.Peek();
        var type = step.Type;

        if (type == UnityType.All)
        {
            this.source.Add(data);
            return;
        }

        if (type == UnityType.Character)
        {
            this.source.Insert(data, step.Index);
            return;
        }
        
        int i = step.Index;
        while (i < this.source.Count && this.source[i].Character != '\n')
            i++;
        
        if (i >= this.source.Count)
        {
            this.source.Add(newline);
            this.source.Add(data);
            return;
        }
        
        this.source.Insert(data, i);
        this.source.Add(newline);
    }

    private void prepend(Data data)
    {
        var step = this.pointerStack.Peek();
        var type = step.Type;

        if (type == UnityType.All)
        {
            this.source.Insert(data, 0);
            return;
        }

        if (type == UnityType.Character)
        {
            this.source.Insert(data, step.Index - 1);
            return;
        }
        
        int i = step.Index;
        while (i >= 0 && this.source[i].Character != '\n')
            i--;
        
        this.source.Insert(data, i);
        this.source.Insert(newline, i);
    }

    public void Append(Token token)
    {
        var data = new Data()
        {
            Character = '\0',
            Token = token
        };
        append(data);
    }

    public void Append(Key baseKey)
    {
        // TODO: get Index
        Token token = new Token(baseKey, null, 0);
        Append(token);
    }

    public void Append(string str)
    {
        var step = this.pointerStack.Peek();
        var type = step.Type;
        var data = new Data[str.Length];
        int n = 0;
        foreach (var c in str)
            data[n] = new Data()
            {
                Character = c,
                Token = null
            };

        if (type == UnityType.All)
        {
            this.source.AddRange(data);
            return;
        }

        if (type == UnityType.Character)
        {
            this.source.Insert(data, step.Index);
            return;
        }
        
        int i = step.Index;
        while (i < this.source.Count && this.source[i].Character != '\n')
            i++;
        
        if (i < this.source.Count)
        {
            this.source.Add(newline);
            this.source.AddRange(data);
            return;
        }
        
        this.source.Insert(data, i);
        this.source.Insert(newline, i);
    }

    public void Prepend(Token token)
    {
        var data = new Data()
        {
            Character = '\0',
            Token = token
        };
        prepend(data);
    }

    public void Prepend(Key baseKey)
    {
        // TODO: get Index
        Token token = new Token(baseKey, null, 0);
        Prepend(token);
    }

    public void Prepend(string str)
    {
        var step = this.pointerStack.Peek();
        var type = step.Type;
        var data = new Data[str.Length];
        int n = 0;
        foreach (var c in str)
            data[n] = new Data()
            {
                Character = c,
                Token = null
            };

        if (type == UnityType.All)
        {
            this.source.AddRange(data);
            return;
        }

        if (type == UnityType.Character)
        {
            this.source.Insert(data, step.Index);
            return;
        }
        
        int i = step.Index;
        while (i < this.source.Count && this.source[i].Character != '\n')
            i++;
        
        if (i < this.source.Count)
        {
            this.source.Add(newline);
            this.source.AddRange(data);
            return;
        }
        
        this.source.Insert(data, i);
        this.source.Insert(newline, i);
    }

    public void AppendNewline()
        => append(newline);

    public void AppendTab()
        => append(tab);

    public void PrependNewline()
        => prepend(newline);

    public void PrependTab()
        => prepend(tab);

    public void Replace(string str)
    {

    }

    public void Replace(Token token)
    {

    }

    public void Replace(Key baseKey)
    {
        
    }

    public void Break()
    {
        var step = this.pointerStack.Pop();
        var parent = this.pointerStack.Peek();

        if (parent.Type == UnityType.All && step.Type == UnityType.Line)
        {
            int start = getStartLineIndex(step);
            this.source.Remove(start, -1);
            this.pointerStack.Push(step);
            return;
        }
        else if (parent.Type == UnityType.All && step.Type == UnityType.Character)
        {
            int start = step.Index;
            this.source.Remove(start, -1);
            this.pointerStack.Push(step);
            return;
        }
        else if (parent.Type == UnityType.Line && step.Type == UnityType.Character)
        {
            int start = step.Index;
            int end = getEndLineIndex(step);
            this.source.Remove(start, end - start);
            this.pointerStack.Push(step);
            return;
        }
        
        throw new Exception("Inconsistence in stack pointers.");
    }

    public void Complete()
    {

    }

    public void Continue()
    {
        
    }

    public void Discard()
    {

    }

    public object[] ToSources()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var data in this.source)
        {
            if (data.Token is null)
                sb.Append(data.Character);
            else sb.Append(data.Token);
        }
        return sb.ToString();
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