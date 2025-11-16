using LTRData.Extensions.Reflection;
using System;
#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP
using System.ComponentModel.DataAnnotations.Schema;
#endif
using System.Data;
using System.Reflection;
using System.Linq;
using LTRData.Data;

namespace LTRData.Extensions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public static class DataExtensions
{
#if NET40_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static T RecordToEntityObject<T>(this IDataRecord record) where T : new() => RecordToEntityObject(record, new T());

    public static T RecordToEntityObject<T>(this IDataRecord record, T obj)
    {
        var props = ExpressionSupport.PropertiesAssigners<T>.Setters;

        for (int i = 0, loopTo = record.FieldCount - 1; i <= loopTo; i++)
        {
            if (props.TryGetValue(record.GetName(i), out var prop))
            {
                prop(obj, record[i] is DBNull ? null : record[i]);
            }
        }

        return obj;
    }
#endif

#if NET461_OR_GREATER || NETSTANDARD || NETCOREAPP
    public static string GetColumnName(this MemberInfo memberInfo) => memberInfo.GetCustomAttribute<ColumnAttribute>()?.Name ?? memberInfo.Name;
#else
    public static string GetColumnName(this MemberInfo memberInfo) => memberInfo.Name;
#endif

    /// <summary>
    /// Returns the value of any EnumDescriptionAttribute object associated with
    /// an enumeration value
    /// </summary>
    /// <returns>Returns Description field of EnumDescriptionAttribute object if found,
    /// otherwise name of enumeration member is returned</returns>
    public static string GetEnumDescription(this Enum enumVar)
    {
        var enumMembers = enumVar.GetType().GetMember(enumVar.ToString());
        if (enumMembers.Length != 1)
        {
            return enumVar.ToString();
        }

        var AttrArray = enumMembers[0].GetCustomAttributes<EnumDescriptionAttribute>(false).ToArray();

        if (AttrArray.Length == 0)
        {
            return enumVar.ToString();
        }
        else
        {
            return AttrArray[0].Description;
        }
    }
}
