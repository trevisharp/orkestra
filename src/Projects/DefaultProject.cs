/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2024
 */
namespace Orkestra.Projects;

/// <summary>
/// Represents a project of a Technology with a single file type and compiler.
/// </summary>
public class DefaultProject : Project
{
    public static DefaultProject Create(string path, Compiler compiler)
    {
        var prj = new DefaultProject();
        prj.Add(new CompileAction(
            new FileSelector(path), 
            compiler
        ));
        return prj;
    }
}