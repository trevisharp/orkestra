/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2023
 */
namespace Orkestra;

/// <summary>
/// Represents a named element that can be used in a rule.
/// </summary>
public interface ISyntaticElement
{
    string KeyName { get; }
}