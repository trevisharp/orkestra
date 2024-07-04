/* Author:  Leonardo Trevisan Silio
 * Date:    04/07/2024
 */
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Orkestra.InternalStructure;

using Projects;
using Providers;
using Exceptions;
using LineInterfaces;

internal class ReflectionHelper
{
    internal static CLI GetConfiguredCLI()
    {
        try
        {
            return construct(typeof(CLI), 
                    () => new DefaultCLI(GetConfiguredProject())
                ) as CLI;
        }
        catch (ManyDefinitionException)
        {
            throw new ManyCLIDefinitionException();
        }
        catch (MissingDefinitionException)
        {
            throw new MissingCLIDefinitionException();
        }
        catch (Exception ex)
        {
            throw new UnexpectedException(ex);
        }
    }

    internal static Project GetConfiguredProject()
    {
        try
        {
            return construct(typeof(Project), 
                () => Tech.DefaultProject ?? 
                    Project.CreateDefault(".code", GetConfiguredCompiler())
                ) as Project;
        }
        catch (ManyDefinitionException)
        {
            System.Console.WriteLine("dfsiadfj2");
            throw new ManyProjectDefinitionException();
        }
        catch (MissingDefinitionException)
        {
            System.Console.WriteLine("dfsiadfj");
            throw new MissingProjectDefinitionException();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine("dfsiadfj3");
            throw new UnexpectedException(ex);
        }
    }
    
    internal static Compiler GetConfiguredCompiler()
    {
        Compiler compiler = null;
        try
        {
            compiler = construct(typeof(Compiler)) as Compiler;
        }
        catch (ManyDefinitionException)
        {
            throw new ManyCompilerDefinitionException();
        }
        catch (MissingDefinitionException)
        {
            throw new MissingCompilerDefinitionException();
        }
        catch (Exception ex)
        {
            throw new UnexpectedException(ex);
        }

        compiler.Provider = GetConfiguredAlgorithmGroupProvider();
        compiler.Load();

        return compiler;
    }

    internal static IAlgorithmGroupProvider GetConfiguredAlgorithmGroupProvider()
    {
        try
        {
            var provider = construct(
                typeof(IAlgorithmGroupProvider), 
                () => new DefaultAlgorithmGroupProvider()
            ) as IAlgorithmGroupProvider;
            return provider;
        }
        catch (MissingDefinitionException)
        {
            return new DefaultAlgorithmGroupProvider();
        }
        catch (ManyDefinitionException)
        {
            throw new ManyAlgorithmGroupProviderDefinitionException();
        }
    }

    internal static Compiler GetCompilerByType<T>()
        where T : Compiler, new()
    {
        var compiler = new T {
            Provider = GetConfiguredAlgorithmGroupProvider()
        };
        compiler.Load();

        return compiler;
    }

    private static object construct(Type matchType, Func<object> defaultValue = null)
    {
        List<Type> findedTypes = [];

        var types = getAssemplyTypes();
        foreach (var type in types)
        {
            var baseType = type.BaseType;
            if (baseType is null)
                continue;
            
            if (type.GetCustomAttribute<IgnoreAttribute>() is not null)
                continue;
            
            if (baseType != matchType)
                continue;
            
            findedTypes.Add(type);
        }

        if (findedTypes.Count == 1)
        {
            var constructor = getEmptyConstructor(findedTypes[0]);
            if (constructor is null)
                throw new NoConstructorException(matchType.Name);
            return constructor.Invoke([]);
        }

        if (findedTypes.Count > 1)
            throw new ManyDefinitionException();

        return defaultValue() ?? throw new MissingDefinitionException();
    }

    private static Type[] getAssemplyTypes()
    {
        var assembly = Assembly.GetEntryAssembly();
        var types = assembly.GetTypes();

        return types;
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