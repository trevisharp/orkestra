/* Author:  Leonardo Trevisan Silio
 * Date:    05/01/2025
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
using Orkestra.Exceptions;

/// <summary>
/// A base class for all compiler created with Orkestra framework.
/// </summary>
public class Compiler
{
    bool loadedFromFields = false;
    string? loadedName = null;
    public string Name => loadedName ??= LoadName();

    public IAlgorithmGroupProvider? Provider { get; set; }
    public Rule StartRule { get; protected set; } = [];
    public List<Key> Keys { get; protected set; } = [];
    public List<Rule> Rules { get; protected set; } = [];
    public List<Processing> Processings { get; protected set; } = [];

    /// <summary>
    /// Load all data of compiler from fields.
    /// </summary>
    public virtual void Load()
    {
        LoadFromFields();
    }

    /// <summary>
    /// Get metadata of language defined by the compiler.
    /// </summary>
    public LanguageInfo GetInfo()
    {
        return new() {
            StartRule = StartRule,
            Extension = null,
            Name = GetSpecialName(),
            Keys = [ ..Keys ],
            Rules = [ ..Rules ],
            Processings = [ ..Processings ],
        };
    }

    public virtual async Task<ExpressionTree> Compile(string filePath, params string[] args)
    {
        if (Provider is null)
            throw new MissingProviderException();

        // TODO: Finish Cache use
        var lstWrite = await Cache.LastWrite.TryGet(filePath);
        var newWrite = File.GetLastWriteTime(filePath);
        await Cache.LastWrite.Set(filePath, newWrite);

        var sourceCode = await Text.FromFile(filePath);

        Info($"[{filePath}]: Preprocessing started...", 3);
        var machine = BuildProcessingMachine();
        var processedText = machine.ProcessAll(sourceCode);
        Success($"[{filePath}]: Preprocessing completed!", 3);
        Content($"[{filePath}]: Processed Text:", 10);
        Content(processedText, 10);
        NewLine(1);

        Info($"[{filePath}]: Lexical Analysis started...", 3);
        var lex = BuildLexicalAnalyzer(Provider);
        var tokens = lex.Parse(processedText);
        Success($"[{filePath}]: Lexical Analysis completed!", 3);
        Content($"[{filePath}]: Token List:", 10);
        foreach (var token in tokens)
            InlineContent(token, 10);
        NewLine(1);

        Info($"[{filePath}]: Syntacic Analysis started...", 3);
        var parser = BuildSyntacticAnalyzer(Provider);
        var tree = parser.Parse(tokens);
        Success($"[{filePath}]: Syntacic Analysis completed!", 3);
        Content($"[{filePath}]: Syntacic Tree:", 10);
        Content(tree.ToString(), 10);
        NewLine(1);

        return tree;
    }

    ProcessingCollection BuildProcessingMachine()
    {
        ProcessingCollection package = new();

        foreach (var process in GetFields<Processing>())
            package.Add(process);

        return package;
    }
    
    ILexicalAnalyzer BuildLexicalAnalyzer(IAlgorithmGroupProvider provider)
    {
        var lexicalAnalyzer = provider.ProvideLexicalAnalyzer();
        lexicalAnalyzer.AddKeys(Keys);
        return lexicalAnalyzer;
    }

    ISyntacticAnalyzer BuildSyntacticAnalyzer(IAlgorithmGroupProvider provider)
    {
        var builder = provider.ProvideSyntacticAnalyzerBuilder();
        var loaded = builder.LoadCache();

        if (loaded)
            return builder.Build();
        
        foreach (var rule in Rules)
            builder.Add(rule);
        builder.StartRule = StartRule;
        builder.Load(Keys);
        builder.SaveCache();

        return builder.Build();
    }

    string GetSpecialName()
    {
        var baseName = GetType().Name;
        if (baseName == "Compiler")
            return "no-named-lang";
        return baseName.Replace("Compiler", "");
    }

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

    void LoadFromFields()
    {
        if (loadedFromFields)
            return;
        
        loadedFromFields = true;
        SetEmptyNames();

        Keys = Keys
            .Concat(GetFields<Key>())
            .Distinct()
            .ToList();
        
        Rules = Rules
            .Concat(GetFields<Rule>())
            .Distinct()
            .ToList();
        
        Processings = Processings
            .Concat(GetFields<Processing>())
            .Distinct()
            .ToList();
        
        StartRule = GetStartRule();
    }

    void SetEmptyNames()
    {
        var type = GetType();
        foreach (var field in type.GetRuntimeFields())
        {
            if (field.GetValue(this) is not ISyntacticElement obj)
                continue;

            if (!string.IsNullOrEmpty(obj.Name))
                continue;
            
            obj.Name = field.Name;
        }
    }

    IEnumerable<T> GetFields<T>() where T : class
    {
        var type = GetType();
        foreach (var field in type.GetRuntimeFields())
        {
            if (field.FieldType != typeof(T))
                continue;
            
            var value = field.GetValue(this) as T;
            yield return value!;
        }
    }

    Rule GetStartRule()
    {
        var type = GetType();
        foreach (var field in type.GetRuntimeFields())
        {
            if (field.FieldType != typeof(Rule))
                continue;
            
            var attribute = field.GetCustomAttribute<StartAttribute>();
            if (attribute is null)
                continue;
            
            var value = field.GetValue(this) as Rule;
            return value!;
        }

        throw new MissingFirstRuleException();
    }
}