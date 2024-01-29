using Undersoft.SDK.Service.Application.Localization.Json;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Localization;
using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

public static class Utility
{
    public static string GetDisplayName(object model, string fieldName) => GetDisplayName(model.GetType(), fieldName);

    public static string GetDisplayName(Type modelType, string fieldName) => CacheManager.GetDisplayName(Nullable.GetUnderlyingType(modelType) ?? modelType, fieldName);

    public static List<SelectedItem> GetNullableBoolItems(object model, string fieldName) => GetNullableBoolItems(model.GetType(), fieldName);

    public static List<SelectedItem> GetNullableBoolItems(Type modelType, string fieldName) => CacheManager.GetNullableBoolItems(modelType, fieldName);

    public static TValue? GetKeyValue<TModel, TValue>(TModel model, Type? customAttribute = null) => CacheManager.GetKeyValue<TModel, TValue>(model, customAttribute);

    public static TResult GetPropertyValue<TModel, TResult>(TModel model, string fieldName) => CacheManager.GetPropertyValue<TModel, TResult>(model, fieldName);

    public static object? GetPropertyValue(object model, string fieldName)
    {
        return model.GetType().Assembly.IsDynamic ? ReflectionInvoke() : LambdaInvoke();

        object? ReflectionInvoke()
        {
            object? ret = null;
            var propertyInfo = model.GetType().GetRuntimeProperties().FirstOrDefault(i => i.Name == fieldName);
            if (propertyInfo != null)
            {
                ret = propertyInfo.GetValue(model);
            }
            return ret;
        }

        object? LambdaInvoke() => GetPropertyValue<object, object?>(model, fieldName);
    }

    public static void SetPropertyValue<TModel, TValue>(TModel model, string fieldName, TValue value) => CacheManager.SetPropertyValue(model, fieldName, value);

    public static Func<IEnumerable<T>, string, SortOrder, IEnumerable<T>> GetSortFunc<T>() => CacheManager.GetSortFunc<T>();

    public static Func<IEnumerable<T>, List<string>, IEnumerable<T>> GetSortListFunc<T>() => CacheManager.GetSortListFunc<T>();

    public static IEnumerable<LocalizedString> GetJsonStringByTypeName(JsonLocalizationOptions option, Assembly assembly, string typeName, string? cultureName = null, bool forceLoad = false) => CacheManager.GetJsonStringByTypeName(option, assembly, typeName, cultureName, forceLoad) ?? Enumerable.Empty<LocalizedString>();

    public static IStringLocalizer? GetStringLocalizerFromService(Assembly assembly, string typeName) => CacheManager.GetStringLocalizerFromService(assembly, typeName);

    public static string? GetPlaceHolder<TModel>(string fieldName) => GetPlaceHolder(typeof(TModel), fieldName);

    public static string? GetPlaceHolder(object model, string fieldName) => GetPlaceHolder(model.GetType(), fieldName);

    public static string? GetPlaceHolder(Type modelType, string fieldName) => modelType.Assembly.IsDynamic
        ? null
        : CacheManager.GetPlaceholder(modelType, fieldName);

    public static bool TryGetProperty(Type modelType, string fieldName, [NotNullWhen(true)] out PropertyInfo? propertyInfo) => CacheManager.TryGetProperty(modelType, fieldName, out propertyInfo);

    public static void Reset<TModel>(TModel source) where TModel : class, new()
    {
        var v = new TModel();
        foreach (var pi in source.GetType().GetRuntimeProperties().Where(p => p.CanWrite))
        {
            var pinfo = v.GetType().GetPropertyByName(pi.Name);
            if (pinfo != null)
            {
                pi.SetValue(source, pinfo.GetValue(v));
            }
        }
    }

    public static TModel Clone<TModel>(TModel item)
    {
        var ret = item;
        if (item != null)
        {
            if (item is ICloneable cloneable)
            {
                ret = (TModel)cloneable.Clone();
            }
            else
            {
                var type = item.GetType();
                if (type.IsClass)
                {
                    var instance = Activator.CreateInstance(type);
                    if (instance != null)
                    {
                        ret = (TModel)instance;
                        if (ret != null)
                        {
                            var valType = ret.GetType();

                            foreach (var f in type.GetFields())
                            {
                                var v = f.GetValue(item);
                                var field = valType.GetField(f.Name)!;
                                field.SetValue(ret, v);
                            };
                            foreach (var p in type.GetRuntimeProperties())
                            {
                                if (p.CanWrite)
                                {
                                    var v = p.GetValue(item);
                                    var property = valType.GetRuntimeProperties().First(i => i.Name == p.Name && i.PropertyType == p.PropertyType);
                                    property.SetValue(ret, v);
                                }
                            };
                        }
                    }
                }
            }
        }
        return ret;
    }

    public static void Copy<TModel>(TModel source, TModel destination) where TModel : class
    {
        var type = source.GetType();
        var valType = destination.GetType();
        if (valType != null)
        {
            foreach (var f in type.GetFields())
            {
                var v = f.GetValue(source);
                var field = valType.GetField(f.Name)!;
                field.SetValue(destination, v);
            }
            foreach (var p in type.GetRuntimeProperties())
            {
                if (p.CanWrite)
                {
                    var v = p.GetValue(source);
                    var property = valType.GetRuntimeProperties().First(i => i.Name == p.Name && i.PropertyType == p.PropertyType);
                    property.SetValue(destination, v);
                }
            }
        }
    }

    #region GenerateColumns

    public static IEnumerable<ITableColumn> GetTableColumns<TModel>(IEnumerable<ITableColumn>? source = null) => GetTableColumns(typeof(TModel), source);

    public static IEnumerable<ITableColumn> GetTableColumns(Type type, IEnumerable<ITableColumn>? source = null)
    {
        var cols = new List<ITableColumn>(50);
        var attrModel = type.GetCustomAttribute<AutoGenerateClassAttribute>(true);
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            ITableColumn? tc;
            var attr = prop.GetCustomAttribute<AutoGenerateColumnAttribute>(true);

            var displayName = attr?.Text ?? Utility.GetDisplayName(type, prop.Name);
            if (attr == null)
            {
                tc = new InternalTableColumn(prop.Name, prop.PropertyType, displayName);

                if (attrModel != null)
                {
                    tc.InheritValue(attrModel);
                }
            }
            else
            {
                if (attr.Ignore) continue;

                attr.Text = displayName;
                attr.FieldName = prop.Name;
                attr.PropertyType = prop.PropertyType;

                if (attrModel != null)
                {
                    attr.InheritValue(attrModel);
                }
                tc = attr;
            }

            var col = source?.FirstOrDefault(c => c.GetFieldName() == tc.GetFieldName());
            if (col != null)
            {
                tc.CopyValue(col);
            }
            cols.Add(tc);
        }

        return cols.Where(a => a.Order > 0).OrderBy(a => a.Order)
            .Concat(cols.Where(a => a.Order == 0))
            .Concat(cols.Where(a => a.Order < 0).OrderBy(a => a.Order));
    }

    public static IEnumerable<ITableColumn> GenerateColumns<TModel>(Func<ITableColumn, bool> predicate) => Utility.GetTableColumns<TModel>().Where(predicate);

    public static void CreateDisplayByFieldType(this RenderTreeBuilder builder, IEditorItem item, object model)
    {
        var fieldType = item.PropertyType;
        var fieldName = item.GetFieldName();
        var displayName = item.GetDisplayName() ?? GetDisplayName(model, fieldName);
        var fieldValue = GenerateValue(model, fieldName);
        var type = (Nullable.GetUnderlyingType(fieldType) ?? fieldType);
        if (type == typeof(bool) || fieldValue?.GetType() == typeof(bool))
        {
            builder.OpenComponent<Switch>(0);
            builder.AddAttribute(1, nameof(Switch.Value), fieldValue);
            builder.AddAttribute(2, nameof(Switch.IsDisabled), true);
            builder.AddAttribute(3, nameof(Switch.DisplayText), displayName);
            builder.AddAttribute(4, nameof(Switch.ShowLabelTooltip), item.ShowLabelTooltip);
            builder.CloseComponent();
        }
        else if (item.ComponentType == typeof(Textarea))
        {
            builder.OpenComponent(0, typeof(Textarea));
            builder.AddAttribute(1, nameof(Textarea.DisplayText), displayName);
            builder.AddAttribute(2, nameof(Textarea.Value), fieldValue);
            builder.AddAttribute(3, nameof(Textarea.ShowLabelTooltip), item.ShowLabelTooltip);
            builder.AddAttribute(4, "readonly", true);
            if (item.Rows > 0)
            {
                builder.AddAttribute(5, "rows", item.Rows);
            }
            builder.CloseComponent();
        }
        else
        {
            builder.OpenComponent(0, typeof(Display<>).MakeGenericType(fieldType));
            builder.AddAttribute(1, nameof(Display<string>.DisplayText), displayName);
            builder.AddAttribute(2, nameof(Display<string>.Value), fieldValue);
            builder.AddAttribute(3, nameof(Display<string>.LookupServiceKey), item.LookupServiceKey);
            builder.AddAttribute(4, nameof(Display<string>.ShowLabelTooltip), item.ShowLabelTooltip);
            builder.CloseComponent();
        }
    }

    public static void CreateComponentByFieldType(this RenderTreeBuilder builder, ComponentBase component, IEditorItem item, object model, ItemChangedType changedType = ItemChangedType.Update, bool isSearch = false, ILookupService? lookUpService = null)
    {
        var fieldType = item.PropertyType;
        var fieldName = item.GetFieldName();
        var displayName = item.GetDisplayName() ?? GetDisplayName(model, fieldName);

        var fieldValue = GenerateValue(model, fieldName);
        var fieldValueChanged = GenerateValueChanged(component, model, fieldName, fieldType);
        var valueExpression = GenerateValueExpression(model, fieldName, fieldType);
        var lookup = item.Lookup ?? lookUpService?.GetItemsByKey(item.LookupServiceKey);
        var componentType = item.ComponentType ?? GenerateComponentType(fieldType, item.Rows != 0, lookup);
        builder.OpenComponent(0, componentType);
        if (componentType.IsSubclassOf(typeof(ValidateBase<>).MakeGenericType(fieldType)))
        {
            builder.AddAttribute(1, nameof(ValidateBase<string>.DisplayText), displayName);
            builder.AddAttribute(2, nameof(ValidateBase<string>.Value), fieldValue);
            builder.AddAttribute(3, nameof(ValidateBase<string>.ValueChanged), fieldValueChanged);
            builder.AddAttribute(4, nameof(ValidateBase<string>.ValueExpression), valueExpression);

            if (!item.CanWrite(model.GetType(), changedType, isSearch))
            {
                builder.AddAttribute(5, nameof(ValidateBase<string>.IsDisabled), true);
            }

            if (item.ValidateRules != null)
            {
                builder.AddAttribute(6, nameof(ValidateBase<string>.ValidateRules), item.ValidateRules);
            }

            if (item.ShowLabelTooltip != null)
            {
                builder.AddAttribute(7, nameof(ValidateBase<string>.ShowLabelTooltip), item.ShowLabelTooltip);
            }
        }

        if (componentType == typeof(NullSwitch) && TryGetProperty(model.GetType(), fieldName, out var propertyInfo))
        {
            var defaultValueAttr = propertyInfo.GetCustomAttribute<DefaultValueAttribute>();
            if (defaultValueAttr != null)
            {
                var dv = defaultValueAttr.Value is bool v && v;
                builder.AddAttribute(8, nameof(NullSwitch.DefaultValueWhenNull), dv);
            }
        }

        if (IsCheckboxList(fieldType, componentType) && item.Items != null)
        {
            builder.AddAttribute(9, nameof(CheckboxList<IEnumerable<string>>.Items), item.Items.Clone());
        }

        if (item.ComponentType == typeof(Select<bool?>) && fieldType == typeof(bool?) && lookup == null && item.Items == null)
        {
            builder.AddAttribute(10, nameof(Select<bool?>.Items), GetNullableBoolItems(model, fieldName));
        }

        if (lookup != null && item.Items == null)
        {
            builder.AddAttribute(11, nameof(Select<SelectedItem>.ShowSearch), item.ShowSearchWhenSelect);
            builder.AddAttribute(12, nameof(Select<SelectedItem>.Items), lookup.Clone());
            builder.AddAttribute(13, nameof(Select<SelectedItem>.StringComparison), item.LookupStringComparison);
        }

        if (item.Items != null && item.ComponentType == typeof(Select<>).MakeGenericType(fieldType))
        {
            builder.AddAttribute(14, nameof(Select<SelectedItem>.Items), item.Items.Clone());
            builder.AddAttribute(15, nameof(Select<SelectedItem>.ShowSearch), item.ShowSearchWhenSelect);
        }

        if (IsValidatableComponent(componentType))
        {
            builder.AddAttribute(16, nameof(IEditorItem.SkipValidate), item.SkipValidate);
        }

        builder.AddMultipleAttributes(17, CreateMultipleAttributes(fieldType, model, fieldName, item));

        if (item.ComponentParameters != null)
        {
            builder.AddMultipleAttributes(18, item.ComponentParameters);
        }

        if (componentType.GetPropertyByName(nameof(Select<string>.IsPopover)) != null)
        {
            builder.AddAttribute(19, nameof(Select<string>.IsPopover), item.IsPopover);
        }
        builder.CloseComponent();
    }

    private static List<SelectedItem> Clone(this IEnumerable<SelectedItem> source) => source.Select(d => new SelectedItem(d.Value, d.Text)
    {
        Active = d.Active,
        IsDisabled = d.IsDisabled,
        GroupName = d.GroupName
    }).ToList();

    private static object? GenerateValue(object model, string fieldName) => Utility.GetPropertyValue<object, object?>(model, fieldName);

    public static object GenerateValueExpression(object model, string fieldName, Type fieldType)
    {
        var type = model.GetType();
        return fieldName.Contains('.') ? ComplexPropertyValueExpression() : SimplePropertyValueExpression();

        object SimplePropertyValueExpression()
        {
            var pi = type.GetPropertyByName(fieldName) ?? throw new InvalidOperationException($"the model {type.Name} not found the property {fieldName}");
            var body = Expression.Property(Expression.Constant(model), pi);
            var tDelegate = typeof(Func<>).MakeGenericType(fieldType);
            return Expression.Lambda(tDelegate, body);
        }

        object ComplexPropertyValueExpression()
        {
            var propertyNames = fieldName.Split(".");
            Expression? body = null;
            Type t = type;
            object? propertyInstance = model;
            foreach (var name in propertyNames)
            {
                var p = t.GetPropertyByName(name) ?? throw new InvalidOperationException($"the model {model.GetType().Name} not found the property {fieldName}");
                propertyInstance = p.GetValue(propertyInstance);
                if (propertyInstance != null)
                {
                    t = propertyInstance.GetType();
                }
                if (body == null)
                {
                    body = Expression.Property(Expression.Convert(Expression.Constant(model), type), p);
                }
                else
                {
                    body = Expression.Property(body, p);
                }
            }
            var tDelegate = typeof(Func<>).MakeGenericType(fieldType);
            return Expression.Lambda(tDelegate, body!);
        }
    }

    private static Type GenerateComponentType(Type fieldType, bool hasRows, IEnumerable<SelectedItem>? lookup)
    {
        Type? ret = null;
        var type = (Nullable.GetUnderlyingType(fieldType) ?? fieldType);
        if (type.IsEnum || lookup != null)
        {
            ret = typeof(Select<>).MakeGenericType(fieldType);
        }
        else if (IsCheckboxList(type))
        {
            ret = typeof(CheckboxList<IEnumerable<string>>);
        }
        else if (fieldType == typeof(bool?))
        {
            ret = typeof(NullSwitch);
        }
        else
        {
            switch (type.Name)
            {
                case nameof(Boolean):
                    ret = typeof(Switch);
                    break;
                case nameof(DateTime):
                    ret = typeof(DateTimePicker<>).MakeGenericType(fieldType);
                    break;
                case nameof(Int16):
                case nameof(Int32):
                case nameof(Int64):
                case nameof(Single):
                case nameof(Double):
                case nameof(Decimal):
                    ret = typeof(BootstrapInputNumber<>).MakeGenericType(fieldType);
                    break;
                case nameof(String):
                    if (hasRows)
                    {
                        ret = typeof(Textarea);
                    }
                    else
                    {
                        ret = typeof(BootstrapInput<>).MakeGenericType(typeof(string));
                    }
                    break;
            }
        }
        return ret ?? typeof(BootstrapInput<>).MakeGenericType(fieldType);
    }

    private static bool IsCheckboxList(Type fieldType, Type? componentType = null)
    {
        var ret = false;
        if (componentType != null)
        {
            ret = componentType.IsGenericType && componentType.GetGenericTypeDefinition() == typeof(CheckboxList<>);
        }
        if (!ret)
        {
            var type = Nullable.GetUnderlyingType(fieldType) ?? fieldType;
            ret = type.IsAssignableTo(typeof(IEnumerable<string>));
        }
        return ret;
    }

    private static bool IsValidatableComponent(Type componentType) => componentType.GetProperties().FirstOrDefault(p => p.Name == nameof(IEditorItem.SkipValidate)) != null;

    private static IEnumerable<KeyValuePair<string, object>> CreateMultipleAttributes(Type fieldType, object model, string fieldName, IEditorItem item)
    {
        var ret = new List<KeyValuePair<string, object>>();
        var type = Nullable.GetUnderlyingType(fieldType) ?? fieldType;
        switch (type.Name)
        {
            case nameof(String):
                var ph = item.PlaceHolder ?? Utility.GetPlaceHolder(model, fieldName);
                if (ph != null)
                {
                    ret.Add(new("placeholder", ph));
                }
                if (item.Rows != 0)
                {
                    ret.Add(new("rows", item.Rows));
                }
                break;
            case nameof(Int16):
            case nameof(Int32):
            case nameof(Int64):
            case nameof(Single):
            case nameof(Double):
            case nameof(Decimal):
                if (item.Step != null)
                {
                    var step = item.Step.ToString();
                    if (!string.IsNullOrEmpty(step))
                    {
                        ret.Add(new("Step", step));
                    }
                }
                break;
            default:
                break;
        }
        return ret;
    }

    private static Func<TType, Task> CreateOnValueChangedCallback<TModel, TType>(TModel model, ITableColumn col, Func<TModel, ITableColumn, object?, Task> callback) => new(v => callback(model, col, v));

    public static Expression<Func<TModel, ITableColumn, Func<TModel, ITableColumn, object?, Task>, object>> CreateOnValueChanged<TModel>(Type fieldType)
    {
        var method = typeof(Utility).GetMethod(nameof(CreateOnValueChangedCallback), BindingFlags.Static | BindingFlags.NonPublic)!.MakeGenericMethod(typeof(TModel), fieldType);
        var exp_p1 = Expression.Parameter(typeof(TModel));
        var exp_p2 = Expression.Parameter(typeof(ITableColumn));
        var exp_p3 = Expression.Parameter(typeof(Func<,,,>).MakeGenericType(typeof(TModel), typeof(ITableColumn), typeof(object), typeof(Task)));
        var body = Expression.Call(null, method, exp_p1, exp_p2, exp_p3);

        return Expression.Lambda<Func<TModel, ITableColumn, Func<TModel, ITableColumn, object?, Task>, object>>(Expression.Convert(body, typeof(object)), exp_p1, exp_p2, exp_p3);
    }

    public static Func<TModel, ITableColumn, Func<TModel, ITableColumn, object?, Task>, object> GetOnValueChangedInvoke<TModel>(Type fieldType) => CacheManager.GetOnValueChangedInvoke<TModel>(fieldType);
    #endregion

    #region Format
    public static string Format(object? source, string format, IFormatProvider? provider = null)
    {
        var ret = string.Empty;
        if (source != null)
        {
            var invoker = CacheManager.GetFormatInvoker(source.GetType());
            ret = invoker(source, format, provider);
        }
        return ret;
    }

    public static string Format(object? source, IFormatProvider provider)
    {
        var ret = string.Empty;
        if (source != null)
        {
            var invoker = CacheManager.GetFormatProviderInvoker(source.GetType());
            ret = invoker(source, provider);
        }
        return ret;
    }
    #endregion

    public static string? ConvertValueToString<TValue>(TValue value)
    {
        var ret = "";
        var typeValue = typeof(TValue);
        if (typeValue == typeof(string))
        {
            ret = value!.ToString();
        }
        else if (typeValue.IsGenericType || typeValue.IsArray)
        {
            var t = typeValue.IsGenericType ? typeValue.GenericTypeArguments[0] : typeValue.GetElementType();
            if (t != null)
            {
                var instance = Activator.CreateInstance(typeof(List<>).MakeGenericType(t));
                if (instance != null)
                {
                    var mi = instance.GetType().GetMethod(nameof(List<string>.AddRange));
                    if (mi != null)
                    {
                        mi.Invoke(instance, new object?[] { value });
                        var invoker = CacheManager.CreateConverterInvoker(t);
                        var v = invoker.Invoke(instance);
                        ret = string.Join(",", v);
                    }
                }
            }
        }
        return ret;
    }

    public static object? GenerateValueChanged(ComponentBase component, object model, string fieldName, Type fieldType)
    {
        var valueChangedInvoker = CreateLambda(fieldType).Compile();
        return valueChangedInvoker(component, model, fieldName);

        static Expression<Func<ComponentBase, object, string, object>> CreateLambda(Type fieldType)
        {
            var exp_p1 = Expression.Parameter(typeof(ComponentBase));
            var exp_p2 = Expression.Parameter(typeof(object));
            var exp_p3 = Expression.Parameter(typeof(string));
            var method = typeof(Utility).GetMethod(nameof(CreateCallback), BindingFlags.Static | BindingFlags.NonPublic)!.MakeGenericMethod(fieldType);
            var body = Expression.Call(null, method, exp_p1, exp_p2, exp_p3);

            return Expression.Lambda<Func<ComponentBase, object, string, object>>(Expression.Convert(body, typeof(object)), exp_p1, exp_p2, exp_p3);
        }
    }

    private static EventCallback<TType> CreateCallback<TType>(ComponentBase component, object model, string fieldName) => EventCallback.Factory.Create<TType>(component, t => CacheManager.SetPropertyValue(model, fieldName, t));

    public static IEnumerable<IEditorItem> GenerateEditorItems<TModel>(IEnumerable<ITableColumn>? source = null) => Utility.GetTableColumns<TModel>(source);

    public static IStringLocalizer? CreateLocalizer<TType>() => CreateLocalizer(typeof(TType));

    public static IStringLocalizer? CreateLocalizer(Type type) => CacheManager.CreateLocalizerByType(type);
}
