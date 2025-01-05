/* Author:  Leonardo Trevisan Silio
 * Date:    30/06/2023
 */
using System;

namespace Orkestra.Exceptions;

[Serializable]
public class NoConstructorException(string type) : OrkestraException
{
    public override string Message => 
    $$"""
    The {{type}} class need a empty constructor to be created.
    """;
}
