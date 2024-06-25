/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2024
 */
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Orkestra.Processings;

using SyntacticAnalysis;
using InternalStructure;
using System.Threading.Tasks;
using System.Collections.Specialized;

/// <summary>
/// A Tree Pointer for Text elements
/// </summary>
public class Text
{
    private string sourceFile;
    private FastList<Data> data;
    private Stack<ProcessStep> pointerStack;

    readonly Data newline = new('\n');
    readonly Data tab = new('\t');

    public string SourceFile => sourceFile;

    public Text(string source, string file)
    {
        this.sourceFile = file;
        initFastList(
            source.Replace("\r", "")
        );
        this.pointerStack = new Stack<ProcessStep>();
        addStep(0, UnityType.All);
    }
    
    /// <summary>
    /// Start to process lines in Text data.
    /// </summary>
    public void ProcessLines()
    {
        var step = pointerStack.Peek();

        if (step.Type == UnityType.All)
        {
            int startLine = getStartLineIndex(step);
            addStep(startLine, UnityType.Line);
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

        if (step.Type == UnityType.All || step.Type == UnityType.Character)
        {
            addStep(step.Index, UnityType.Character);
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
            UnityType.Character => goToNextCharacter(),
            UnityType.Line => goToNextLine(),
            _ => true
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
        append(newData);
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

    public void Prepend(Token token)
    {
        var data = new Data('\0', token);
        prepend(data);
    }

    public void Prepend(Key baseKey)
    {
        // TODO: get Index
        Token token = new Token(baseKey, null, sourceFile, 0);
        Prepend(token);
    }

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
            updateStep(step.Index - data.Length, UnityType.Character);
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
            this.data.Remove(start, -1);
            this.pointerStack.Push(step);
            return;
        }
        else if (parent.Type == UnityType.All && step.Type == UnityType.Character)
        {
            int start = step.Index;
            this.data.Remove(start, -1);
            this.pointerStack.Push(step);
            return;
        }
        else if (parent.Type == UnityType.Line && step.Type == UnityType.Character)
        {
            int start = step.Index;
            int end = getEndLineIndex(step);
            this.data.Remove(start, end - start);
            this.pointerStack.Push(step);
            return;
        }
        
        throw new Exception("Inconsistence in stack pointers.");
    }

    public bool Next2()
    {
        var step = this.pointerStack.Peek();

        switch (step.Type)
        {
            case UnityType.All:
                this.pointerStack.Pop();
                addStep(-1, UnityType.All);
                return true;
            
            case UnityType.Line:
                return goToNextLine();
            
            case UnityType.Character:
                step = this.pointerStack.Pop();
                var parent = this.pointerStack.Peek();
                this.pointerStack.Push(step);

                if (parent.Type == UnityType.All)
                    return nextCharacter();
                else if (parent.Type == UnityType.Line)
                    return nextCharacterLine();
                
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

        while (index < this.data.Count)
        {
            sb = new StringBuilder();
            while (this.data[index].Token is null)
            {
                sb.Append(this.data[index].Character);

                index++;
                if (index >= this.data.Count)
                    break;
            }
            data.Add(sb.ToString());
            if (index >= this.data.Count)
                break;

            while (this.data[index].Token is not null)
            {
                data.Add(this.data[index].Token);

                index++;
                if (index >= this.data.Count)
                    break;
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
            return null;

        if (type == UnityType.Line)
        {
            StringBuilder sb = new StringBuilder();

            var start = getStartLineIndex(step);
            var end = getEndLineIndex(step);

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

    void initFastList(string text)
    {
        this.data = new FastList<Data>();

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

    void addStep(int index, UnityType type)
        => pointerStack.Push(new(index, type));

    ProcessStep popStep() => 
        pointerStack.Count == 0 ? 
            null : 
            pointerStack.Pop();

    void updateStep(int index, UnityType type)
    {
        popStep();
        addStep(index, type);
    }

    int getStartLineIndex(ProcessStep step)
    {
        int i = step.Index - 1;
        while (i >= 0 && data[i].Character != '\n')
            i--;
        
        if (i == 0)
            return 0;
        
        return i + 1;
    }

    int getEndLineIndex(ProcessStep step)
    {
        int i = step.Index;
        while (i < data.Count && data[i].Character != '\n')
            i++;
        
        if (i == this.data.Count)
            i--;
        
        return i;
    }

    bool goToNextLine()
    {
        var step = pointerStack.Peek();

        var end = getEndLineIndex(step);
        if (end > this.data.Count)
        {
            PopProcessing();
            return false;
        }
        
        updateStep(end + 1, UnityType.Line);
        return true;
    }

    bool goToNextCharacter()
    {
        var crr = popStep();
        var parent = popStep();

        addStep(parent.Index, parent.Type);
        addStep(crr.Index, crr.Type);

        if (parent is null || parent.Type == UnityType.All)
            return nextCharacter();
        
        if (parent.Type == UnityType.Line)
            return nextCharacterLine();
        
        throw new Exception("Invalid processing: Character processing need start in all or line processing.");
    }

    bool nextCharacter()
    {
        var step = pointerStack.Peek();
        int index = step.Index;
        
        index++;
        if (index >= data.Count)
        {
            popStep();
            return false;
        }
        
        updateStep(index, UnityType.Character);
        return true;
    }

    bool nextCharacterLine()
    {
        var step = pointerStack.Peek();
        int index = step.Index;

        if (index >= this.data.Count)
        {
            popStep();
            return false;
        }
        
        if (this.data[index].Character == '\n')
        {
            popStep();
            return false;
        }
        
        index++;
        if (index >= this.data.Count)
        {
            popStep();
            return false;
        }

        updateStep(index, UnityType.Character);
        return true;
    }

    void appendAll(Data data)
        => this.data.Add(data);

    void appendLine(Data data)
    {
        var step = this.pointerStack.Peek();
        int end = getEndLineIndex(step);
        if (this.data[end].Character == '\n')
            this.data.Insert(data, end);
        else this.data.Insert(data, end + 1);
    }

    void appendCharacter(Data data)
    {
        var step = this.pointerStack.Peek();
        this.data.Insert(data, step.Index + 1);
    }

    private void append(Data data)
    {
        var step = pointerStack.Peek();
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
        this.data.Insert(data, 0);
    }

    private void prependLine(Data data)
    {
        var step = this.pointerStack.Peek();
        int index = getStartLineIndex(step);
        this.data.Insert(data, index);
    }

    private void prependCharacter(Data data)
    {
        var step = this.pointerStack.Peek();
        this.data.Insert(data, step.Index - 1);
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

    private void removeUnity()
    {
        var step = this.pointerStack.Peek();

        if (step.Type == UnityType.All)
        {
            this.data.Clear();
            return;
        }

        if (step.Type == UnityType.Line)
        {
            int start = getStartLineIndex(step);
            int end = getStartLineIndex(step);
            this.data.Remove(start, end - start + 1);
            return;
        }

        if (step.Type == UnityType.Character)
        {
            int index = step.Index;
            this.data.Remove(index, 1);
            int newIndex = index - 1;
            updateStep(newIndex, UnityType.Character);
            return;
        }
    }

    /// <summary>
    /// Create a Text object asynchronously from a file path.
    /// </summary>
    public static async Task<Text> FromFile(string path)
    {
        StreamReader reader = new StreamReader(path);
        string source = await reader.ReadToEndAsync();
        reader.Close();
        
        return new Text(source, path);
    }

    public static implicit operator string(Text text)
        => text?.ToString() ?? string.Empty;

    record Data(char Character = '\0', Token Token = null, int Line = -1);
    record ProcessStep(int Index, UnityType Type);
}