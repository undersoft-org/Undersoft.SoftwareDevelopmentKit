using System.ComponentModel;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

public static class EnumExtensions
{
    public static string ToDescriptionString<TEnum>(this TEnum val) where TEnum : Enum => typeof(TEnum).ToDescriptionString(val.ToString());

    public static string ToDescriptionString(this Type? type, string? fieldName)
    {
        var ret = string.Empty;
        if (type != null && !string.IsNullOrEmpty(fieldName))
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            var attributes = t.GetField(fieldName)?.GetCustomAttribute<DescriptionAttribute>(true);
            ret = attributes?.Description ?? fieldName;
        }
        return ret;
    }

    public static string ToDisplayName<TEnum>(this TEnum @enum) where TEnum : Enum => Utility.GetDisplayName(typeof(TEnum), @enum.ToString());

    public static List<SelectedItem> ToSelectList(this Type type, SelectedItem? addtionalItem = null)
    {
        var ret = new List<SelectedItem>();
        if (addtionalItem != null)
        {
            ret.Add(addtionalItem);
        }

        if (type.IsEnum())
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            foreach (var field in Enum.GetNames(t))
            {
                var desc = Utility.GetDisplayName(t, field);
                ret.Add(new SelectedItem(field, desc));
            }
        }
        return ret;
    }

    public static bool IsEnum(this Type? type)
    {
        var ret = false;
        if (type != null)
        {
            var t = Nullable.GetUnderlyingType(type) ?? type;
            ret = t.IsEnum;
        }
        return ret;
    }
}
