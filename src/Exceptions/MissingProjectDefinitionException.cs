/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2023
 */
using System;

namespace Orkestra.Exceptions;

public class MissingProjectDefinitionException : Exception
{
    public override string Message => 
        """
        Missing a project class definition. To solve this problem you
        can:
        A) Define a class likes that:

         1. public class MyProject : Project<MyProject>
         2. {
         3.    public MyProject()
         4.    {
         5.        // See documentation to configure your project
         6.        Add<MyCompiler>(
         7.            new FileSelector("*.ext")
         8.        );
         9.    }
        10. }
        
        B) If your technology use only one compiler and only onde file 
        extesion you can add this code to your project:

         1. Tech.ConfigureProject("*.ext");
         2. Tech.Run(args);
        
        C) Sometimes you added [Ignore] in all classes that inherits
        from Project. Remove the attribute of one of them.
        """;
}