/* Author:  Leonardo Trevisan Silio
 * Date:    17/03/2024
 */
using System.Collections.Generic;

namespace Orkestra.Projects;

/// <summary>
/// Represents a collection of compile actions,
/// tree processing and output generation.
/// </summary>
public class Project
{
    List<CompileAction> actions = new();
    
    public void Add<C>(PathSelector selector)
        where C : Compiler, new()
        => actions.Add(new(selector, new C()));
}