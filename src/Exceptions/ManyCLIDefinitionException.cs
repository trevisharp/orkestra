/* Author:  Leonardo Trevisan Silio
 * Date:    04/07/2023
 */
using System;

namespace Orkestra.Exceptions;

public class ManyCLIDefinitionException : Exception
{
    public override string Message => 
        """
        Many cli class definition. To solve this problem you
        can remove additional classes that inherits from CLI class
        or add [Ignore] over theses classes (except one).
        """;
}