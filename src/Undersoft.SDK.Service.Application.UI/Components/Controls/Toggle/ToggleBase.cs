namespace Undersoft.SDK.Service.Application.Components;

public class ToggleBase<TValue> : ValidateBase<TValue>
{
    protected virtual string? StyleName => CssBuilder.Default()
        .AddClass($"width: {Width}px;", Width > 0)
        .Build();

    [Parameter]
    public virtual int Width { get; set; } = 120;

    [Parameter]
    [NotNull]
    public virtual string? OnText { get; set; }

    [Parameter]
    [NotNull]
    public virtual string? OffText { get; set; }
}
