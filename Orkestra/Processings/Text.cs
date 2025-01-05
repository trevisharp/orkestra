/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2024
 */
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Orkestra.Processings;

using SyntacticAnalysis;
using InternalStructure;

/// <summary>
/// A Tree Pointer for Text elements
/// </summary>
public class Text
{
    private string sourceFile;
    private FastList<Data> data;
    private Stack<ProcessStep> pointerStack;
    private readonly Data newline = new('\n');
    private readonly Data tab = new('\t');

    public string SourceFile => sourceFile;

    public Text(string source, string file)
    {
        this.data = [];
        this.sourceFile = file;
        InitFastList(
            source.Replace("\r", "")
        );
        this.pointerStack = new Stack<ProcessStep>();
        AddStep(0, UnityType.All, false);
    }
    
    /// <summary>
    /// Start to process lines in Text data.
    /// </summary>
    public void ProcessLines()
    {
        var step = pointerStack.Peek();

        if (step.Type == UnityType.All)
        {
            int startLine = GetStartLineIndex(step);
            AddStep(startLine, UnityType.Line, false);
            return;
        }
        
        throw new Exception("Invalid processing: Line processing need start in all processing.");
    }

    /// <summary>
    /// Start to process characters in Text data.
    /// </summary>
    public void ProcessCharacters()
    {
        var step = pointerStack.Peek();

        if (step.Type == UnityType.All || step.Type == UnityType.Line)
        {
            AddStep(step.Index, UnityType.Character, false);
            return;
        }
        
        throw new Exception("Invalid processing: Character processing need start in all or line processing.");
    }

    /// <summary>
    /// Move to the next unity in the processing. Return true if has next unity.
    /// Recommended use:
    /// while (text.Next()) { ... }
    /// </summary>
    public bool Next()
        => pointerStack.Peek().Type switch
        {
            UnityType.Character => GoToNextCharacter(),
            UnityType.Line => GoToNextLine(),
            _ => false
        };

    /// <summary>
    /// Compare the current position of text with another.
    /// </summary>
    public bool Is(string comparation)
    {
        var step = this.pointerStack.Peek();
        var index = step.Index;
        int i = 0;

        while (i < comparation.Length && index < this.data.Count)
        {
            if (this.data[index++].Character == comparation[i++])
                continue;
            
            return false;
        }

        if (index == this.data.Count && i < comparation.Length)
            return false;

        return true;
    }

    /// <summary>
    /// Compare the current position of text with a character.
    /// </summary>
    public bool Is(char character)
    {
        var step = this.pointerStack.Peek();
        var index = step.Index;

        return
            index < this.data.Count && 
            this.data[index].Character == character;
    }
    
    /// <summary>
    /// Append a Token in current position.
    /// </summary>
    public void Append(Token token)
    {
        var step = this.pointerStack.Peek();
        var data = this.data[step.Index];
        var newData = new Data('\0', token, data.Line);
        Append(newData);
    }

    /// <summary>
    /// Append a Token based on a Key.
    /// </summary>
    public void Append(Key baseKey)
    {
        var step = this.pointerStack.Peek();
        var data = this.data[step.Index];
        Token token = new Token(baseKey, null, sourceFile, data.Line);
        Append(token);
    }

    /// <summary>
    /// Append a String in current position.
    /// </summary>
    public void Append(string str)
    {
        var step = this.pointerStack.Peek();
        var type = step.Type;
        var data = new Data[str.Length];
        int n = 0;
        foreach (var c in str)
            data[n] = new Data(c, null);

        if (type == UnityType.All)
        {
            this.data.AddRange(data);
            return;
        }

        if (type == UnityType.Character)
        {
            this.data.Insert(data, step.Index);
            return;
        }
        
        int i = step.Index;
        while (i < this.data.Count && this.data[i].Character != '\n')
            i++;
        
        if (i < this.data.Count)
        {
            this.data.Add(newline);
            this.data.AddRange(data);
            return;
        }
        
        this.data.Insert(data, i);
    }
    
    /// <summary>
    /// Prepend a Token in current position.
    /// </summary>
    public void Prepend(Token token)
    {
        var data = new Data('\0', token);
        Prepend(data);
    }

    /// <summary>
    /// Prepend a Token in current position.
    /// </summary>
    public void Prepend(Key baseKey)
    {
        // TODO: get Index
        var token = new Token(baseKey, null, sourceFile, 0);
        Prepend(token);
    }

    /// <summary>
    /// Preprend a string in current position.
    /// </summary>
    public void Prepend(string str)
    {
        var step = this.pointerStack.Peek();
        var type = step.Type;
        var data = new Data[str.Length];
        int n = 0;
        foreach (var c in str)
            data[n] = new Data(c, null);

        if (type == UnityType.All)
        {
            this.data.AddRange(data);
            return;
        }

        if (type == UnityType.Character)
        {
            this.data.Insert(data, step.Index);
            UpdateStep(step.Index - data.Length, UnityType.Character, step.Started);
            return;
        }
        
        int i = step.Index;
        while (i < this.data.Count && this.data[i].Character != '\n')
            i++;
        
        if (i < this.data.Count)
        {
            this.data.Add(newline);
            this.data.AddRange(data);
            return;
        }
        
        this.data.Insert(data, i);
        this.data.Insert(newline, i);
        UpdateStep(step.Index - data.Length - 1, UnityType.Line, step.Started);
    }

    public void AppendNewline()
        => Append(newline);

    public void AppendTab()
        => Append(tab);

    public void PrependNewline()
        => Prepend(newline);

    public void PrependTab()
        => Prepend(tab);
    
    /// <summary>
    /// Replace the current unity by a String.
    /// </summary>
    public void Replace(string str)
    {
        RemoveUnity();
        Append(str);
    }

    /// <summary>
    /// Replace the current unity by a Token.
    /// </summary>
    public void Replace(Token token)
    {
        RemoveUnity();
        Append(token);
    }

    /// <summary>
    /// Replace the current unity by a Key.
    /// </summary>
    public void Replace(Key baseKey)
    {
        RemoveUnity();
        Append(baseKey);
    }

    /// <summary>
    /// Discard the following unities from final result.
    /// </summary>
    public void Discard()
    {
        var step = PopStep()!;
        var parent = PopStep()!;

        if (parent.Type == UnityType.All && step.Type == UnityType.Line)
        {
            int start = GetStartLineIndex(step);
            this.data.Remove(start, -1);
            AddStep(parent);
            AddStep(step);
            return;
        }
        
        if (parent.Type == UnityType.All && step.Type == UnityType.Character)
        {
            int start = step.Index;
            this.data.Remove(start, -1);
            AddStep(parent);
            AddStep(step);
            return;
        }
        
        if (parent.Type == UnityType.Line && step.Type == UnityType.Character)
        {
            int start = step.Index;
            int end = GetEndLineIndex(step);
            this.data.Remove(start, end - start);
            AddStep(parent);
            AddStep(step);
            return;
        }
        
        throw new Exception("Inconsistence in stack pointers.");
    }

    //TODO: Update to new API; Improve code quality
    public bool Continue()
    {
        var step = this.pointerStack.Peek();

        switch (step.Type)
        {
            case UnityType.All:
                this.pointerStack.Pop();
                AddStep(-1, UnityType.All, true);
                return true;
            
            case UnityType.Line:
                return GoToNextLine();
            
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

    //TODO: Update to new API; Improve code quality
    public void Skip()
    {
        var step = this.pointerStack.Peek();
        RemoveUnity();
        UpdateStep(step.Index - 1, step.Type, step.Started);
    }

    public object[] ToSources()
    {
        List<object> data = new List<object>();
        int index = 0;
        int crrLine = 0;

        while (index < this.data.Count)
        {
            StringBuilder sb = new StringBuilder();
            var crr = this.data[index];
            while (crr.Token is null)
            {
                if (crr.Character == '\n')
                {
                    index++;
                    if (index >= this.data.Count)
                        break;
                    crr = this.data[index];
                    break;
                }

                sb.Append(crr.Character);
                crrLine = crr.Line;

                index++;
                if (index >= this.data.Count)
                    break;
                crr = this.data[index];
            }

            data.Add(new Line(crrLine, sb.ToString()));
            if (index >= this.data.Count)
                break;

            while (crr.Token is not null)
            {
                data.Add(crr.Token);

                index++;
                if (index >= this.data.Count)
                    break;
                crr = this.data[index];
            }
            if (index >= this.data.Count)
                break;
        }

        return data.ToArray();
    }

    public string FullText()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var data in this.data)
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
            return "";

        if (type == UnityType.Line)
        {
            StringBuilder sb = new StringBuilder();

            var start = GetStartLineIndex(step);
            var end = GetEndLineIndex(step);

            for (int i = start; i <= end; i++)
            {
                var data = this.data[i];
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
            var data = this.data[index];
            if (data.Token is null)
                return data.Character.ToString();
            
            return data.Token.ToString();
        }

        return "";
    }

    private void InitFastList(string text)
    {
        char[] characters = text.ToCharArray();
        int line = 1;
        Data[] data = new Data[characters.Length];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = new Data(characters[i], null, line);
            if (characters[i] == '\n')
                line++;
        }
        
        this.data.AddRange(data);
    }

    private void AddStep(ProcessStep step)
        => pointerStack.Push(step);

    private void AddStep(int index, UnityType type, bool started)
        => pointerStack.Push(new(index, type, started));

    private ProcessStep? PopStep() => 
        pointerStack.Count == 0 ? 
            null : 
            pointerStack.Pop();

    private void UpdateStep(int index, UnityType type, bool started)
    {
        PopStep();
        AddStep(index, type, started);
    }

    private int GetStartLineIndex(ProcessStep step)
    {
        int i = step.Index - 1;
        while (i >= 0 && data[i].Character != '\n')
            i--;
        
        if (i == 0)
            return 0;
        
        return i + 1;
    }

    private int GetEndLineIndex(ProcessStep step)
    {
        int i = step.Index;
        while (i < data.Count && data[i].Character != '\n')
            i++;
        
        if (i == this.data.Count)
            i--;
        
        return i;
    }

    private bool GoToNextLine()
    {
        var step = pointerStack.Peek();
        if (!step.Started)
        {
            UpdateStep(step.Index, step.Type, true);
            return true;
        }

        var end = GetEndLineIndex(step);
        if (end + 1 >= this.data.Count)
        {
            PopStep();
            return false;
        }
        
        UpdateStep(end + 1, UnityType.Line, true);
        return true;
    }

    private bool GoToNextCharacter()
    {
        var crr = PopStep()!;
        var parent = PopStep()!;

        AddStep(parent.Index, parent.Type, parent.Started);
        AddStep(crr.Index, crr.Type, crr.Started);

        if (parent is null || parent.Type == UnityType.All)
            return NextCharacter();
        
        if (parent.Type == UnityType.Line)
            return NextCharacterLine();
        
        throw new Exception("Invalid processing: Character processing need start in all or line processing.");
    }

    private bool NextCharacter()
    {
        var step = pointerStack.Peek();
        int index = step.Index;

        if (!step.Started)
        {
            UpdateStep(index, UnityType.Character, true);
            return true;
        }
        
        index++;
        if (index >= data.Count)
        {
            PopStep();
            return false;
        }
        
        UpdateStep(index, UnityType.Character, true);
        return true;
    }

    private bool NextCharacterLine()
    {
        var step = pointerStack.Peek();
        int index = step.Index;

        if (!step.Started)
        {
            UpdateStep(step.Index, step.Type, true);
            return true;
        }

        if (index >= this.data.Count)
        {
            PopStep();
            return false;
        }
        
        if (this.data[index].Character == '\n')
        {
            PopStep();
            return false;
        }
        
        index++;
        if (index >= this.data.Count)
        {
            PopStep();
            return false;
        }

        UpdateStep(index, UnityType.Character, true);
        return true;
    }

    private void AppendAll(Data data)
        => this.data.Add(data);

    private void AppendLine(Data data)
    {
        var step = this.pointerStack.Peek();
        int end = GetEndLineIndex(step);
        if (this.data[end].Character == '\n')
            this.data.Insert(data, end);
        else this.data.Insert(data, end + 1);
    }

    private void AppendCharacter(Data data)
    {
        var step = this.pointerStack.Peek();
        this.data.Insert(data, step.Index + 1);
    }

    private void Append(Data data)
    {
        var step = pointerStack.Peek();
        var type = step.Type;

        if (type == UnityType.All)
            AppendAll(data);
        else if (type == UnityType.Line)
            AppendLine(data);
        else if (type == UnityType.Character)
            AppendCharacter(data);
    }

    private void PrependAll(Data data)
    {
        this.data.Insert(data, 0);
    }

    private void PrependLine(Data data)
    {
        var step = this.pointerStack.Peek();
        int index = GetStartLineIndex(step);
        this.data.Insert(data, index);
    }

    private void PrependCharacter(Data data)
    {
        var step = this.pointerStack.Peek();
        this.data.Insert(data, step.Index - 1);
    }

    private void Prepend(Data data)
    {
        var step = this.pointerStack.Peek();
        var type = step.Type;

        if (type == UnityType.All)
            PrependAll(data);
        else if (type == UnityType.Line)
            PrependLine(data);
        else if (type == UnityType.Character)
            PrependCharacter(data);
    }

    private void RemoveUnity()
    {
        var step = this.pointerStack.Peek();

        if (step.Type == UnityType.All)
        {
            this.data.Clear();
            return;
        }

        if (step.Type == UnityType.Line)
        {
            int start = GetStartLineIndex(step);
            int end = GetStartLineIndex(step);
            this.data.Remove(start, end - start + 1);
            return;
        }

        if (step.Type == UnityType.Character)
        {
            int index = step.Index;
            this.data.Remove(index, 1);
            int newIndex = index - 1;
            UpdateStep(newIndex, UnityType.Character, step.Started);
            return;
        }
    }

    /// <summary>
    /// Create a Text object asynchronously from a file path.
    /// </summary>
    public static async Task<Text> FromFile(string path)
    {
        var reader = new StreamReader(path);
        string source = await reader.ReadToEndAsync();
        reader.Close();
        
        return new Text(source, path);
    }

    public static implicit operator string(Text text)
        => text?.ToString() ?? string.Empty;

    private record Data(char Character = '\0', Token? Token = null, int Line = -1);

    private record ProcessStep(int Index, UnityType Type, bool Started = false);
}