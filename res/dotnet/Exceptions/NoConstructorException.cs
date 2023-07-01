/* Author:  Leonardo Trevisan Silio
 * Date:    30/06/2023
 */
using System;

namespace Orkestra.Exceptions;

[Serializable]
public class NoConstructorException : OrkestraException
{
    public override string Message => 
    """
    The Compiler class need a empty constructor to be created.
    """;

    public NoConstructorException() { }
}
