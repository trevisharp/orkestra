/* Author:  Leonardo Trevisan Silio
 * Date:    29/04/2024
 */
namespace Orkestra.Errors;

/// <summary>
/// Represents the types of errors.
/// </summary>
public enum ErrorType
{
    ProcessingError,
    LexicalError,
    SyntacticalError,
    SemanticError
}