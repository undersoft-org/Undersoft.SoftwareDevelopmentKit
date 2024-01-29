using System.ComponentModel;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

public static class DynamicObjectContextExtensions
{
    public static void AddRequiredAttribute(this DynamicObjectContext context, string columnName, string? errorMessage = null, bool allowEmptyStrings = false)
    {
        var parameters = new KeyValuePair<string, object?>[]
        {
                new(nameof(RequiredAttribute.ErrorMessage), errorMessage),
                new(nameof(RequiredAttribute.AllowEmptyStrings), allowEmptyStrings)
        };
        context.AddMultipleParameterAttribute<RequiredAttribute>(columnName, parameters);
    }

    public static void AddAutoGenerateColumnAttribute(this DynamicObjectContext context, string columnName, IEnumerable<KeyValuePair<string, object?>> parameters) => context.AddMultipleParameterAttribute<AutoGenerateColumnAttribute>(columnName, parameters);

    public static void AddDisplayAttribute(this DynamicObjectContext context, string columnName, IEnumerable<KeyValuePair<string, object?>> parameters) => context.AddMultipleParameterAttribute<DisplayAttribute>(columnName, parameters);

    public static void AddMultipleParameterAttribute<TAttribute>(this DynamicObjectContext context, string columnName, IEnumerable<KeyValuePair<string, object?>> parameters) where TAttribute : Attribute
    {
        var type = typeof(TAttribute);
        var propertyInfos = new List<PropertyInfo>();
        var propertyValues = new List<object?>();
        foreach (var kv in parameters)
        {
            var pInfo = type.GetProperty(kv.Key);
            if (pInfo != null)
            {
                propertyInfos.Add(pInfo);
                propertyValues.Add(kv.Value);
            }
        }
        context.AddAttribute(columnName, type, Type.EmptyTypes, Array.Empty<object>(), propertyInfos.ToArray(), propertyValues.ToArray());
    }

    public static void AddDisplayNameAttribute(this DynamicObjectContext context, string columnName, string displayName) => context.AddAttribute<DisplayNameAttribute>(columnName, new Type[] { typeof(string) }, new object?[] { displayName });

    public static void AddDescriptionAttribute(this DynamicObjectContext context, string columnName, string description) => context.AddAttribute<DescriptionAttribute>(columnName, new Type[] { typeof(string) }, new object?[] { description });

    public static void AddAttribute<TAttribute>(this DynamicObjectContext context, string columnName, Type[] types, object?[] constructorArgs, PropertyInfo[]? propertyInfos = null, object?[]? propertyValues = null) where TAttribute : Attribute
    {
        var type = typeof(TAttribute);
        context.AddAttribute(columnName, type, types, constructorArgs, propertyInfos, propertyValues);
    }

    public static async Task SetValue(this IDynamicObjectContext context, object model)
    {
        if (model is IDynamicObject v)
        {
            var item = context.GetItems().FirstOrDefault(i => i.DynamicObjectPrimaryKey == v.DynamicObjectPrimaryKey);
            if (item != null && context.OnValueChanged != null)
            {
                foreach (var col in context.GetColumns())
                {
                    await context.OnValueChanged(item, col, v.GetValue(col.GetFieldName()));
                }
            }
        }
    }
}
