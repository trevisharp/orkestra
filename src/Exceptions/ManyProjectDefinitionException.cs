/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2023
 */
using System;

namespace Orkestra.Exceptions;

public class ManyProjectDefinitionException : Exception
{
    public override string Message => 
        """
        Many projects class definition. To solve this problem you
        can remove additional classes that inherits from Project class
        or add [Ignore] over theses classes (except one).
        """;
}