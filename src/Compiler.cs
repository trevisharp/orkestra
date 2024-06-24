/* Author:  Leonardo Trevisan Silio
 * Date:    24/06/2024
 */
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Orkestra;

using static Verbose;

using Caches;
using Providers;
using Processings;
using LexicalAnalysis;
using SyntacticAnalysis;
using Orkestra.Extensions;

/// <summary>
/// A base class for all compiler created with Orkestra framework.
/// </summary>
public class Compiler
{
    private bool loadedFromFields = false;

    public virtual string Name
    {
        get
        {
            var className = ToString();
            var pascalLangName = className
                .Replace("Compiler", "")
                .Replace("compiler", "");
            
            return string.Concat(
                pascalLangName.Select((c, i) =>
                    (c, i) switch
                    {
                        (_, 0) => c.ToString(),
                        _ when c >= 'A' && c <= 'Z' => $" {c}",
                        _ => c.ToString()
                    }
                )
            );
        }
    }

    public IAlgorithmGroupProvider Provider { get; set; }
    public List<Key> Keys { get; private set; } = new();
    public List<Rule> Rules { get; private set; } = new();
    public List<Processing> Processings { get; private set; } = new();

    /// <summary>
    /// Load all data of compiler from fields.
    /// </summary>
    public void Load()
    {
        loadFromFields();
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
        Info("Build started...");
        NewLine();

        Info("Preprocessing started...", 1);
        var machine = buildProcessingMachine();
        var processedText = machine.ProcessAll(sourceCode);
        Success("Preprocessing completed!", 1);
        Content("Processed Text:", 5);
        Content(processedText, 5);
        NewLine(1);

        Info("Lexical Analysis started...", 1);
        var lex = buildLexicalAnalyzer();
        var tokens = lex.Parse(processedText);
        Success("Lexical Analysis completed!", 1);
        Content("Token List:", 5);
        foreach (var token in tokens)
            InlineContent(token, 5);
        NewLine(1);

        Info("Syntacic Analysis started...", 1);
        var parser = buildSyntacticAnalyzer();
        var tree = parser.Parse(tokens);
        Success("Syntacic Analysis completed!", 1);
        Content("Syntacic Tree:", 5);
        Content(tree.ToString(), 5);
        NewLine(1);

        return tree;
    }

    protected static Key key(string name, string expression)
        => Key.CreateKey(name, expression);

    protected static Key keyword(string name, string expression)
        => Key.CreateKeyword(name, expression);

    protected static Key keyword(string expression)
        => keyword(expression.ToUpper(), expression);

    protected static Key contextual(string name, string expression)
        => Key.CreateContextual(name, expression);

    protected static Key contextual(string expression)
        => contextual(expression.ToUpper(), expression);

    protected static Key auto(string name)
        => Key.CreateAutoKeyword(name);

    protected static Key identity(string name, string expression)
        => Key.CreateIdentity(name, expression);

    protected static Rule rule(string name, params List<ISyntacticElement>[] subRules)
    {
        var rule = Rule.CreateRule(name);
        rule.AddSubRules(subRules);
        return rule;
    }
    
    protected static Rule many(ISyntacticElement element, ISyntacticElement separator = null)
    {
        var newRule = Rule.CreateRule(element.Name + "s");
        newRule.AddSubRules(
            SubRule.Create(element),
            separator is null
                ? SubRule.Create(element, newRule)
                : SubRule.Create(element, separator, newRule)
        );
        return newRule;
    }

    protected static Rule leaf(string name, params ISyntacticElement[] elements)
    {
        var newRule = Rule.CreateRule(name);
        foreach (var element in elements)
            newRule.AddSubRules(
                SubRule.Create(element)
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

    private void loadFromFields()
    {
        if (loadedFromFields)
            return;
        loadedFromFields = true;

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

    private IEnumerable<T> getFields<T>()
        where T : class
    {
        var type = this.GetType();
        foreach (var filed in type.GetRuntimeFields())
        {
            if (filed.FieldType == typeof(T))
                yield return filed.GetValue(this) as T;
        }
    }
}