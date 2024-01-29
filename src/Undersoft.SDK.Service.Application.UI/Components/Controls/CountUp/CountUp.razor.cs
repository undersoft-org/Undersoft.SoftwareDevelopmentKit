namespace Undersoft.SDK.Service.Application.Components;

public partial class CountUp<TValue>
{
    [Parameter]
    [NotNull]
    public TValue? Value { get; set; }

    [Parameter]
    public Func<Task>? OnCompleted { get; set; }

    [NotNull]
    private TValue? PreviousValue { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (!typeof(TValue).IsNumber())
        {
            throw new InvalidOperationException();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            PreviousValue = Value;
        }
        else if (!PreviousValue.Equals(Value))
        {
            await Update(Value);
        }
    }

    protected override Task InvokeInitAsync() => InvokeVoidAsync("init", Id, Interop, Value, OnCompleted != null ? nameof(OnCompleteCallback) : null);

    private async ValueTask Update(TValue? value)
    {
        PreviousValue = value;

        if (Module != null)
        {
            await Module.InvokeVoidAsync("update", Id, Value);
        }
    }

    [JSInvokable]
    public async Task OnCompleteCallback()
    {
        if (OnCompleted != null)
        {
            await OnCompleted();
        }
    }
}
