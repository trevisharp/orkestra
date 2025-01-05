/* Author:  Leonardo Trevisan Silio
 * Date:    04/07/2023
 */
using System;

namespace Orkestra.Exceptions;

public class MissingCLIDefinitionException : Exception
{
    public override string Message => 
        """
        Missing a cli class definition. To solve this problem you
        can:
        A) Define a class likes that:

         1. public class MyCLI : CLI
         2. {
         3. 
         4. }
        
        B) Sometimes you added [Ignore] in all classes that inherits
        from Project. Remove the attribute of one of them.
        """;
}