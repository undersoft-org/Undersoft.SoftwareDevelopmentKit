using System.Linq.Expressions;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

public partial class TableFooterCell
{
    private string? ClassString => CssBuilder.Default("table-cell")
        .AddClass("justify-content-start", Align == Alignment.Left)
        .AddClass("justify-content-center", Align == Alignment.Center)
        .AddClass("justify-content-end", Align == Alignment.Right)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public Alignment Align { get; set; }

    [Parameter]
    public AggregateType Aggregate { get; set; }

    [Parameter]
    public Func<object?, string?, string>? CustomerAggregateCallback { get; set; }

    [Parameter]
    public string? Field { get; set; }

    [CascadingParameter(Name = "IsMobileMode")]
    private bool IsMobileMode { get; set; }

    [CascadingParameter(Name = "TableFooterContext")]
    private object? DataSource { get; set; }

    private string? GetText() => Text ?? (GetCount(DataSource) == 0 ? "0" : (GetCountValue() ?? GetAggegateValue()));

    private string? GetCountValue()
    {
        string? v = null;
        if (Aggregate == AggregateType.Count && DataSource != null)
        {
            var type = DataSource.GetType();
            var modelType = type.GenericTypeArguments[0];

            var mi = GetType().GetMethod(nameof(CreateCountMethod), BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(modelType);

            if (mi != null)
            {
                var obj = mi.Invoke(null, new object[] { DataSource });
                if (obj != null)
                {
                    v = obj.ToString();
                }
            }
        }
        return v;
    }

    private string? GetAggegateValue()
    {
        return Aggregate == AggregateType.Customer ? AggregateCustomerValue() : AggregateNumberValue();

        string? AggregateCustomerValue()
        {
            string? v = null;
            if (CustomerAggregateCallback != null)
            {
                v = CustomerAggregateCallback(DataSource, Field);
            }
            return v;
        }

        string? AggregateNumberValue()
        {
            string? v = null;
            if (!string.IsNullOrEmpty(Field) && DataSource != null)
            {
                var type = DataSource.GetType();
                var modelType = type.GenericTypeArguments[0];

                var propertyInfo = modelType.GetProperty(Field);
                if (propertyInfo != null)
                {
                    var propertyType = propertyInfo.PropertyType;

                    var aggegateMethod = Aggregate switch
                    {
                        AggregateType.Average => propertyType.Name switch
                        {
                            nameof(Int32) or nameof(Int64) or nameof(Double) => GetType()
                                .GetMethod(nameof(CreateAggregateLambda), BindingFlags.NonPublic | BindingFlags.Static)!
                                .MakeGenericMethod(typeof(Double)),
                            _ => GetType()
                                .GetMethod(nameof(CreateAggregateLambda), BindingFlags.NonPublic | BindingFlags.Static)!
                                .MakeGenericMethod(propertyType),
                        },
                        _ => GetType().GetMethod(nameof(CreateAggregateLambda), BindingFlags.NonPublic | BindingFlags.Static)!
                            .MakeGenericMethod(propertyType)
                    };
                    if (aggegateMethod != null)
                    {
                        v = AggregateMethodInvoker(aggegateMethod, type, modelType, propertyType);
                    }
                }
            }
            return v;
        }

        string? AggregateMethodInvoker(MethodInfo aggegateMethod, Type type, Type modelType, Type propertyType)
        {
            string? v = null;
            var invoker = aggegateMethod.Invoke(null, new object[] { Aggregate, type, modelType, propertyType });
            if (invoker != null)
            {
                var methodInfo = GetType().GetMethod(nameof(CreateSelector), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(modelType, propertyType);
                if (methodInfo != null)
                {
                    var selector = methodInfo.Invoke(null, new object[] { Field });
                    if (selector != null)
                    {
                        if (invoker is Delegate d)
                        {
                            var val = d.DynamicInvoke(DataSource, selector);
                            if (val != null)
                            {
                                v = val.ToString();
                            }
                        }
                    }
                }
            }
            return v;
        }
    }

    private static Func<TModel, TValue> CreateSelector<TModel, TValue>(string field)
    {
        var type = typeof(TModel);
        var p1 = Expression.Parameter(type);
        var propertyInfo = type.GetProperty(field);
        var fieldExpression = Expression.Property(p1, propertyInfo!);
        return Expression.Lambda<Func<TModel, TValue>>(fieldExpression, p1).Compile();
    }

    private static Func<object, object, TValue?> CreateAggregateLambda<TValue>(AggregateType aggregate, Type type, Type modelType, Type propertyType)
    {
        Func<object, object, TValue?> ret = (_, _) => default;
        var mi = GetMethodInfoByAggregate(aggregate, modelType, propertyType);
        if (mi != null)
        {
            var p1 = Expression.Parameter(typeof(object));
            var p2 = Expression.Parameter(typeof(object));
            var body = Expression.Call(mi,
                Expression.Convert(p1, type),
                Expression.Convert(p2, typeof(Func<,>).MakeGenericType(new Type[] { modelType, propertyType })));
            ret = Expression.Lambda<Func<object, object, TValue?>>(body, p1, p2).Compile();
        }
        return ret;
    }

    private static MethodInfo? GetMethodInfoByAggregate(AggregateType aggregate, Type modelType, Type propertyType)
    {
        var mi = aggregate switch
        {
            AggregateType.Average => propertyType.Name switch
            {
                nameof(Int32) => typeof(Enumerable).GetMethods()
                    .FirstOrDefault(m => m.Name == aggregate.ToString() && m.IsGenericMethod
                        && m.ReturnType == typeof(Double) && m.GetParameters().Length == 2
                        && m.GetParameters()[1].ParameterType.GenericTypeArguments[1] == typeof(Int32)),
                nameof(Int64) => typeof(Enumerable).GetMethods()
                    .FirstOrDefault(m => m.Name == aggregate.ToString() && m.IsGenericMethod
                        && m.ReturnType == typeof(Double) && m.GetParameters().Length == 2
                        && m.GetParameters()[1].ParameterType.GenericTypeArguments[1] == typeof(Int64)),
                nameof(Double) => typeof(Enumerable).GetMethods()
                    .FirstOrDefault(m => m.Name == aggregate.ToString() && m.IsGenericMethod
                        && m.ReturnType == typeof(Double) && m.GetParameters().Length == 2
                        && m.GetParameters()[1].ParameterType.GenericTypeArguments[1] == typeof(Double)),
                nameof(Decimal) => typeof(Enumerable).GetMethods()
                    .FirstOrDefault(m => m.Name == aggregate.ToString() && m.IsGenericMethod
                        && m.ReturnType == typeof(Decimal) && m.GetParameters().Length == 2
                        && m.GetParameters()[1].ParameterType.GenericTypeArguments[1] == typeof(Decimal)),
                nameof(Single) => typeof(Enumerable).GetMethods()
                    .FirstOrDefault(m => m.Name == aggregate.ToString() && m.IsGenericMethod
                        && m.ReturnType == typeof(Single) && m.GetParameters().Length == 2
                        && m.GetParameters()[1].ParameterType.GenericTypeArguments[1] == typeof(Single)),
                _ => null
            },
            _ => typeof(Enumerable).GetMethods()
                    .FirstOrDefault(m => m.Name == aggregate.ToString() && m.IsGenericMethod && m.ReturnType == propertyType)
        };
        return mi?.MakeGenericMethod(modelType);
    }

    private static int CreateCountMethod<TSource>(IEnumerable<TSource> source) => source.Count();

    private static int GetCount(object? source)
    {
        var ret = 0;
        if (source != null)
        {
            var type = source.GetType();

            var modelType = type.GenericTypeArguments[0];

            var mi = typeof(TableFooterCell).GetMethod(nameof(CreateCountMethod), BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(modelType);

            if (mi != null)
            {
                var obj = mi.Invoke(null, new object[] { source });
                if (obj != null)
                {
                    var v = obj.ToString();
                    _ = int.TryParse(v, out ret);
                }
            }
        }
        return ret;
    }
}
