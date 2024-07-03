/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2024
 */
using System.Linq;
using System.Reflection;
using Orkestra.Projects;

namespace Orkestra.LineInterfaces;

public class DefaultCLI(Project project) : CLI
{
    [HelpMessage("Run the project.")]
    void run(params string[] args)
    {
        project.Compile(args);
    }

    [HelpMessage("Manage the extension.")]
    void extension(params string[] args)
    {
        if (args is [ "gen", .. ])
            project.GenerateExtension(args);
        else project.InstallExtension(args);
    }
}