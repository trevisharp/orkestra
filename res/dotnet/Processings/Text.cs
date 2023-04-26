using System;
using System.IO;
using System.Linq;
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
        var newlineReplacedSource = source
            .Replace("\r\n", "\n") // For Windows
            .Replace("\r", "\n");  // For MAC version < X
        initFastList(newlineReplacedSource);
        this.pointerStack = new Stack<ProcessStep>();
        addStep(0, UnityType.All);
    }

    private int getStartLineIndex(ProcessStep step)
    {
        int i = step.Index - 1;
        while (i >= 0 && this.source[i].Character != '\n')
            i--;
        
        if (i == 0)
            return 0;
        
        return i + 1;
    }

    private int getEndLineIndex(ProcessStep step)
    {
        int i = step.Index;
        while (i < this.source.Count && this.source[i].Character != '\n')
            i++;
        
        if (i == this.source.Count)
            i--;
        
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

    private void appendAll(Data data)
    {
        this.source.Add(data);
    }

    private void appendLine(Data data)
    {
        var step = this.pointerStack.Peek();
        int end = getEndLineIndex(step);
        if (this.source[end].Character == '\n')
            this.source.Insert(data, end);
        else this.source.Insert(data, end + 1);
    }

    private void appendCharacter(Data data)
    {
        var step = this.pointerStack.Peek();
        this.source.Insert(data, step.Index + 1);
    }

    private void append(Data data)
    {
        var step = this.pointerStack.Peek();
        var type = step.Type;

        if (type == UnityType.All)
            appendAll(data);
        else if (type == UnityType.Line)
            appendLine(data);
        else if (type == UnityType.Character)
            appendCharacter(data);
    }

    private void prependAll(Data data)
    {
        this.source.Insert(data, 0);
    }

    private void prependLine(Data data)
    {
        var step = this.pointerStack.Peek();
        int index = getStartLineIndex(step);
        this.source.Insert(data, index);
    }

    private void prependCharacter(Data data)
    {
        var step = this.pointerStack.Peek();
        this.source.Insert(data, step.Index - 1);
    }

    private void prepend(Data data)
    {
        var step = this.pointerStack.Peek();
        var type = step.Type;

        if (type == UnityType.All)
            prependAll(data);
        else if (type == UnityType.Line)
            prependLine(data);
        else if (type == UnityType.Character)
            prependCharacter(data);
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

        if (step.Index == -1) // Non-initialized yet
        {
            updateStep(0, UnityType.Line);
            return true;
        }

        var end = getEndLineIndex(step);
        if (end + 1 >= this.source.Count)
            return false;
        
        updateStep(end + 1, UnityType.Line);
        return true;
    }

    public bool NextCharacter()
    {
        var step = this.pointerStack.Peek();

        if (step.Type == UnityType.All)
        {
            addStep(0, UnityType.Character);
            return true;
        }

        if (step.Type != UnityType.Character)
            throw new Exception("Invalid processing: Character processing need start in all processing.");

        if (step.Index == -1) // Non-initialized yet
        {
            updateStep(0, UnityType.Line);
            return true;
        }

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
            
            if (step.Index == source.Count) //End Of File
                return false;
            
            return true;
        }

        if (step.Type != UnityType.Character)
            throw new Exception("Invalid processing: Character processing need start in all or line processing.");

        int index = step.Index;
        if (index >= this.source.Count)
            return false;
        
        if (this.source[index].Character == '\n')
            return false;
        
        index++;
        if (index >= this.source.Count)
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

    public bool Is(char character)
    {
        var step = this.pointerStack.Peek();
        var index = step.Index;

        return
            index < this.source.Count && 
            this.source[index].Character == character;
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
            updateStep(step.Index - data.Length, UnityType.Character);
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
        updateStep(step.Index - data.Length - 1, UnityType.Line);
    }

    public void AppendNewline()
        => append(newline);

    public void AppendTab()
        => append(tab);

    public void PrependNewline()
        => prepend(newline);

    public void PrependTab()
        => prepend(tab);

    private void removeUnity()
    {
        var step = this.pointerStack.Peek();

        if (step.Type == UnityType.All)
        {
            this.source.Clear();
            return;
        }

        if (step.Type == UnityType.Line)
        {
            int start = getStartLineIndex(step);
            int end = getStartLineIndex(step);
            this.source.Remove(start, end - start + 1);
            return;
        }

        if (step.Type == UnityType.Character)
        {
            int index = step.Index;
            this.source.Remove(index, 1);
            int newIndex = index - 1;
            updateStep(newIndex, UnityType.Character);
            return;
        }
    }

    public void Replace(string str)
    {
        removeUnity();
        Append(str);
    }

    public void Replace(Token token)
    {
        removeUnity();
        Append(token);
    }

    public void Replace(Key baseKey)
    {
        removeUnity();
        Append(baseKey);
    }

    public void Discard()
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

    public bool Next()
    {
        var step = this.pointerStack.Peek();

        switch (step.Type)
        {
            case UnityType.All:
                this.pointerStack.Pop();
                addStep(-1, UnityType.All);
                return true;
            
            case UnityType.Line:
                return NextLine();
            
            case UnityType.Character:
                step = this.pointerStack.Pop();
                var parent = this.pointerStack.Peek();
                this.pointerStack.Push(step);

                if (parent.Type == UnityType.All)
                    return NextCharacter();
                else if (parent.Type == UnityType.Line)
                    return NextCharacterLine();
                
                return false;

            default:
                return false;    
        }
    }

    public void Skip()
    {
        var step = this.pointerStack.Peek();
        removeUnity();
        updateStep(step.Index - 1, step.Type);
    }

    public object[] ToSources()
    {
        List<object> data = new List<object>();
        StringBuilder sb = null;
        int index = 0;

        while (index < this.source.Count)
        {
            sb = new StringBuilder();
            while (this.source[index].Token is null)
            {
                sb.Append(this.source[index].Character);

                index++;
                if (index >= this.source.Count)
                    break;
            }
            data.Add(sb.ToString());
            if (index >= this.source.Count)
                break;

            while (this.source[index].Token is not null)
            {
                data.Add(this.source[index].Token);

                index++;
                if (index >= this.source.Count)
                    break;
            }
            if (index >= this.source.Count)
                break;
        }

        return data.ToArray();
    }

    public string FullText()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var data in this.source)
        {
            if (data.Token is null)
                sb.Append(data.Character);
            else
            {
                sb.Append(" ");
                sb.Append(data.Token);
                sb.Append(" ");
            }
        }
        return sb.ToString();
    }

    public override string ToString()
    {
        if (this.pointerStack.Count == 0 || this.pointerStack.Peek().Type == UnityType.All)
            return FullText();
        
        var step = this.pointerStack.Peek();
        var type = step.Type;
        var index = step.Index;

        if (step.Index == -1)
            return null;

        if (type == UnityType.Line)
        {
            StringBuilder sb = new StringBuilder();

            var start = getStartLineIndex(step);
            var end = getEndLineIndex(step);

            for (int i = start; i <= end; i++)
            {
                var data = this.source[i];
                if (data.Token is null)
                    sb.Append(data.Character);
                else
                {
                    sb.Append(" ");
                    sb.Append(data.Token);
                    sb.Append(" ");
                }
            }
            
            return sb.ToString();
        }

        if (type == UnityType.Character)
        {
            var data = this.source[index];
            if (data.Token is null)
                return data.Character.ToString();
            
            return data.Token.ToString();
        }

        return "";
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