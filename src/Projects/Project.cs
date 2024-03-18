/* Author:  Leonardo Trevisan Silio
 * Date:    18/03/2024
 */
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Orkestra.Projects;

/// <summary>
/// Represents a collection of compile actions,
/// tree processing and output generation.
/// </summary>
public class Project<T>
    where T : Project<T>, new()
{
    public static void Compile()
    {
        var prj = new T();
        prj.StartCompilation();
    }

    List<CompileAction> actions = new();
    
    public void Add<C>(PathSelector selector)
        where C : Compiler, new()
        => actions.Add(new(selector, new C()));
    
    /// <summary>
    /// Start compile process.
    /// </summary>
    public void StartCompilation()
    {
        var dir = Environment.CurrentDirectory;
        ConcurrentQueue<CompilerOutput> results = new();

        var compilationPairs = actions
            .Select(
                action => action.Selector
                    .GetFiles(dir)
                    .Select(file => (file, action.Compiler))
            )
            .SelectMany(x => x);

        Parallel.ForEach(compilationPairs, tuple =>
        {
            (var file, var compiler) = tuple;
            var tree = compiler.Compile(file);
            var result = new CompilerOutput(
                file, compiler, tree
            );
            results.Enqueue(result);
        });
    }
}