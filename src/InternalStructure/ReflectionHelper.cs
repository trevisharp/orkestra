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
using System.Diagnostics.Contracts;

internal class ReflectionHelper
{
    internal static CLI GetConfiguredCLI()
    {
        var types = getAssemplyTypes();
        foreach (var type in types)
        {
            if (type.BaseType != typeof(CLI))
                continue;
            
            var constructor = getEmptyConstructor(type);
            
            if (constructor is null)
                throw new NoConstructorException("CLI");
            
            var cli = constructor.Invoke([]) as CLI;

            return cli;
        }
        
        return new DefaultCLI(
            GetConfiguredProject()
        );
    }

    internal static Project GetConfiguredProject()
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
            
            if (baseType != typeof(Project))
                continue;
            
            findedTypes.Add(baseType);
        }

        if (findedTypes.Count == 1)
        {
            var constructor = getEmptyConstructor(findedTypes[0]);
            if (constructor is null)
                throw new NoConstructorException("Project");
            return constructor.Invoke([]) as Project;
        }

        if (findedTypes.Count > 1)
            throw new ManyProjectDefinitionException();
        
        var compiler = GetConfiguredCompiler<>();

        return ;
    }
    
    internal static Compiler GetConfiguredCompiler<T>()
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
            throw new NoConstructorException();
        
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