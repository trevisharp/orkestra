/* Author:  Leonardo Trevisan Silio
 * Date:    18/03/2024
 */
namespace Orkestra.Projects;

using SyntacticAnalysis;

/// <summary>
/// The result of a compilation process.
/// </summary>
public record CompilerOutput(
    string FileOrigin,
    Compiler Compiler,
    ExpressionTree Tree
);