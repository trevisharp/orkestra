/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2024
 */
using System;
using Orkestra.Projects;
using Orkestra.LineInterfaces;

namespace Orkestra;

public static class Tech
{
    internal static Project defaultProject = null;
    public static void ConfigureProject(string fileSelector)
    {
        if (fileSelector is null)
            throw new ArgumentNullException(nameof(fileSelector));
        
        defaultProject = DefaultProject.Create(fileSelector, null);
    }

    public static void Run(params string[] args)
    {
        CLI.Run(args);
    }
}