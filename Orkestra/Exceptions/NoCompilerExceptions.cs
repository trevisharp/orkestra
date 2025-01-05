/* Author:  Leonardo Trevisan Silio
 * Date:    12/03/2023
 */
using System;

namespace Orkestra.Exceptions;

[Serializable]
public class NoCompilerExceptionsException : OrkestraException
{
    public override string Message => 
    """
    No exists class that inherits from Compiler and 
    not is marked with attribute IgnoreAttribute in this assembly.
    Create a class in the same assembly that OrkestraApp.Compile is used
    to be solve the problem.
    You can create a class to inherits from other class in another
    assembly that inherits from Compiler too.
    """;
}