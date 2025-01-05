/* Author:  Leonardo Trevisan Silio
 * Date:    24/06/2024
 */
namespace Orkestra;

/// <summary>
/// Represents a named element that can be used in a rule.
/// </summary>
public interface ISyntacticElement
{
    /// <summary>
    /// The string name of the element.
    /// </summary>
    string? Name { get; set; }
}