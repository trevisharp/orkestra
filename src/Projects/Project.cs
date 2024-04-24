/* Author:  Leonardo Trevisan Silio
 * Date:    24/04/2024
 */
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Orkestra.Projects;

using InternalStructure;

/// <summary>
/// Represents a collection of compile actions,
/// tree processing and output generation.
/// </summary>
public class Project<T>
    where T : Project<T>, new()
{
    public static void Compile(params string[] args)
    {
        var prj = new T();
        prj.StartCompilation(args);
    }

    List<CompileAction> actions = new();
    
    public void Add<C>(PathSelector selector)
        where C : Compiler, new()
        => actions.Add(new(
            selector, 
            ReflectionHelper.GetConfiguredCompiler<C>()
        ));
    
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

        Parallel.ForEachAsync(compilationPairs, async (tuple, tk) =>
        {
            (var file, var compiler) = tuple;
            var tree = await compiler.Compile(file, args);
            var result = new CompilerOutput(
                file, compiler, tree
            );
            queue.Enqueue(result);
        }).Wait();

        var results = queue.ToArray();
    }
}