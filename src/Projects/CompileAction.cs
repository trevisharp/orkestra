/* Author:  Leonardo Trevisan Silio
 * Date:    17/03/2024
 */
namespace Orkestra.Projects;

/// <summary>
/// Represents a compilation of files.
/// </summary>
public record CompileAction(
    PathSelector Selector,
    Compiler Compiler
);