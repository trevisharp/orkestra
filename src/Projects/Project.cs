/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2024
 */
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Orkestra.Projects;

using Providers;
using Exceptions;
using Extensions;
using InternalStructure;

/// <summary>
/// Represents a collection of compile actions,
/// tree processing and output generation.
/// </summary>
public class Project
{
    List<CompileAction> actions = new();
    public IExtensionProvider ExtensionProvider { get; set; }
        = new DefaultExtensionProvider();

    /// <summary>
    /// Add a compiler action to the project.
    /// </summary>
    public void Add(CompileAction action)
        => actions.Add(action ?? throw new ArgumentNullException(nameof(action)));

    /// <summary>
    /// Add a compiler action to the project.
    /// </summary>
    public void Add<C>(PathSelector selector)
        where C : Compiler, new()
        => actions.Add(new(
            selector, 
            ReflectionHelper.GetCompilerByType<C>()
        ));
    
    public void InstallExtension(params string[] args)
    {
        Verbose.Info("Generating and installing Extension...");
        var extension = ExtensionProvider.Provide();

        Verbose.Info("Loading language metadata...", 1);
        var extArgs = getArgs(args);

        try
        {
            extension.Install(extArgs).Wait();
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
            Verbose.Info("Extension generation process finished!");
        }
    }   

    public void GenerateExtension(string[] args)
    {
        Verbose.Info("Generating Extension...");
        var extension = ExtensionProvider.Provide();

        Verbose.Info("Loading language metadata...", 1);
        var extArgs = getArgs(args);

        try
        {
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
            Verbose.Info("Extension generation process finished!");
        }
    }   


    /// <summary>
    /// Start compile process.
    /// </summary>
    public void Compile(string[] args)
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
                try
                {
                    (var file, var compiler) = tuple;
                    var tree = await compiler.Compile(file, args);
                    var result = new CompilerOutput(
                        file, compiler, tree
                    );
                    queue.Enqueue(result);
                }
                catch (SyntacticException ex)
                {
                    Verbose.Error($"Syntax error in {tuple.file} compilation:");
                    Verbose.Error(ex.Message);
                }
                catch (Exception ex)
                {
                    Verbose.Error($"Internal error in {tuple.file} compilation!");
                    Verbose.StartGroup();
                    Verbose.Error(ex.Message);
                    Verbose.Error(ex.StackTrace, 1);
                    Verbose.EndGroup();
                }
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

    private ExtensionArguments getArgs(string[] args)
    {
        var extArgs = new ExtensionArguments
        {
            Name = GetType().Name.Replace("Project", ""),
            Arguments = args
        };
        
        foreach (var lang in getLangs())
            extArgs.Languages.Add(lang);
        
        return extArgs;
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
            Rules = action.Compiler.Rules,
            Processings = action.Compiler.Processings
        };

    /// <summary>
    /// Create a empty project with only one compiler for a only extension type.
    /// </summary>
    public static Project CreateDefault(string path, Compiler compiler)
    {
        var prj = new Project();
        prj.Add(new CompileAction(
            new FileSelector(path), 
            compiler
        ));
        return prj;
    }
}