/* Author:  Leonardo Trevisan Silio
 * Date:    12/03/2024
 */
using System;

namespace Orkestra.LineInterfaces;

/// <summary>
/// Represents a detailed help message that will showed in CLI help command.
/// </summary>
public class DetailedHelpMessageAttribute(string message) : Attribute
{
    public readonly string Message = message; 
}