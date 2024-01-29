namespace Undersoft.SDK.Service.Application.Components;

public class CollapseItem : Component, IDisposable
{
    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public bool IsCollapsed { get; set; } = true;

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public Color TitleColor { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [CascadingParameter]
    protected Collapse? Collpase { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Collpase?.AddItem(this);
    }

    public virtual void SetCollapsed(bool collapsed) => IsCollapsed = collapsed;

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            disposedValue = true;

            if (disposing)
            {
                Collpase?.RemoveItem(this);
            }
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
