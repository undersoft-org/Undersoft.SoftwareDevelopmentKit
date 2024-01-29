using Microsoft.JSInterop;
using Undersoft.SDK.Service.Application.Extensions;

namespace Undersoft.SDK.Service.Application.Components;

public abstract class BootstrapInputBase<TValue> : ValidateBase<TValue>
{
    protected virtual string? ClassName => CssBuilder.Default("form-control")
        .AddClass($"border-{Color.ToDescriptionString()}", Color != Color.None && !IsDisabled && !IsValid.HasValue)
        .AddClass(CssClass).AddClass(ValidCss)
        .Build();

    protected ElementReference FocusElement { get; set; }

    [Parameter]
    public string? PlaceHolder { get; set; }

    [Parameter]
    public Func<TValue, Task>? OnEnterAsync { get; set; }

    [Parameter]
    public Func<TValue, Task>? OnEscAsync { get; set; }

    [Parameter]
    public Color Color { get; set; } = Color.None;

    [Parameter]
    public Func<TValue, string>? Formatter { get; set; }

    [Parameter]
    public string? FormatString { get; set; }

    [Parameter]
    public bool IsAutoFocus { get; set; }

    [Parameter]
    public bool IsSelectAllTextOnFocus { get; set; }

    [Parameter]
    public bool IsSelectAllTextOnEnter { get; set; }

    [Parameter]
    public bool IsTrim { get; set; }

    [CascadingParameter]
    private Modal? Modal { get; set; }

    protected string Type { get; set; } = "text";

    public ValueTask FocusAsync() => FocusElement.FocusAsync();

    public async ValueTask SelectAllTextAsync() => await JSRuntime.InvokeVoidAsync(FocusElement, "bb_input_selectAll");

    private JSInterop<BootstrapInputBase<TValue>>? Interop { get; set; }

    protected bool SkipRegisterEnterEscJSInvoke { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (string.IsNullOrEmpty(PlaceHolder) && FieldIdentifier.HasValue)
        {
            PlaceHolder = FieldIdentifier.Value.GetPlaceHolder();
        }

        if (AdditionalAttributes != null && AdditionalAttributes.TryGetValue("type", out var t))
        {
            var type = t.ToString();
            if (!string.IsNullOrEmpty(type))
            {
                Type = type;
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            if (!SkipRegisterEnterEscJSInvoke && (OnEnterAsync != null || OnEscAsync != null))
            {
                Interop ??= new JSInterop<BootstrapInputBase<TValue>>(JSRuntime);
                await Interop.InvokeVoidAsync(this, FocusElement, "bb_input", OnEnterAsync != null, nameof(EnterCallback), OnEscAsync != null, nameof(EscCallback));
            }
            if (IsSelectAllTextOnFocus)
            {
                await JSRuntime.InvokeVoidAsync(FocusElement, "bb_input_selectAll_focus");
            }
            if (IsSelectAllTextOnEnter)
            {
                await JSRuntime.InvokeVoidAsync(FocusElement, "bb_input_selectAll_enter");
            }
            if (IsAutoFocus)
            {
                if (Modal != null)
                {
                    await Task.Delay(100);
                }
                await FocusAsync();
            }
        }
    }

    protected override string? FormatValueAsString(TValue value) => Formatter != null
        ? Formatter.Invoke(value)
        : (!string.IsNullOrEmpty(FormatString) && value != null
            ? Utility.Format(value, FormatString)
            : base.FormatValueAsString(value));

    protected override bool TryParseValueFromString(string value, [MaybeNullWhen(false)] out TValue result, out string? validationErrorMessage) => base.TryParseValueFromString(IsTrim ? value.Trim() : value, out result, out validationErrorMessage);

    [JSInvokable]
    public async Task EnterCallback(string val)
    {
        if (OnEnterAsync != null)
        {
            CurrentValueAsString = val;
            await OnEnterAsync(Value);
        }
    }

    [JSInvokable]
    public async Task EscCallback()
    {
        if (OnEscAsync != null)
        {
            await OnEscAsync(Value);
        }
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            Interop?.Dispose();
            Interop = null;
        }
        await base.DisposeAsync(disposing);
    }
}
