/* Author:  Leonardo Trevisan Silio
 * Date:    30/06/2023
 */
using System;
using System.Reflection;

namespace Orkestra;

using Exceptions;

/// <summary>
/// Main Orkestra class framework.
/// </summary>
public static class Orkestra
{
    public static void Run(string[] args, string sourceCode)
    {
        try
        {
            var types = getAssemplyTypes();

            var compilerType = getCompilerType(types);
            if (compilerType is null)
                throw new NoCompilerExceptionsException();

            var compiler = createCompiler(compilerType);

            
        }
        catch (OrkestraException ex)
        {
            throw ex;
        }
        catch (Exception ex)
        {
            throw new UnexpectedException(ex);
        }
    }

    public static void Run(string[] args)
    {

    }

    private static Type[] getAssemplyTypes()
    {
        var assembly = Assembly.GetCallingAssembly();
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