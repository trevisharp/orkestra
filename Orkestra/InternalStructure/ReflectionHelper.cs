/* Author:  Leonardo Trevisan Silio
 * Date:    05/01/2025
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
        CLI? cli = null;
        try
        {
            cli = Construct(typeof(CLI), 
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

        if (cli is null)
            throw new MissingCLIDefinitionException();
        
        return cli;
    }

    internal static Project GetConfiguredProject()
    {
        Project? project = null;
        try
        {
            project = Construct(typeof(Project), 
                () => Tech.DefaultProject ?? 
                    Project.CreateDefault(".code", GetConfiguredCompiler())
                ) as Project;
        }
        catch (ManyDefinitionException)
        {
            throw new ManyProjectDefinitionException();
        }
        catch (MissingDefinitionException)
        {
            throw new MissingProjectDefinitionException();
        }
        catch (Exception ex)
        {
            throw new UnexpectedException(ex);
        }

        if (project is null)
            throw new MissingProjectDefinitionException();
        
        return project;
    }
    
    internal static Compiler GetConfiguredCompiler()
    {
        Compiler? compiler = null;
        try
        {
            compiler = Construct(typeof(Compiler)) as Compiler;
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

        if (compiler is null)
            throw new MissingCompilerDefinitionException();

        compiler.Provider = GetConfiguredAlgorithmGroupProvider();
        compiler.Load();

        return compiler;
    }

    internal static IAlgorithmGroupProvider? GetConfiguredAlgorithmGroupProvider()
    {
        try
        {
            var provider = Construct(
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

    static object Construct(Type matchType, Func<object>? defaultValue = null)
    {
        List<Type> findedTypes = [];

        var types = GetAssemplyTypes();
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
            var constructor = GetEmptyConstructor(findedTypes[0]);
            if (constructor is null)
                throw new NoConstructorException(matchType.Name);
            return constructor.Invoke([]);
        }

        if (findedTypes.Count > 1)
            throw new ManyDefinitionException();

        if (defaultValue is null)
            throw new MissingDefinitionException();

        return defaultValue();
    }

    static Type[] GetAssemplyTypes()
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly is null)
            return [];
        
        var types = assembly.GetTypes();
        return types;
    }

    static ConstructorInfo? GetEmptyConstructor(Type type)
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