using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Display<TValue>
{
    private string? ClassString => CssBuilder.Default("form-control is-display")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    protected string? CurrentTextAsString { get; set; }

    [Parameter]
    public Func<TValue, Task<string>>? FormatterAsync { get; set; }

    [Parameter]
    public string? FormatString { get; set; }

    [Parameter]
    public IEnumerable<SelectedItem>? Lookup { get; set; }

    [Parameter]
    public string? LookupServiceKey { get; set; }

    [Inject]
    [NotNull]
    private ILookupService? LookupService { get; set; }

    [Parameter]

    public Func<Assembly?, string, bool, Type?>? TypeResolver { get; set; }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (!string.IsNullOrEmpty(LookupServiceKey))
        {
            Lookup = LookupService.GetItemsByKey(LookupServiceKey);
        }

        return base.SetParametersAsync(ParameterView.Empty);
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        CurrentTextAsString = await FormatTextAsString(Value);
    }

    protected virtual async Task<string?> FormatTextAsString(TValue value) => FormatterAsync != null
        ? await FormatterAsync(value)
        : (!string.IsNullOrEmpty(FormatString) && value != null
            ? Utility.Format(value, FormatString)
            : value == null
                ? FormatValueString()
                : FormatText(value));

    private string FormatText([DisallowNull] TValue value)
    {
        string ret;
        var type = typeof(TValue);
        if (type.IsEnum())
        {
            ret = Utility.GetDisplayName(type, value.ToString()!);
        }
        else if (type.IsArray)
        {
            ret = ConvertArrayToString(value);
        }
        else if (type.IsGenericType && type.IsAssignableTo(typeof(IEnumerable)))
        {
            ret = ConvertEnumerableToString(value);
        }
        else
        {
            ret = FormatValueString();
        }
        return ret;
    }

    private string FormatValueString()
    {
        string? ret = null;

        var valueString = Value?.ToString();
        if (Lookup != null)
        {
            ret = Lookup.FirstOrDefault(i => i.Value.Equals(valueString ?? "", StringComparison.OrdinalIgnoreCase))?.Text;
        }
        return ret ?? valueString ?? string.Empty;
    }

    private Func<TValue, string>? _converterArray;
    private string ConvertArrayToString(TValue value)
    {
        return (_converterArray ??= ConvertArrayToStringLambda())(value);

        Func<TValue, string> ConvertArrayToStringLambda()
        {
            Func<TValue, string> ret = _ => "";
            var param_p1 = Expression.Parameter(typeof(Array));
            var target_type = typeof(TValue).UnderlyingSystemType;
            var methodType = ResolveArrayType();
            if (methodType != null)
            {
                var method = typeof(string).GetMethods().Where(m => m.Name == "Join" && m.IsGenericMethod && m.GetParameters()[0].ParameterType == typeof(string)).First().MakeGenericMethod(methodType);
                var body = Expression.Call(method, Expression.Constant(","), Expression.Convert(param_p1, target_type));
                ret = Expression.Lambda<Func<TValue, string>>(body, param_p1).Compile();
            }
            return ret;

            Type? ResolveArrayType()
            {
                Type? ret = null;
                var typeName = target_type.FullName;
                if (!string.IsNullOrEmpty(typeName))
                {
                    typeName = typeName.Replace("[]", "");
                    if (typeName.Contains('+'))
                    {
                        typeName = typeName.Split('+', StringSplitOptions.RemoveEmptyEntries).Last();
                    }
                    ret = Type.GetType(typeName, null, TypeResolver, false, true);
                }
                return ret;
            }
        }
    }

    private static Func<TValue, string>? _convertEnumerableToString;
    private static Func<TValue, IEnumerable<string>>? _convertToEnumerableString;
    private string ConvertEnumerableToString(TValue value)
    {
        return Lookup == null
            ? (_convertEnumerableToString ??= ConvertEnumerableToStringLambda())(value)
            : GetTextByValue((_convertToEnumerableString ??= ConvertToEnumerableStringLambda())(value));

        static Func<TValue, string> ConvertEnumerableToStringLambda()
        {
            var typeArguments = typeof(TValue).GenericTypeArguments;
            var param_p1 = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(typeArguments));
            var method = typeof(string).GetMethods().Where(m => m.Name == "Join" && m.IsGenericMethod && m.GetParameters()[0].ParameterType == typeof(string)).First().MakeGenericMethod(typeArguments);
            var body = Expression.Call(method, Expression.Constant(","), param_p1);
            return Expression.Lambda<Func<TValue, string>>(body, param_p1).Compile();
        }

        static Func<TValue, IEnumerable<string>> ConvertToEnumerableStringLambda()
        {
            var typeArguments = typeof(TValue).GenericTypeArguments;
            var param_p1 = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(typeArguments));

            var method = typeof(Display<>).MakeGenericType(typeof(TValue))
                .GetMethod(nameof(Cast), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(typeArguments);
            var body = Expression.Call(method, param_p1);
            return Expression.Lambda<Func<TValue, IEnumerable<string>>>(body, param_p1).Compile();
        }
    }

    private static IEnumerable<string> Cast<TType>(IEnumerable<TType> source) => source.Select(i => i?.ToString() ?? string.Empty);

    private string GetTextByValue(IEnumerable<string> source) => string.Join(",", source.Aggregate(new List<string>(), (s, i) =>
    {
        var text = Lookup!.FirstOrDefault(d => d.Value.Equals(i, StringComparison.OrdinalIgnoreCase))?.Text;
        if (text != null)
        {
            s.Add(text);
        }
        return s;
    }));
}
