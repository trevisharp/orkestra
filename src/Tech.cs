/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2024
 */
using System;

namespace Orkestra;

using Projects;
using LineInterfaces;
using InternalStructure;

/// <summary>
/// Geral class to configuration and application managment.
/// </summary>
public static class Tech
{
    internal static Project DefaultProject = null;
    public static void ConfigureProject(string fileSelector, Compiler compiler = null)
    {
        if (fileSelector is null)
            throw new ArgumentNullException(nameof(fileSelector));
        
        DefaultProject = Project.CreateDefault(fileSelector, compiler);
    }

    public static void Run(params string[] args)
    {
        var cli = ReflectionHelper.GetConfiguredCLI();
        cli.ReciveCommand(args);
    }
}