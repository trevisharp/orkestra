/* Author:  Leonardo Trevisan Silio
 * Date:    11/06/2024
 */
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Orkestra.Projects;

using System.IO;
using Exceptions;
using InternalStructure;
using Orkestra.Extensions;
using Orkestra.Providers;

/// <summary>
/// Represents a collection of compile actions,
/// tree processing and output generation.
/// </summary>
public class Project<T>
    where T : Project<T>, new()
{
    List<CompileAction> actions = new();
    public IExtensionProvider ExtensionProvider { get; set; }
        = new DefaultExtensionProvider();

    public static void Compile(params string[] args)
    {
        var prj = new T();
        prj.StartCompilation(args);
    }

    public static void InstallExtension(params string[] args)
    {
        var prj = new T();
        prj.CreateInstallExtension(args);
    }

    public void Add<C>(PathSelector selector)
        where C : Compiler, new()
        => actions.Add(new(
            selector, 
            ReflectionHelper.GetConfiguredCompiler<C>()
        ));

    public void CreateInstallExtension(params string[] args)
    {
        Verbose.Info("Generating Extension...");
        var extension = ExtensionProvider.Provide();

        Verbose.Info("Loading language metadata...", 1);
        var extArgs = new ExtensionArguments
        {
            Name = typeof(T).Name.Replace("Project", ""),
            Arguments = args
        };
        
        foreach (var lang in getLangs())
            extArgs.Languages.Add(lang);

        try
        {
            Verbose.StartGroup();
            extension.Generate(extArgs).Wait();
        }
        catch (Exception ex)
        {
            Verbose.Error("Error in extension generation!");
            Verbose.Error("Use --verbose 1 or bigger to see details...");
            Verbose.Error(ex.Message, 1);
            Verbose.Error(ex.StackTrace, 2);
        }
        finally
        {
            Verbose.EndGroup();
            Verbose.Info("Extension generation process finished!");
        }
    }      

    /// <summary>
    /// Start compile process.
    /// </summary>
    public void StartCompilation(string[] args)
    {
        var dir = Environment.CurrentDirectory;
        ConcurrentQueue<CompilerOutput> queue = new();

        var compilationPairs = actions
            .Select(
                action => action.Selector
                    .GetFiles(dir)
                    .Select(file => (file, action.Compiler))
            )
            .SelectMany(x => x);
        
        try
        {
            Parallel.ForEachAsync(compilationPairs, async (tuple, tk) =>
            {
                (var file, var compiler) = tuple;
                var tree = await compiler.Compile(file, args);
                var result = new CompilerOutput(
                    file, compiler, tree
                );
                queue.Enqueue(result);
            }).Wait();
        }
        catch (AggregateException ex)
        {
            Verbose.Error(
                string.Join('\n', ex.InnerExceptions
                    .Where(e => e is SyntacticException)
                    .Select(e => e.Message)
                )
            );
        }

        var results = queue.ToArray();
    }

    private IEnumerable<LanguageInfo> getLangs() => 
        from action in actions
        where action.Selector is FileSelector
        select new LanguageInfo
        {
            Name = action.Compiler.Name
                .Replace(" ", "")
                .ToLower(),
            Extension = ((FileSelector)action.Selector).Extension,
            Keys = action.Compiler.Keys,
            Rules = action.Compiler.Rules
        };
}