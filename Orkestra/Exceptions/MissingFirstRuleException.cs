/* Author:  Leonardo Trevisan Silio
 * Date:    01/05/2025
 */
using System;

namespace Orkestra.Exceptions;

public class MissingFirstRuleException : Exception
{
    public override string Message => "A Compiler needs a inital/start rule. Add [Start] on some rule.";
}