/* Author:  Leonardo Trevisan Silio
 * Date:    04/07/2023
 */
using System;

namespace Orkestra.Exceptions;

public class ManyCompilerDefinitionException : Exception
{
    public override string Message => 
        """
        Many compiler class definition. To solve this problem you
        can remove additional classes that inherits from Compiler class
        or add [Ignore] over theses classes (except one).

        This occours because a default Project is loaded and need
        find a default Compiler in your code. Specify the Project
        class can solve this problem if having many compilers is 
        intentional.
        """;
}