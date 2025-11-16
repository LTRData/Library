using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace LTRData.Extensions.Reflection;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class ReflectionExtensions
{
#if NET35_OR_GREATER || NETSTANDARD || NETCOREAPP

    /// <summary>
    /// Returns an IEnumerable(Of MethodInfo) object that enumerates static methods in an
    /// assembly.
    /// </summary>
    /// <param name="Assembly">Assembly to search</param>
    /// <param name="Name">Name of static method to search for</param>
    public static IEnumerable<MethodInfo> GetStaticMethods(this Assembly Assembly, string Name)
    {
        return from TypeDefinition in Assembly.GetTypes()
               let FoundMethod = TypeDefinition.GetMethod(Name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
               where FoundMethod is not null
               select FoundMethod;
    }

    /// <summary>
    /// Returns an IEnumerable(Of MethodInfo) object that enumerates static methods in an
    /// assembly.
    /// </summary>
    /// <param name="Assembly">Assembly to search</param>
    /// <param name="Name">Name of static method to search for</param>
    /// <param name="ParameterTypes">Types of parameters that searched method accepts</param>
    /// <param name="ReturnType">Return type from searched method</param>
    public static IEnumerable<MethodInfo> GetStaticMethods(this Assembly Assembly, string Name, Type[] ParameterTypes, Type ReturnType)
    {
        return from TypeDefinition in Assembly.GetTypes()
               let FoundMethod = TypeDefinition.GetMethod(Name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, ParameterTypes, null)
               where FoundMethod is not null && ReferenceEquals(FoundMethod.ReturnParameter.ParameterType, ReturnType)
               select FoundMethod;
    }

#endif

#if NET35_OR_GREATER && !NET45_OR_GREATER
    /// <summary>
    /// Returns a custom attribute object for a type or member.
    /// </summary>
    /// <typeparam name="T">Type of custom attribute to find</typeparam>
    /// <param name="MemberInfo">Type or member to search</param>
    /// <param name="Inherit">Search inherited custom attributes</param>
    /// <returns>Returns custom attribute object if found, otherwise Nothing is returned.</returns>
    public static T? GetCustomAttribute<T>(this MemberInfo MemberInfo, bool Inherit) where T : Attribute
    {
        var attribs = MemberInfo.GetCustomAttributes(typeof(T), Inherit);

        if (attribs is null || attribs.Length == 0)
        {
            return null;
        }

        return (T)attribs[0];
    }

    /// <summary>
    /// Returns an array of custom attribute objects for a type or member.
    /// </summary>
    /// <typeparam name="T">Type of custom attribute to find</typeparam>
    /// <param name="MemberInfo">Type or member to search</param>
    /// <param name="inherit">Search inherited custom attributes</param>
    public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo MemberInfo, bool inherit) where T : Attribute => (IEnumerable<T>)MemberInfo.GetCustomAttributes(typeof(T), inherit);
#endif

    /// <summary>
    /// Clones ICloneable object and returns clone cast to same type as source variable.
    /// </summary>
    /// <typeparam name="T">Type of source variable</typeparam>
    /// <param name="obj">Source object to clone</param>
    public static T CreateTypedClone<T>(this T obj) where T : ICloneable => (T)obj.Clone();
}

