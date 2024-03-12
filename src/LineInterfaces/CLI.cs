/* Author:  Leonardo Trevisan Silio
 * Date:    12/03/2024
 */
using System;
using System.Reflection;

using static System.Console;

namespace Orkestra.LineInterfaces;

/// <summary>
/// Represents a command line interface implementation.
/// </summary>
public abstract class CLI
{
    public virtual string Header => "Running iteractive command line interface...";

    [IgnoreCommand]
    public void Run(params string[] args)
    {
        if (args.Length == 0)
        {
            startContinuousCLI();
            return;
        }
        
        call(args);
    }

    [IgnoreCommand]
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
            {
                Clear();
                continue;
            }
            
            call(args);
        }
    }

    [IgnoreCommand]
    private void call(string[] args)
    {
        try
        {
            call(args[0], args.Length > 1 ? args[1..] : []);
        }
        catch (Exception ex)
        {
            Verbose.Error(ex.Message);

            help();
        }
    }

    [IgnoreCommand]
    private void call(string command, string[] otherArgs)
    {
        command = command.ToLower();
        var cliType = this.GetType();
        foreach (var method in cliType.GetRuntimeMethods())
        {
            if (method.Name.ToLower() != command)
                continue;
            
            var ignore = method.GetCustomAttribute<IgnoreCommandAttribute>();
            if (ignore is not null)
                continue;
            
            var parameters = getParameters(
                method.GetParameters(), otherArgs
            );
            method.Invoke(this, parameters);
            return;
        }

        throw new MissingMethodException(
            "Missing commmand."
        );
    }

    [IgnoreCommand]
    private object[] getParameters(ParameterInfo[] parameterInfos, string[] args)
    {
        int parameterIndex = 0;
        var parameters = new object[parameterInfos.Length];

        foreach (var parameter in parameterInfos)
        {
            if (parameterIndex >= args.Length)
            {
                if (!parameter.HasDefaultValue)
                    throw new InvalidOperationException(
                        $"Missing parameter named '{parameter.Name}' of type '{parameter.ParameterType.Name}'."
                    );

                parameters[parameterIndex] = parameter.DefaultValue;
                continue;
            }

            var value = args[parameterIndex];
            parameters[parameterIndex] =
                parameter.ParameterType.Name switch
                {
                    "Int32" => int.TryParse(value, out int result) ? result : throw getException(parameter, value),
                    "Single" => float.TryParse(value, out float result) ? result : throw getException(parameter, value),
                    _ => value
                };
            parameterIndex++;
        }

        return parameters;

        InvalidCastException getException(ParameterInfo parameter, string value)
        {
            return new InvalidCastException(
                $"The parameter named '{parameter.Name}' of type '{parameter.ParameterType.Name}' recive the value '{value}' resulting in a cast exception."
            );
        }
    }

    protected virtual void help(string command = "")
    {
        bool isDetailed = command != string.Empty;

        var cliType = this.GetType();
        foreach (var method in cliType.GetRuntimeMethods())
        {
            if (method.DeclaringType != cliType)
                continue;

            var ignore = method.GetCustomAttribute<IgnoreCommandAttribute>();
            if (ignore is not null)
                continue;
            
            if (isDetailed && command.ToLower() != method.Name.ToLower())
                continue;

            var commandName = method.Name.ToLower();
            var helpMessage = 
                isDetailed ?
                method.GetCustomAttribute<DetailedHelpMessageAttribute>()?.Message :
                method.GetCustomAttribute<HelpMessageAttribute>()?.Message;

            Verbose.Info(
                helpMessage is not null ?
                $"{commandName} - {helpMessage}" :
                commandName
             );
        }

        Verbose.Info("help - Open this help screen.");
        Verbose.Info("clear - Clear the screen if the continuous CLI is opened.");
        Verbose.Info("exit - Exit of the continuous CLI is opened.");
    }
}