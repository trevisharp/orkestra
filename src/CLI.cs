/* Author:  Leonardo Trevisan Silio
 * Date:    12/03/2024
 */
using System;
using System.Reflection;
using System.Collections.Generic;

using static System.Console;

namespace Orkestra;

using Projects;

/// <summary>
/// Represents a command line interface implementation.
/// </summary>
public abstract class CLI
{
    public virtual string Header => "Running iteractive command line interface...";

    public void Run(params string[] args)
    {
        if (args.Length == 0)
        {
            startContinuousCLI();
            return;
        }
        
        call(args);
    }

    private void startContinuousCLI()
    {
        Verbose.Info(Header);
        while (true)
        {
            Verbose.Info(">> ");

            ForegroundColor = ConsoleColor.Magenta;
            var args = ReadLine()
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);
            ForegroundColor = ConsoleColor.Gray;

            Verbose.NewLine();
            
            if (args.Length == 0)
                continue;
            
            if (args[0].ToLower() == "exit")
                break;
            
            if (args[0].ToLower() == "clear")
                Clear();
            
            call(args);
        }
    }

    private void call(string[] args)
    {
        try
        {
            call(args[0], args[1..]);
        }
        catch (Exception ex)
        {
            Verbose.Error(ex.Message);
        }
    }

    private void call(string command, string[] otherArgs)
    {
        command = command.ToLower();
        var cliType = this.GetType();
        foreach (var method in cliType.GetRuntimeMethods())
        {
            if (method.Name.ToLower() != command)
                continue;
            
            var parameters = getParameters(
                method.GetParameters(), otherArgs
            );
            method.Invoke(this, parameters);
            return;
        }

        help();
    }

    private object[] getParameters(ParameterInfo[] parameterInfos, string[] args)
    {
        int parameterIndex = 0;
        var parameters = new object[parameterInfos.Length];

        foreach (var parameter in parameterInfos)
        {
            var value = args[parameterIndex];
            parameters[parameterIndex] =
                parameter.ParameterType.Name switch
                {
                    "Int32" => int.TryParse(value, out int result) ? result : throw getException(parameter, value),
                    "Single" => float.TryParse(value, out float result) ? result : throw getException(parameter, value),
                    _ => value
                };
        }

        return parameters;

        InvalidCastException getException(ParameterInfo parameter, string value)
        {
            return new InvalidCastException(
                $"The parameter named '{parameter.Name}' of type '{parameter.ParameterType.Name}' recive the value '{value}' resulting in a cast exception."
            );
        }
    }

    protected virtual void help()
    {
        Verbose.Info("help - Open this help screen.");
        Verbose.Info("clear - Clear the screen if the continuous CLI is opened.");
        Verbose.Info("exit - Exit of the continuous CLI is opened.");

        
    }
}