/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2024
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
            var defaultProject = Tech.DefaultProject 
                ?? Project.CreateDefault(".code", GetConfiguredCompiler());
            return construct(typeof(Project), () => defaultProject) as Project;
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
    }
    
    internal static Compiler GetConfiguredCompiler()
    {
        try
        {
            return construct(typeof(Compiler)) as Compiler;
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
    }

    internal static Compiler GetCompilerByType<T>()
        where T : Compiler, new()
    {
        var types = getAssemplyTypes();
        var provider = getProvider<IAlgorithmGroupProvider>(
            types, new DefaultAlgorithmGroupProvider()
        );

        var compiler = new T {
            Provider = provider
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
            
            findedTypes.Add(baseType);
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

    private static T getProvider<T>(Type[] types, T defaultValue)
    {
        var providerType = getProviderType<T>(types);

        var provider = providerType is null ?
            defaultValue :
            createProvider<T>(providerType);
        
        return provider;
    }

    private static Type getProviderType<T>(Type[] types)
    {
        foreach (var type in types)
        {
            if (type.BaseType != typeof(T))
                continue;
            
            var ignoreAttribute = type.GetCustomAttribute<IgnoreAttribute>();
            if (ignoreAttribute is not null)
                continue;
            
            return type;
        }

        return null;
    }

    private static T createProvider<T>(Type type)
    {
        var constructor = getEmptyConstructor(type);
        
        if (constructor is null)
            throw new NoConstructorException(type.Name);
        
        var compiler = constructor.Invoke([]);

        return (T)compiler;
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