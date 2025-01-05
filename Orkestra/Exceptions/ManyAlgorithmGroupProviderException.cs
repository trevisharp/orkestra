/* Author:  Leonardo Trevisan Silio
 * Date:    04/07/2023
 */
using System;

namespace Orkestra.Exceptions;

public class ManyAlgorithmGroupProviderDefinitionException : Exception
{
    public override string Message => 
        """
        Many algorithm group provider  class definition. To solve this problem you
        can remove additional classes that inherits from IAlgorithmGroupProvider class
        or add [Ignore] over theses classes (except one).
        """;
}