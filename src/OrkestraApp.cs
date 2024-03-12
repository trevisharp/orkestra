/* Author:  Leonardo Trevisan Silio
 * Date:    12/03/2024
 */
using System;
using System.Reflection;

namespace Orkestra;

using Providers;
using Exceptions;
using Orkestra.LineInterfaces;

/// <summary>
/// Main Orkestra class framework.
/// </summary>
public static class OrkestraApp
{
    /// <summary>
    /// Compile a code based in a compiler class inside this assembly.
    /// </summary>
    /// <param name="sourceCode">The code to be compiled.</param>
    /// <param name="args">The compiler arguments.</param>
    public static void Compile(string sourceCode, params string[] args)
    {
        try
        {
            configureVerbose(args);

            var compiler = getConfiguredCompiler(args);
            compiler.Compile(sourceCode);
        }
        catch (OrkestraException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new UnexpectedException(ex);
        }
    }
    
    /// <summary>
    /// Run the Command Line Interface of this application.
    /// </summary>
    public static void Run(params string[] args)
    {
        try
        {
            var cli = getConfiguredCLI();
            cli.Run(args);
        }
        catch (OrkestraException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new UnexpectedException(ex);
        }
    }

    private static void configureVerbose(string[] args)
    {
        Verbose.VerboseLevel = 0;
        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg != "-v" && arg != "--verbose")
                continue;
            
            Verbose.VerboseLevel = 1;
            if (++i >= args.Length)
                break;
            
            arg = args[i];
            if (int.TryParse(arg, out int level))
                Verbose.VerboseLevel = level;
            else if (arg == "max")
                Verbose.VerboseLevel = int.MaxValue;
        }

    }

    private static CLI getConfiguredCLI()
    {
        var types = getAssemplyTypes();
        foreach (var type in types)
        {
            if (type.BaseType != typeof(CLI))
                continue;
            
            var constructor = getEmptyConstructor(type);
            
            if (constructor is null)
                throw new NoConstructorException();
            
            var cli = constructor.Invoke([]) as CLI;

            return cli;
        }
        
        throw new NoCLIExceptionsException();
    }

    private static Compiler getConfiguredCompiler(string[] args)
    {
        var types = getAssemplyTypes();
        var compiler = getCompiler(types);

        var provider = getProvider(types);
        compiler.Provider = provider;
        return compiler;
    }

    private static Compiler getCompiler(Type[] types)
    {
        var compilerType = getCompilerType(types);
        if (compilerType is null)
            throw new NoCompilerExceptionsException();

        var compiler = createCompiler(compilerType);

        return compiler;
    }

    private static IAlgorithmGroupProvider getProvider(Type[] types)
    {
        var providerType = getProviderType(types);

        var provider = providerType is null ?
            new DefaultAlgorithmGroupProvider() :
            createProvider(providerType);
        
        return provider;
    }

    private static Type[] getAssemplyTypes()
    {
        var assembly = Assembly.GetEntryAssembly();
        var types = assembly.GetTypes();

        return types;
    }

    private static Type getCompilerType(Type[] types)
    {
        foreach (var type in types)
        {
            if (type.BaseType != typeof(Compiler))
                continue;
            
            var ignoreAttribute = type.GetCustomAttribute<IgnoreAttribute>();
            if (ignoreAttribute is not null)
                continue;
            
            return type;
        }

        return null;
    }

    private static Compiler createCompiler(Type type)
    {
        var constructor = getEmptyConstructor(type);
        
        if (constructor is null)
            throw new NoConstructorException();
        
        var compiler = constructor.Invoke(new object[0]) as Compiler;

        return compiler;
    }

    private static Type getProviderType(Type[] types)
    {
        foreach (var type in types)
        {
            if (type.BaseType != typeof(IAlgorithmGroupProvider))
                continue;
            
            var ignoreAttribute = type.GetCustomAttribute<IgnoreAttribute>();
            if (ignoreAttribute is not null)
                continue;
            
            return type;
        }

        return null;
    }

    private static IAlgorithmGroupProvider createProvider(Type type)
    {
        var constructor = getEmptyConstructor(type);
        
        if (constructor is null)
            throw new NoConstructorException();
        
        var compiler = constructor.Invoke(new object[0]) as IAlgorithmGroupProvider;

        return compiler;
    }

    private static ConstructorInfo getEmptyConstructor(Type type)
    {
        var constructors = type.GetConstructors();

        foreach (var constructor in constructors)
        {
            if (constructor.GetParameters().Length == 0)
                return constructor;
        }

        return null;
    }
}