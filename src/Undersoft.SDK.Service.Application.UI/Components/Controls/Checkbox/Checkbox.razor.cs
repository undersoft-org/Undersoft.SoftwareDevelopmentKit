using System.Globalization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Checkbox<TValue>
{
    protected string? GetClassString(bool isButton = false) => CssBuilder.Default("form-check")
        .AddClass("is-label", IsShowAfterLabel)
        .AddClass("is-checked", State == CheckboxState.Checked)
        .AddClass("is-indeterminate", State == CheckboxState.Indeterminate)
        .AddClass($"form-check-{Color.ToDescriptionString()}", Color != Color.None && !isButton)
        .AddClass($"bg-{Color.ToDescriptionString()}", Color != Color.None && isButton && State == CheckboxState.Checked)
        .AddClass($"form-check-{Size.ToDescriptionString()}", Size != Size.None)
        .AddClass("disabled", IsDisabled)
        .AddClass(ValidCss)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private bool IsShowAfterLabel => ShowAfterLabel && !string.IsNullOrEmpty(DisplayText);

    protected string? InputClassString => CssBuilder.Default("form-check-Input")
        .AddClass($"border-{Color.ToDescriptionString()}", Color != Color.None)
        .AddClass("disabled", IsDisabled)
        .Build();

    protected string? CheckedString => State switch
    {
        CheckboxState.Checked => "checked",
        _ => null
    };

    private bool IsBoolean { get; set; }

    [Parameter]
    public Color Color { get; set; }

    [Parameter]
    public Size Size { get; set; }

    [Parameter]
    public bool ShowAfterLabel { get; set; }

    [Parameter]
    public CheckboxState State { get; set; }

    [Parameter]
    public EventCallback<CheckboxState> StateChanged { get; set; }

    [Parameter]
    public Func<CheckboxState, TValue, Task>? OnStateChanged { get; set; }

    [Parameter]
    public bool StopPropagation { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        IsBoolean = (Nullable.GetUnderlyingType(typeof(TValue)) ?? typeof(TValue)) == typeof(bool);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (ShowAfterLabel)
        {
            ShowLabel = false;
        }

        if (IsBoolean && Value != null && State != CheckboxState.Indeterminate)
        {
            if (BindConverter.TryConvertToBool(Value, CultureInfo.InvariantCulture, out var v))
            {
                State = v ? CheckboxState.Checked : CheckboxState.UnChecked;
            }
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        _peddingStateChanged = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        Module ??= await JSRuntime.LoadModule2("base/utility");
        if (Module != null)
        {
            await Module.InvokeVoidAsync("setIndeterminate", Id, State == CheckboxState.Indeterminate);
        }
    }

    private async Task OnToggleClick()
    {
        if (!IsDisabled)
        {
            _peddingStateChanged = true;
            await InternalStateChanged(State == CheckboxState.Checked ? CheckboxState.UnChecked : CheckboxState.Checked);
        }
    }

    private bool _peddingStateChanged;

    private async Task InternalStateChanged(CheckboxState state)
    {
        if (_peddingStateChanged)
        {
            if (IsBoolean)
            {
                CurrentValue = (TValue)(object)(state == CheckboxState.Checked);
            }

            if (State != state)
            {
                State = state;
                if (StateChanged.HasDelegate)
                {
                    await StateChanged.InvokeAsync(State);
                }

                if (OnStateChanged != null)
                {
                    await OnStateChanged.Invoke(State, Value);
                }
            }
        }
    }

    public virtual async Task SetState(CheckboxState state)
    {
        if (!_peddingStateChanged)
        {
            _peddingStateChanged = true;

            await InternalStateChanged(state);
            StateHasChanged();
        }
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            if (Module != null)
            {
                await Module.DisposeAsync();
                Module = null;
            }
        }
        await base.DisposeAsync(disposing);
    }
}
