namespace Undersoft.SDK.Service.Application.Components;

internal class DragDropService<T>
{
    public T? ActiveItem { get; set; }

    public T? DragTargetItem { get; set; }

    public List<T>? Items { get; set; }

    public int? ActiveSpacerId { get; set; }

    public int? OldIndex { get; set; }

    public EventHandler? StateHasChanged { get; set; }

    public void Reset()
    {
        if (OldIndex is >= 0 && Items != null && ActiveItem != null)
        {
            Items.Insert(OldIndex.Value, ActiveItem);
        }
        Commit();
    }

    public void Commit()
    {
        ActiveItem = default;
        ActiveSpacerId = null;
        Items = null;
        DragTargetItem = default;

        if (StateHasChanged != null)
        {
            StateHasChanged.Invoke(this, EventArgs.Empty);
        }
    }
}
