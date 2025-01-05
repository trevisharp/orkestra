/* Author:  Leonardo Trevisan Silio
 * Date:    04/07/2024
 */
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Orkestra;

using static Verbose;

using Caches;
using Providers;
using Extensions;
using Processings;
using LexicalAnalysis;
using SyntacticAnalysis;
using Processings.Implementations;

/// <summary>
/// A base class for all compiler created with Orkestra framework.
/// </summary>
public class Compiler
{
    private bool loadedFromFields = false;

    string? loadedName = null;
    string LoadName()
    {
        var className = ToString() ?? "nonamecompiler";
        var pascalLangName = className
            .Replace("Compiler", "")
            .Replace("compiler", "");
        
        return string.Concat(
            pascalLangName.Select((c, i) =>
                (c, i) switch
                {
                    _ when c is >= 'A' and <= 'Z' && i is not 0 => $" {c}",
                    _ => c.ToString()
                }
            )
        );
    }
    public string Name => loadedName ??= LoadName();

    public IAlgorithmGroupProvider Provider { get; set; }
    public List<Key> Keys { get; private set; } = new();
    public List<Rule> Rules { get; private set; } = new();
    public List<Processing> Processings { get; private set; } = new();

    /// <summary>
    /// Load all data of compiler from fields.
    /// </summary>
    public void Load()
    {
        LoadFromFields();
    }

    /// <summary>
    /// Get metadata of language defined by the compiler.
    /// </summary>
    public LanguageInfo GetInfo()
    {
        return new() {
            Name = getSpecialName(),
            Keys = Keys,
            Rules = Rules
        };
    }

    public async Task<ExpressionTree> Compile(string filePath, params string[] args)
    {
        // TODO: Finish Cache use
        var lstWrite = await Cache.LastWrite.TryGet(filePath);
        var newWrite = File.GetLastWriteTime(filePath);
        await Cache.LastWrite.Set(filePath, newWrite);

        var sourceCode = await Text.FromFile(filePath);

        Info($"[{filePath}]: Preprocessing started...", 3);
        var machine = buildProcessingMachine();
        var processedText = machine.ProcessAll(sourceCode);
        Success($"[{filePath}]: Preprocessing completed!", 3);
        Content($"[{filePath}]: Processed Text:", 10);
        Content(processedText, 10);
        NewLine(1);

        Info($"[{filePath}]: Lexical Analysis started...", 3);
        var lex = buildLexicalAnalyzer();
        var tokens = lex.Parse(processedText);
        Success($"[{filePath}]: Lexical Analysis completed!", 3);
        Content($"[{filePath}]: Token List:", 10);
        foreach (var token in tokens)
            InlineContent(token, 10);
        NewLine(1);

        Info($"[{filePath}]: Syntacic Analysis started...", 3);
        var parser = buildSyntacticAnalyzer();
        var tree = parser.Parse(tokens);
        Success($"[{filePath}]: Syntacic Analysis completed!", 3);
        Content($"[{filePath}]: Syntacic Tree:", 10);
        Content(tree.ToString(), 10);
        NewLine(1);

        return tree;
    }

    protected static LineCommentProcessing lineComment(string starter)
        => new(starter);

    protected static Key key(string name, string expression)
        => Key.CreateKey(name, expression);

    protected static Key keyword(string name, string expression)
        => Key.CreateKeyword(name, expression);

    protected static Key keyword(string expression)
        => keyword(expression.ToUpper(), expression);

    protected static Key contextual(string expression)
        => Key.CreateContextual(null, expression);

    protected static Key auto(string name)
        => Key.CreateAutoKeyword(name);

    protected static Key identity(string expression)
        => Key.CreateIdentity(expression);
    
    protected static Rule rule(Func<Rule, List<ISyntacticElement>[]> creator)
    {
        var rule = Rule.CreateRule();
        rule.AddSubRules(creator(rule));
        return rule;
    }
    
    protected static Rule start(params List<ISyntacticElement>[] subRules)
    {
        var rule = Rule.CreateStartRule();
        rule.AddSubRules(subRules);
        return rule;
    }

    protected static Rule start(Func<Rule, List<ISyntacticElement>[]> creator)
    {
        var rule = Rule.CreateStartRule();
        rule.AddSubRules(creator(rule));
        return rule;
    }
    
    protected static Rule start(params ISyntacticElement[] elements)
    {
        var rule = Rule.CreateStartRule();
        rule.AddSubRules(
            new SubRule(elements)
        );
        return rule;
    }
    
    protected static Rule many(ISyntacticElement element, ISyntacticElement separator = null)
    {
        var newRule = Rule.CreateRule();
        newRule.AddSubRules(
            new SubRule(element),
            separator is null
                ? new SubRule(element, newRule)
                : new SubRule(element, separator, newRule)
        );
        return newRule;
    }

    private string getSpecialName()
    {
        var baseName = GetType().Name;
        if (baseName == "Compiler")
            return "no-named-lang";
        return baseName.Replace("Compiler", "");
    }

    private void LoadFromFields()
    {
        if (loadedFromFields)
            return;
        loadedFromFields = true;
        setEmptyNames();

        Keys = Keys
            .Concat(getFields<Key>())
            .Distinct()
            .ToList();
        
        Rules = Rules
            .Concat(getFields<Rule>())
            .Distinct()
            .ToList();
        
        Processings = Processings
            .Concat(getFields<Processing>())
            .Distinct()
            .ToList();
    }

    private ProcessingCollection buildProcessingMachine()
    {
        ProcessingCollection package = new ProcessingCollection();

        foreach (var process in getFields<Processing>())
            package.Add(process);

        return package;
    }
    
    private ILexicalAnalyzer buildLexicalAnalyzer()
    {
        var lexicalAnalyzer = Provider.ProvideLexicalAnalyzer();
        lexicalAnalyzer.AddKeys(Keys);
        return lexicalAnalyzer;
    }

    private ISyntacticAnalyzer buildSyntacticAnalyzer()
    {
        var builder = Provider.ProvideSyntacticAnalyzerBuilder();
        var loaded = builder.LoadCache();

        if (loaded)
            return builder.Build();
        
        foreach (var rule in Rules)
        {
            if (rule is null)
                continue;

            if (rule.IsStartRule)
                builder.StartRule = rule;
            builder.Add(rule);
        }
        builder.Load(Keys);
        builder.SaveCache();

        return builder.Build();
    }

    private void setEmptyNames()
    {
        var type = this.GetType();
        foreach (var field in type.GetRuntimeFields())
        {
            if (field.FieldType != typeof(Rule) && field.FieldType != typeof(Key))
                continue;
            
            var obj = field.GetValue(this) as ISyntacticElement;
            if (obj is null)
                continue;
            
            if (!string.IsNullOrEmpty(obj.Name))
                continue;
            
            obj.Name = field.Name;
        }
    }

    private IEnumerable<T> getFields<T>()
        where T : class
    {
        var type = this.GetType();
        foreach (var field in type.GetRuntimeFields())
        {
            if (field.FieldType == typeof(T))
                yield return field.GetValue(this) as T;
        }
    }
}