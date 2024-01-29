namespace Undersoft.SDK.Service.Application.Components;

public sealed partial class MessageItem
{
    private ElementReference MessageItemElement { get; set; }

    protected override string? ClassName => CssBuilder.Default("alert")
        .AddClass($"alert-{Color.ToDescriptionString()}", Color != Color.None)
        .AddClass("is-bar", ShowBar)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? AutoHideString => IsAutoHide ? "true" : null;

    [Parameter]
    public string? Content { get; set; }

    [Parameter]
    public bool IsAutoHide { get; set; } = true;

    [Parameter]
    public int Delay { get; set; } = 4000;

    [CascadingParameter]
    public Message? Message { get; set; }

    private JSInterop<Message>? _interop;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && Message != null)
        {
            _interop = new JSInterop<Message>(JSRuntime);
            await _interop.InvokeVoidAsync(Message, MessageItemElement, "bb_message", nameof(Message.Clear));
        }
    }

    private async Task OnClick()
    {
        if (OnDismiss != null) await OnDismiss();
    }
}
