using Microsoft.CSharp.RuntimeBinder;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Undersoft.SDK.Service.Application.Components;

namespace System.Linq;

public static class LambdaExtensions
{
    private class ComboExpressionVisitor : ExpressionVisitor
    {
        private ParameterExpression exp_p { get; set; }

        public ComboExpressionVisitor(ParameterExpression parameter)
        {
            exp_p = parameter;
        }

        protected override Expression VisitParameter(ParameterExpression p) => exp_p;
    }

    public static Expression<Func<TItem, bool>> GetFilterLambda<TItem>(this IEnumerable<FilterKeyValueAction> filters)
    {
        Expression<Func<TItem, bool>>? ret = null;
        if (filters.Any())
        {
            var exp_p = Expression.Parameter(typeof(TItem));
            var visitor = new ComboExpressionVisitor(exp_p);

            foreach (var filter in filters)
            {
                var exp = filter.GetFilterLambda<TItem>();
                if (ret == null)
                {
                    ret = exp;
                    continue;
                }

                var left = visitor.Visit(ret.Body);
                var right = visitor.Visit(exp.Body);

                ret = filter.FilterLogic switch
                {
                    FilterLogic.And => Expression.Lambda<Func<TItem, bool>>(Expression.AndAlso(left, right), exp_p),
                    _ => Expression.Lambda<Func<TItem, bool>>(Expression.OrElse(left, right), exp_p),
                };
            }
        }
        return ret ?? (r => true);
    }

    private static Expression<Func<TItem, bool>> ExpressionAndLambda<TItem>(this IEnumerable<Expression<Func<TItem, bool>>> expressions, FilterLogic logic = FilterLogic.And)
    {
        Expression<Func<TItem, bool>>? ret = null;
        if (expressions.Any())
        {
            var exp_p = Expression.Parameter(typeof(TItem));
            var visitor = new ComboExpressionVisitor(exp_p);

            foreach (var exp in expressions)
            {
                if (ret == null)
                {
                    ret = exp;
                    continue;
                }

                var left = visitor.Visit(ret.Body);
                var right = visitor.Visit(exp.Body);
                ret = logic == FilterLogic.And
                    ? Expression.Lambda<Func<TItem, bool>>(Expression.AndAlso(left, right), exp_p)
                    : Expression.Lambda<Func<TItem, bool>>(Expression.OrElse(left, right), exp_p);
            }
        }
        return ret ?? (r => true);
    }

    public static Func<TItem, bool> GetFilterFunc<TItem>(this IEnumerable<IFilterAction> filters, FilterLogic logic = FilterLogic.And)
    {
        return filters.GetFilterLambda<TItem>(logic).Compile();
    }

    public static Expression<Func<TItem, bool>> GetFilterLambda<TItem>(this IEnumerable<IFilterAction> filters, FilterLogic logic = FilterLogic.And)
    {
        var exps = filters.Select(f => f.GetFilterConditions().GetFilterLambda<TItem>());
        return exps.ExpressionAndLambda(logic);
    }

    public static Expression<Func<TItem, bool>> GetFilterLambda<TItem>(this FilterKeyValueAction filter)
    {
        Expression<Func<TItem, bool>> ret = t => true;
        var type = typeof(TItem);
        if (!string.IsNullOrEmpty(filter.FieldKey) && filter.FieldValue != null)
        {
            ret = filter.FieldKey.Contains('.') ? GetComplexFilterExpression() : GetSimpleFilterExpression();
        }
        return ret;

        Expression<Func<TItem, bool>> GetSimpleFilterExpression()
        {
            var prop = typeof(TItem).GetPropertyByName(filter.FieldKey) ?? throw new InvalidOperationException($"the model {type.Name} not found the property {filter.FieldKey}");
            if (prop != null)
            {
                var p = Expression.Parameter(type);
                var fieldExpression = Expression.Property(p, prop);
                var isNullable = false;

                Expression eq = fieldExpression;

                if (prop.PropertyType.IsGenericType &&
                    prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    isNullable = true;
                    eq = Expression.Convert(fieldExpression, prop.PropertyType.GenericTypeArguments[0]);
                }
                else if (prop.PropertyType.IsEnum && filter.FieldValue is string)
                {
                    eq = Expression.Call(fieldExpression, prop.PropertyType.GetMethod("ToString", Array.Empty<Type>())!);
                }
                eq = isNullable
                    ? Expression.AndAlso(Expression.NotEqual(fieldExpression, Expression.Constant(null)), filter.GetExpression(eq))
                    : filter.GetExpression(eq);
                ret = Expression.Lambda<Func<TItem, bool>>(eq, p);
            }
            return ret;
        }

        Expression<Func<TItem, bool>> GetComplexFilterExpression()
        {
            Expression<Func<TItem, bool>> ret = t => true;
            var p = Expression.Parameter(type);
            var propertyNames = filter.FieldKey.Split('.');
            var isNullable = false;
            PropertyInfo? pInfo = null;
            Expression? fieldExpression = null;
            foreach (var name in propertyNames)
            {
                if (pInfo == null)
                {
                    pInfo = typeof(TItem).GetPropertyByName(name) ?? throw new InvalidOperationException($"the model {type.Name} not found the property {name}");
                    fieldExpression = Expression.Property(p, pInfo);
                }
                else
                {
                    pInfo = pInfo.PropertyType.GetPropertyByName(name) ?? throw new InvalidOperationException($"the model {pInfo.PropertyType.Name} not found the property {name}");
                    fieldExpression = Expression.Property(fieldExpression, pInfo);
                }
            }

            if (fieldExpression != null)
            {
                var eq = fieldExpression;

                if (pInfo!.PropertyType.IsGenericType && pInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    isNullable = true;
                    eq = Expression.Convert(fieldExpression, pInfo.PropertyType.GenericTypeArguments[0]);
                }
                else if (pInfo.PropertyType.IsEnum && filter.FieldValue is string)
                {
                    eq = Expression.Call(fieldExpression, pInfo.PropertyType.GetMethod("ToString", Array.Empty<Type>())!);
                }
                eq = isNullable
                    ? Expression.AndAlso(Expression.NotEqual(fieldExpression, Expression.Constant(null)), filter.GetExpression(eq))
                    : filter.GetExpression(eq);
                ret = Expression.Lambda<Func<TItem, bool>>(eq, p);
            }
            return ret;
        }
    }

    public static Func<TItem, bool> GetFilterFunc<TItem>(this FilterKeyValueAction filter) => filter.GetFilterLambda<TItem>().Compile();

    private static Expression GetExpression(this FilterKeyValueAction filter, Expression left)
    {
        var right = Expression.Constant(filter.FieldValue);
        return filter.FilterAction switch
        {
            FilterAction.Equal => Expression.Equal(left, right),
            FilterAction.NotEqual => Expression.NotEqual(left, right),
            FilterAction.GreaterThan => Expression.GreaterThan(left, right),
            FilterAction.GreaterThanOrEqual => Expression.GreaterThanOrEqual(left, right),
            FilterAction.LessThan => Expression.LessThan(left, right),
            FilterAction.LessThanOrEqual => Expression.LessThanOrEqual(left, right),
            FilterAction.Contains => left.Contains(right),
            FilterAction.NotContains => Expression.Not(left.Contains(right)),
            _ => filter.FieldValue switch
            {
                LambdaExpression t => Expression.Invoke(t, left),
                Delegate _ => Expression.Invoke(right, left),
                _ => throw new InvalidOperationException(nameof(FilterKeyValueAction.FieldValue))
            },
        };
    }

    private static Expression Contains(this Expression left, Expression right)
    {
        var method = typeof(string).GetMethod("Contains", new Type[1] { typeof(string) })!;
        return Expression.AndAlso(Expression.NotEqual(left, Expression.Constant(null)), Expression.Call(left, method, right));
    }

    #region Count
    public static int ElementCount(object? value) => CacheManager.ElementCount(value);

    public static Expression<Func<object, int>> CountLambda(Type type)
    {
        Expression<Func<object, int>> invoker = _ => 0;
        var elementType = type.IsGenericType ? type.GetGenericArguments()[0] : type.GetElementType();
        if (elementType != null)
        {
            var p1 = Expression.Parameter(typeof(object));
            var method = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.Name == nameof(Enumerable.Count) && m.GetGenericArguments().Length == 1);
            if (method != null)
            {
                method = method.MakeGenericMethod(elementType);
                var body = Expression.Call(method, Expression.Convert(p1, typeof(IEnumerable<>).MakeGenericType(elementType)));
                invoker = Expression.Lambda<Func<object, int>>(body, p1);
            }
        }
        return invoker;
    }
    #endregion

    #region Sort
    public static Expression<Func<IEnumerable<TItem>, List<string>, IEnumerable<TItem>>> GetSortListLambda<TItem>()
    {
        var exp_p1 = Expression.Parameter(typeof(IEnumerable<TItem>));
        var exp_p2 = Expression.Parameter(typeof(List<string>));

        var mi = typeof(LambdaExtensions).GetMethods().First(m => m.Name == nameof(Sort) && m.ReturnType.Name == typeof(IEnumerable<>).Name && m.GetParameters().Any(p => p.Name == "sortList")).MakeGenericMethod(typeof(TItem));
        var body = Expression.Call(mi, exp_p1, exp_p2);
        return Expression.Lambda<Func<IEnumerable<TItem>, List<string>, IEnumerable<TItem>>>(body, exp_p1, exp_p2);
    }

    public static IEnumerable<TItem> Sort<TItem>(this IEnumerable<TItem> items, List<string> sortList)
    {
        for (var index = 0; index < sortList.Count; index++)
        {
            var sortExp = sortList[index];
            var segs = sortExp.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var sortOrder = SortOrder.Asc;
            var sortName = sortExp;
            if (segs.Length == 2)
            {
                sortName = segs[0];
                if (segs[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    sortOrder = SortOrder.Desc;
                }
            }
            if (index == 0)
            {
                items = EnumerableOrderBy(items, sortName, sortOrder);
            }
            else
            {
                items = EnumerableThenBy(items, sortName, sortOrder);
            }
        }
        return items;
    }

    public static IQueryable<TItem> Sort<TItem>(this IQueryable<TItem> items, List<string> sortList)
    {
        for (var index = 0; index < sortList.Count; index++)
        {
            var sortExp = sortList[index];
            var segs = sortExp.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var sortOrder = SortOrder.Asc;
            var sortName = sortExp;
            if (segs.Length == 2)
            {
                sortName = segs[0];
                if (segs[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
                {
                    sortOrder = SortOrder.Desc;
                }
            }
            if (index == 0)
            {
                items = QueryableOrderBy(items, sortName, sortOrder);
            }
            else
            {
                items = QueryableThenBy(items, sortName, sortOrder);
            }
        }
        return items;
    }

    public static Expression<Func<IEnumerable<TItem>, string, SortOrder, IEnumerable<TItem>>> GetSortLambda<TItem>()
    {
        var exp_p1 = Expression.Parameter(typeof(IEnumerable<TItem>));
        var exp_p2 = Expression.Parameter(typeof(string));
        var exp_p3 = Expression.Parameter(typeof(SortOrder));

        var mi = typeof(LambdaExtensions).GetMethods().First(m => m.Name == nameof(Sort) && m.ReturnType.Name == typeof(IEnumerable<>).Name && m.GetParameters().Any(p => p.Name == "sortName")).MakeGenericMethod(typeof(TItem));
        var body = Expression.Call(mi, exp_p1, exp_p2, exp_p3);
        return Expression.Lambda<Func<IEnumerable<TItem>, string, SortOrder, IEnumerable<TItem>>>(body, exp_p1, exp_p2, exp_p3);
    }

    public static IEnumerable<TItem> Sort<TItem>(this IEnumerable<TItem> items, string sortName, SortOrder sortOrder)
    {
        return sortOrder == SortOrder.Unset ? items : EnumerableOrderBy(items, sortName, sortOrder);
    }

    public static IQueryable<TItem> Sort<TItem>(this IQueryable<TItem> items, string sortName, SortOrder sortOrder)
    {
        return sortOrder == SortOrder.Unset ? items : QueryableOrderBy(items, sortName, sortOrder);
    }

    private static IQueryable<TItem>? InvokeSortByPropertyInfo<TItem>(this IQueryable<TItem> query, string methodName, PropertyInfo pi)
    {
        var mi = typeof(LambdaExtensions)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(typeof(TItem), pi.PropertyType);
        return mi!.Invoke(null, new object[] { query.AsQueryable(), pi }) as IOrderedQueryable<TItem>;
    }

    private static IQueryable<TItem>? InvokeSortByPropertyName<TItem>(this IQueryable<TItem> query, string methodName, PropertyInfo pi, string propertyName)
    {
        var mi = typeof(LambdaExtensions)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(typeof(TItem), pi.PropertyType);
        return mi!.Invoke(null, new object[] { query.AsQueryable(), propertyName }) as IOrderedQueryable<TItem>;
    }

    private static PropertyInfo? GetPropertyInfoByName<TItem>(this PropertyInfo? pi, string propertyName)
    {
        if (pi == null)
        {
            pi = typeof(TItem).GetPropertyByName(propertyName);
        }
        else
        {
            pi = pi.PropertyType.GetPropertyByName(propertyName);
        }
        return pi;
    }

    private static IEnumerable<TItem> EnumerableOrderBy<TItem>(IEnumerable<TItem> query, string propertyName, SortOrder sortOrder)
    {
        return propertyName.Contains('.') ? EnumerableOrderByComplex() : EnumerableOrderBySimple();

        IEnumerable<TItem> EnumerableOrderBySimple()
        {
            IEnumerable<TItem>? ret = null;
            var pi = typeof(TItem).GetPropertyByName(propertyName);
            if (pi != null)
            {
                var methodName = sortOrder == SortOrder.Desc ? nameof(OrderByDescendingInternal) : nameof(OrderByInternal);
                ret = query.AsQueryable().InvokeSortByPropertyInfo(methodName, pi);
            }
            return ret ?? query;
        }

        IEnumerable<TItem> EnumerableOrderByComplex()
        {
            IEnumerable<TItem>? ret = null;
            PropertyInfo? pi = null;
            foreach (var name in propertyName.Split('.'))
            {
                pi = pi.GetPropertyInfoByName<TItem>(name);
            }
            if (pi != null)
            {
                var methodName = sortOrder == SortOrder.Desc ? nameof(OrderByDescendingInternalByName) : nameof(OrderByInternalByName);
                ret = query.AsQueryable().InvokeSortByPropertyName(methodName, pi, propertyName);
            }
            return ret ?? query;
        }
    }

    private static IEnumerable<TItem> EnumerableThenBy<TItem>(IEnumerable<TItem> query, string propertyName, SortOrder sortOrder)
    {
        return propertyName.Contains('.') ? EnumerableThenByComplex() : EnumerableThenBySimple();

        IEnumerable<TItem> EnumerableThenBySimple()
        {
            IEnumerable<TItem>? ret = null;
            var pi = typeof(TItem).GetPropertyByName(propertyName);
            if (pi != null)
            {
                var methodName = sortOrder == SortOrder.Desc ? nameof(ThenByDescendingInternal) : nameof(ThenByInternal);
                ret = query.AsQueryable().InvokeSortByPropertyInfo(methodName, pi);
            }
            return ret ?? query;
        }

        IEnumerable<TItem> EnumerableThenByComplex()
        {
            IEnumerable<TItem>? ret = null;
            PropertyInfo? pi = null;
            foreach (var name in propertyName.Split('.'))
            {
                pi = pi.GetPropertyInfoByName<TItem>(name);
            }
            if (pi != null)
            {
                var methodName = sortOrder == SortOrder.Desc ? nameof(ThenByDescendingInternalByName) : nameof(ThenByInternalByName);
                ret = query.AsQueryable().InvokeSortByPropertyName(methodName, pi, propertyName);
            }
            return ret ?? query;
        }
    }

    private static IQueryable<TItem> QueryableOrderBy<TItem>(IQueryable<TItem> query, string propertyName, SortOrder sortOrder)
    {
        return propertyName.Contains('.') ? QueryableOrderByComplex() : QueryableOrderBySimple();

        IQueryable<TItem> QueryableOrderBySimple()
        {
            IQueryable<TItem>? ret = null;
            var pi = typeof(TItem).GetPropertyByName(propertyName);
            if (pi != null)
            {
                var methodName = sortOrder == SortOrder.Desc ? nameof(OrderByDescendingInternal) : nameof(OrderByInternal);
                ret = query.AsQueryable().InvokeSortByPropertyInfo(methodName, pi);
            }
            return ret ?? query;
        }

        IQueryable<TItem> QueryableOrderByComplex()
        {
            IQueryable<TItem>? ret = null;
            PropertyInfo? pi = null;
            foreach (var name in propertyName.Split('.'))
            {
                pi = pi.GetPropertyInfoByName<TItem>(name);
            }
            if (pi != null)
            {
                var methodName = sortOrder == SortOrder.Desc ? nameof(OrderByDescendingInternalByName) : nameof(OrderByInternalByName);
                ret = query.AsQueryable().InvokeSortByPropertyName(methodName, pi, propertyName);
            }
            return ret ?? query;
        }
    }

    private static IQueryable<TItem> QueryableThenBy<TItem>(IQueryable<TItem> query, string propertyName, SortOrder sortOrder)
    {
        return propertyName.Contains('.') ? QueryableThenByComplex() : QueryableThenBySimple();

        IQueryable<TItem> QueryableThenBySimple()
        {
            IQueryable<TItem>? ret = null;
            var pi = typeof(TItem).GetPropertyByName(propertyName);
            if (pi != null)
            {
                var methodName = sortOrder == SortOrder.Desc ? nameof(ThenByDescendingInternal) : nameof(ThenByInternal);
                ret = query.AsQueryable().InvokeSortByPropertyInfo(methodName, pi);
            }
            return ret ?? query;
        }

        IQueryable<TItem> QueryableThenByComplex()
        {
            IQueryable<TItem>? ret = null;
            PropertyInfo? pi = null;
            foreach (var name in propertyName.Split('.'))
            {
                pi = pi.GetPropertyInfoByName<TItem>(name);
            }
            if (pi != null)
            {
                var methodName = sortOrder == SortOrder.Desc ? nameof(ThenByDescendingInternalByName) : nameof(ThenByInternalByName);
                ret = query.AsQueryable().InvokeSortByPropertyName(methodName, pi, propertyName);
            }
            return ret ?? query;
        }
    }

    private static IOrderedQueryable<TItem> OrderByInternalByName<TItem, TKey>(IQueryable<TItem> query, string propertyName) => query.OrderBy(GetPropertyLambdaByName<TItem, TKey>(propertyName));

    private static IOrderedQueryable<TItem> OrderByDescendingInternalByName<TItem, TKey>(IQueryable<TItem> query, string propertyName) => query.OrderByDescending(GetPropertyLambdaByName<TItem, TKey>(propertyName));

    private static IOrderedQueryable<TItem> ThenByInternalByName<TItem, TKey>(IOrderedQueryable<TItem> query, string propertyName) => query.ThenBy(GetPropertyLambdaByName<TItem, TKey>(propertyName));

    private static IOrderedQueryable<TItem> ThenByDescendingInternalByName<TItem, TKey>(IOrderedQueryable<TItem> query, string propertyName) => query.ThenByDescending(GetPropertyLambdaByName<TItem, TKey>(propertyName));

    private static IOrderedQueryable<TItem> OrderByInternal<TItem, TKey>(IQueryable<TItem> query, System.Reflection.PropertyInfo memberProperty) => query.OrderBy(GetPropertyLambda<TItem, TKey>(memberProperty));

    private static IOrderedQueryable<TItem> OrderByDescendingInternal<TItem, TKey>(IQueryable<TItem> query, System.Reflection.PropertyInfo memberProperty) => query.OrderByDescending(GetPropertyLambda<TItem, TKey>(memberProperty));

    private static IOrderedQueryable<TItem> ThenByInternal<TItem, TKey>(IOrderedQueryable<TItem> query, System.Reflection.PropertyInfo memberProperty) => query.ThenBy(GetPropertyLambda<TItem, TKey>(memberProperty));

    private static IOrderedQueryable<TItem> ThenByDescendingInternal<TItem, TKey>(IOrderedQueryable<TItem> query, System.Reflection.PropertyInfo memberProperty) => query.ThenByDescending(GetPropertyLambda<TItem, TKey>(memberProperty));

    private static Expression<Func<TItem, TKey>> GetPropertyLambda<TItem, TKey>(PropertyInfo pi)
    {
        var exp_p1 = Expression.Parameter(typeof(TItem));
        return Expression.Lambda<Func<TItem, TKey>>(Expression.Property(exp_p1, pi), exp_p1);
    }

    private static Expression<Func<TItem, TKey>> GetPropertyLambdaByName<TItem, TKey>(string propertyName)
    {
        var exp_p1 = Expression.Parameter(typeof(TItem));
        PropertyInfo? pi = null;
        Expression? expression = null;
        foreach (var name in propertyName.Split('.'))
        {
            if (pi == null)
            {
                pi = typeof(TItem).GetPropertyByName(name);
                expression = Expression.PropertyOrField(exp_p1, name);
            }
            else
            {
                pi = pi.PropertyType.GetPropertyByName(name);
                expression = Expression.PropertyOrField(expression!, name);
            }
        }
        return Expression.Lambda<Func<TItem, TKey>>(expression!, exp_p1);
    }
    #endregion

    public static Expression<Func<TModel, TResult>> GetPropertyValueLambda<TModel, TResult>(TModel model, string propertyName)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        var type = model.GetType();
        var param_p1 = Expression.Parameter(typeof(TModel));
        return propertyName.Contains('.') ? GetComplexPropertyExpression() : GetSimplePropertyExpression();

        Expression<Func<TModel, TResult>> GetSimplePropertyExpression()
        {
            Expression body;
            var p = type.GetPropertyByName(propertyName);
            if (p != null)
            {
                body = Expression.Property(Expression.Convert(param_p1, type), p);
            }
            else if (type.IsAssignableTo(typeof(IDynamicMetaObjectProvider)))
            {
                var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(
                    CSharpBinderFlags.None,
                    propertyName,
                    type,
                    new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
                body = Expression.Dynamic(binder, typeof(object), param_p1);
            }
            else
            {
                throw new InvalidOperationException($"类型 {type.Name} 未找到 {propertyName} 属性，无法获取其值");
            }

            return Expression.Lambda<Func<TModel, TResult>>(Expression.Convert(body, typeof(TResult)), param_p1);
        }

        Expression<Func<TModel, TResult>> GetComplexPropertyExpression()
        {
            var propertyNames = propertyName.Split(".");
            Expression? body = null;
            Type t = type;
            object? propertyInstance = model;
            foreach (var name in propertyNames)
            {
                var p = t.GetPropertyByName(name) ?? throw new InvalidOperationException($"类型 {type.Name} 未找到 {name} 属性，无法获取其值");
                propertyInstance = p.GetValue(propertyInstance);
                if (propertyInstance != null)
                {
                    t = propertyInstance.GetType();
                }
                if (body == null)
                {
                    body = Expression.Property(Expression.Convert(param_p1, type), p);
                }
                else
                {
                    body = Expression.Property(body, p);
                }
            }
            return Expression.Lambda<Func<TModel, TResult>>(Expression.Convert(body!, typeof(TResult)), param_p1);
        }
    }

    public static Expression<Action<TModel, TValue>> SetPropertyValueLambda<TModel, TValue>(TModel model, string propertyName)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }

        var type = model.GetType();
        var param_p1 = Expression.Parameter(typeof(TModel));
        var param_p2 = Expression.Parameter(typeof(TValue));
        return propertyName.Contains('.') ? SetComplexPropertyExpression() : SetSimplePropertyExpression();

        Expression<Action<TModel, TValue>> SetSimplePropertyExpression()
        {
            var p = type.GetPropertyByName(propertyName) ?? throw new InvalidOperationException($"类型 {type.Name} 未找到 {propertyName} 属性，无法设置其值");

            var mi = p.GetSetMethod(true);
            var body = Expression.Call(Expression.Convert(param_p1, model.GetType()), mi!, Expression.Convert(param_p2, p.PropertyType));
            return Expression.Lambda<Action<TModel, TValue>>(body, param_p1, param_p2);
        }

        Expression<Action<TModel, TValue>> SetComplexPropertyExpression()
        {
            var propertyNames = propertyName.Split(".");
            Expression? body = null;
            Type t = type;
            object? propertyInstance = model;
            foreach (var name in propertyNames)
            {
                var p = t.GetPropertyByName(name) ?? throw new InvalidOperationException($"类型 {type.Name} 未找到 {name} 属性，无法获取其值");
                propertyInstance = p.GetValue(propertyInstance);
                if (propertyInstance != null)
                {
                    t = propertyInstance.GetType();
                }
                if (body == null)
                {
                    body = Expression.Property(Expression.Convert(param_p1, type), p);
                }
                else
                {
                    body = Expression.Property(body, p);
                }
            }
            body = Expression.Assign(body!, param_p2);
            return Expression.Lambda<Action<TModel, TValue>>(body, param_p1, param_p2);
        }
    }

    #region TryParse
    internal delegate TResult FuncEx<TIn, TOut, TResult>(TIn source, out TOut outValue);

    [ExcludeFromCodeCoverage]
    internal static Expression<FuncEx<string, TValue, bool>> TryParse<TValue>()
    {
        var t = typeof(TValue);
        var p1 = Expression.Parameter(typeof(string));
        var p2 = Expression.Parameter(t.MakeByRefType());
        var method = t.GetMethod("TryParse", new Type[] { typeof(string), t.MakeByRefType() });
        var body = method != null ? Expression.Call(method, p1, p2) : Expression.Call(typeof(LambdaExtensions).GetMethod("TryParseEmpty", BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(typeof(TValue)), p1, p2);
        return Expression.Lambda<FuncEx<string, TValue, bool>>(body, p1, p2);
    }

    [ExcludeFromCodeCoverage]
    private static bool TryParseEmpty<TValue>(string source, out TValue val)
    {
        val = default!;
        return false;
    }
    #endregion

    public static Expression<Func<TModel, TValue>> GetKeyValue<TModel, TValue>(Type? customAttribute = null)
    {
        var type = typeof(TModel);
        Expression<Func<TModel, TValue>> ret = _ => default!;
        var properties = type.GetRuntimeProperties()
                             .Where(p => p.IsDefined(customAttribute ?? typeof(KeyAttribute)))
                             .ToList();
        if (properties.Any())
        {
            var param = Expression.Parameter(type);
            var valueType = typeof(TValue);
            if (properties.Count == 1)
            {
                var body = Expression.Property(Expression.Convert(param, type), properties.First());
                ret = Expression.Lambda<Func<TModel, TValue>>(Expression.Convert(body, valueType), param);
            }
            else if (properties.Count < 9)
            {
                var tupleType = Type.GetType($"System.Tuple`{properties.Count}")!;
                var keyPropertyTypes = properties.Select(x => x.PropertyType).ToArray();
                var tupleConstructor = tupleType.MakeGenericType(keyPropertyTypes).GetConstructor(keyPropertyTypes);
                if (tupleConstructor != null)
                {
                    var newTupleExpression = Expression.New(tupleConstructor, properties.Select(p => Expression.Property(param, p)));
                    var body = Expression.Convert(newTupleExpression, valueType);
                    ret = Expression.Lambda<Func<TModel, TValue>>(Expression.Convert(body, valueType), param);
                }
            }
        }
        return ret;
    }
}
