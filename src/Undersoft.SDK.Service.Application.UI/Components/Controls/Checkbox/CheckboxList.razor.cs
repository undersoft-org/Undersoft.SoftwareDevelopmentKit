using Microsoft.Extensions.Localization;
using System.Collections;
using System.Reflection;

namespace Undersoft.SDK.Service.Application.Components;

public partial class CheckboxList<TValue>
{
    protected string? GetClassString(string? defaultClass = "checkbox-vector form-control") => CssBuilder.Default()
        .AddClass(defaultClass)
        .AddClass("no-border", !ShowBorder && ValidCss != "is-invalid")
        .AddClass("is-vertical", IsVertical)
        .AddClass(CssClass).AddClass(ValidCss)
        .Build();

    protected string? CheckboxItemClassString => CssBuilder.Default("checkbox-item")
        .AddClass(CheckboxItemClass)
        .Build();

    [Parameter]
    [NotNull]
    public IEnumerable<SelectedItem>? Items { get; set; }

    [Parameter]
    public string? CheckboxItemClass { get; set; }

    [Parameter]
    public bool ShowBorder { get; set; } = true;

    [Parameter]
    public bool IsVertical { get; set; }

    [Parameter]
    public Func<IEnumerable<SelectedItem>, TValue, Task>? OnSelectedChanged { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizerFactory? LocalizerFactory { get; set; }

    protected bool GetDisabledState(SelectedItem item) => IsDisabled || item.IsDisabled;

    public override Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        EnsureParameterValid();

        return base.SetParametersAsync(ParameterView.Empty);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (EditContext != null && FieldIdentifier != null)
        {
            var pi = FieldIdentifier.Value.Model.GetType().GetPropertyByName(FieldIdentifier.Value.FieldName);
            if (pi != null)
            {
                var required = pi.GetCustomAttribute<RequiredAttribute>(true);
                if (required != null)
                {
                    Rules.Add(new RequiredValidator()
                    {
                        LocalizerFactory = LocalizerFactory,
                        ErrorMessage = required.ErrorMessage,
                        AllowEmptyString = required.AllowEmptyStrings
                    });
                }
            }
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Items == null)
        {
            var t = typeof(TValue);
            var innerType = t.GetGenericArguments().FirstOrDefault();
            if (innerType != null)
            {
                Items = innerType.ToSelectList();
            }
            Items ??= Enumerable.Empty<SelectedItem>();
        }

        InitValue();
    }

    private void InitValue()
    {
        if (Value != null)
        {
            var typeValue = typeof(TValue);
            IEnumerable? list = null;
            if (typeValue == typeof(string))
            {
                var values = CurrentValueAsString.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in Items)
                {
                    item.Active = values.Any(v => v.Equals(item.Value, StringComparison.OrdinalIgnoreCase));
                }
                list = values;
            }
            else if (typeValue.IsGenericType)
            {
                ProcessGenericItems(typeValue, list);
            }
        }
    }

    protected virtual void ProcessGenericItems(Type typeValue, IEnumerable? list)
    {
        var t = typeValue.GenericTypeArguments;
        var instance = Activator.CreateInstance(typeof(List<>).MakeGenericType(t));
        if (instance != null)
        {
            var mi = instance.GetType().GetMethod(nameof(List<string>.AddRange))!;
            mi.Invoke(instance, new object[] { Value });
            list = instance as IEnumerable;
            if (list != null)
            {
                foreach (var item in Items)
                {
                    item.Active = false;
                    foreach (var v in list)
                    {
                        item.Active = item.Value.Equals(v!.ToString(), StringComparison.OrdinalIgnoreCase);
                        if (item.Active)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    private async Task OnStateChanged(SelectedItem item, bool v)
    {
        item.Active = v;

        var typeValue = typeof(TValue);
        if (typeValue == typeof(string))
        {
            CurrentValueAsString = string.Join(",", Items.Where(i => i.Active).Select(i => i.Value));
        }
        else if (typeValue.IsGenericType)
        {
            var t = typeValue.GenericTypeArguments;
            if (Activator.CreateInstance(typeof(List<>).MakeGenericType(t)) is IList instance)
            {
                foreach (var sl in Items.Where(i => i.Active))
                {
                    if (sl.Value.TryConvertTo(t[0], out var val))
                    {
                        instance.Add(val);
                    }
                }
                CurrentValue = (TValue)instance;
            }
        }

        if (OnSelectedChanged != null)
        {
            await OnSelectedChanged.Invoke(Items, Value);
        }
    }

    protected virtual void EnsureParameterValid()
    {
        var typeValue = typeof(TValue);
        if (typeValue.IsGenericType)
        {
            if (!typeValue.IsAssignableTo(typeof(IEnumerable)))
            {
                throw new NotSupportedException();
            }
        }
        else if (typeValue != typeof(string))
        {
            throw new NotSupportedException();
        }
    }
}
