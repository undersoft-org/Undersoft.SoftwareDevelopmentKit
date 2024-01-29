namespace Undersoft.SDK.Service.Application.Components;

public partial class Textarea
{
    private string? ClassName => CssBuilder.Default("form-control")
        .AddClass(CssClass).AddClass(ValidCss)
        .Build();

    public Task ScrollToTop() => InvokeExecuteAsync(Id, "toTop");

    public Task ScrollTo(int value) => InvokeExecuteAsync(Id, "to", value);

    public Task ScrollToBottom() => InvokeExecuteAsync(Id, "toBottom");

    [Parameter]
    public bool IsAutoScroll { get; set; }

    private string? AutoScrollString => IsAutoScroll ? "auto" : null;

    protected override Task ModuleExecuteAsync() => InvokeExecuteAsync(Id, "refresh");
}
