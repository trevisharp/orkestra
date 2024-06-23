/* Author:  Leonardo Trevisan Silio
 * Date:    23/06/2024
 */
using System;
using System.Reflection;

namespace Orkestra.InternalStructure;

using Providers;
using Exceptions;
using LineInterfaces;

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
                throw new NoConstructorException();
            
            var cli = constructor.Invoke([]) as CLI;

            return cli;
        }
        
        throw new NoCLIExceptionsException();
    }

    internal static Compiler GetConfiguredCompiler<T>()
        where T : Compiler, new()
    {
        var types = getAssemplyTypes();
        var provider = getProvider<IAlgorithmGroupProvider>(
            types, new DefaultAlgorithmGroupProvider());

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