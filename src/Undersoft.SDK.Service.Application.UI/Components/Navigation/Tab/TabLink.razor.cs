namespace Undersoft.SDK.Service.Application.Components;

public sealed partial class TabLink
{
    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public string? Url { get; set; }

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public bool Closable { get; set; } = true;

    [Parameter]
    public Func<Task>? OnClick { get; set; }

    [Inject]
    [NotNull]
    private TabItemTextOptions? TabItemOptions { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private async Task OnClickLink()
    {
        if (OnClick != null)
        {
            await OnClick();
        }

        TabItemOptions.Icon = Icon;
        TabItemOptions.Text = Text;
        TabItemOptions.Closable = Closable;
    }

    private RenderFragment RenderChildContent() => builder =>
    {
        if (ChildContent == null)
        {
            if (!string.IsNullOrEmpty(Icon))
            {
                builder.OpenElement(0, "i");
                builder.AddAttribute(1, "class", Icon);
                builder.CloseElement();
            }
            if (!string.IsNullOrEmpty(Text))
            {
                builder.AddContent(2, Text);
            }
        }
        else
        {
            builder.AddContent(3, ChildContent);
        }
    };
}
