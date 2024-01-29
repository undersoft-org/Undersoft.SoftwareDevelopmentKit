namespace Undersoft.SDK.Service.Application.Components;

public class TabItem : ComponentBase
{
    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public RenderFragment<TabItem>? HeaderTemplate { get; set; }

    [Parameter]
    [NotNull]
    public string? Url { get; set; }

    [Parameter]
    public bool IsActive { get; set; }

    [Parameter]
    public bool Closable { get; set; } = true;

    [Parameter]
    public bool AlwaysLoad { get; set; }

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [CascadingParameter]
    protected internal Tab? TabSet { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Url ??= "";
        TabSet?.AddItem(this);
    }

    public virtual void SetActive(bool active) => IsActive = active;

    public void SetHeader(string text, string? icon = null, bool? closable = null)
    {
        if (TabSet != null)
        {
            Text = text;

            if (!string.IsNullOrEmpty(icon))
            {
                Icon = icon;
            }
            if (closable.HasValue)
            {
                Closable = closable.Value;
            }
            TabSet.ActiveTab(this);
        }
    }

    public static TabItem Create(Dictionary<string, object?> parameters)
    {
        var item = new TabItem();
        if (parameters.TryGetValue(nameof(Url), out var url))
        {
            parameters[nameof(Url)] = url?.ToString()?.TrimStart('/') ?? "";
        }
        var _ = item.SetParametersAsync(ParameterView.FromDictionary(parameters!));
        return item;
    }
}
