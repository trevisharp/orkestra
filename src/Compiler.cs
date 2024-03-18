/* Author:  Leonardo Trevisan Silio
 * Date:    18/03/2024
 */
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Orkestra;

using static Verbose;

using Providers;
using Processings;
using LexicalAnalysis;
using SyntacticAnalysis;

/// <summary>
/// A base class for all compiler created with Orkestra framework.
/// </summary>
public abstract class Compiler
{
    public IAlgorithmGroupProvider Provider { get; set; }

    public ExpressionTree Compile(string filePath)
    {
        var sourceCode = File.ReadAllText(filePath);
        Info("Build started...");
        NewLine();

        Info("Preprocessing started...", 1);
        var machine = buildProcessingMachine();
        var processedText = machine.ProcessAll(sourceCode);
        Success("Preprocessing completed!", 1);
        Content("Processed Text:", 2);
        Content(processedText, 2);
        NewLine();

        Info("Lexical Analysis started...", 1);
        var keys = getKeys();
        var lex = buildLexicalAnalyzer(keys);
        var tokens = lex.Parse(processedText);
        Success("Lexical Analysis completed!", 1);
        Content("Token List:", 2);
        foreach (var token in tokens)
            InlineContent(token, 2);
        NewLine();

        Info("Syntacic Analysis started...", 1);
        var parser = buildSyntacticAnalyzer(keys);
        var tree = parser.Parse(tokens);
        Success("Syntacic Analysis completed!", 1);
        Content("Processed Text:", 2);
        Content(processedText, 2);
        NewLine();

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

    protected static Rule rule(string name, params SubRule[] subRules)
        => Rule.CreateRule(name, subRules);

    protected static SubRule sub(params ISyntacticElement[] elements)
        => SubRule.Create(elements);

    private ProcessingCollection buildProcessingMachine()
    {
        ProcessingCollection package = new ProcessingCollection();

        foreach (var process in getFields<Processing>())
            package.Add(process);

        return package;
    }
    
    private IEnumerable<Key> getKeys()
        => getFields<Key>();

    private ILexicalAnalyzer buildLexicalAnalyzer(IEnumerable<Key> keys)
    {
        var lexicalAnalyzer = Provider.ProvideLexicalAnalyzer();

        lexicalAnalyzer.AddKeys(keys);

        return lexicalAnalyzer;
    }

    private ISyntacticAnalyzer buildSyntacticAnalyzer(IEnumerable<Key> keys)
    {
        var builder = Provider.ProvideSyntacticAnalyzerBuilder();
        var loaded = builder.LoadCache();

        if (loaded)
            return builder.Build();
        
        foreach (var rule in getFields<Rule>())
        {
            if (rule.IsStartRule)
                builder.StartRule = rule;
            builder.Add(rule);
        }
        builder.Load(keys);
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