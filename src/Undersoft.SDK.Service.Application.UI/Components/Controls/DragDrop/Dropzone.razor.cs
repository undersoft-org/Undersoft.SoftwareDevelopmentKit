using System.Text;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Dropzone<TItem> : IDisposable
{
    [Parameter]
    [NotNull]
    public List<TItem>? Items { get; set; }

    [Parameter]
    public int? MaxItems { get; set; }

    [Parameter]
    public RenderFragment<TItem>? ChildContent { get; set; }

    [Parameter]
    public Func<TItem, string>? ItemWrapperClass { get; set; }

    [Parameter]
    public Func<TItem, TItem>? CopyItem { get; set; }

    [Parameter]
    public Func<TItem?, TItem?, bool>? Accepts { get; set; }

    [Parameter]
    public EventCallback<TItem> OnItemDropRejectedByMaxItemLimit { get; set; }

    [Parameter]
    public EventCallback<TItem> OnItemDropRejected { get; set; }

    [Parameter]
    public EventCallback<TItem> OnReplacedItemDrop { get; set; }

    [Parameter]
    public EventCallback<TItem> OnItemDrop { get; set; }

    [Parameter]
    public Func<TItem, bool>? AllowsDrag { get; set; }

    [Inject]
    [NotNull]
    private DragDropService<TItem>? DragDropService { get; set; }

    private string? ItemClass => CssBuilder.Default()
        .AddClass("bb-dd-inprogess", DragDropService.ActiveItem != null)
        .Build();

    [ExcludeFromCodeCoverage]
    private string GetItemClass(TItem? item)
    {
        if (item == null)
        {
            return "";
        }
        var builder = new StringBuilder();
        builder.Append("bb-dd-draggable");
        if (ItemWrapperClass != null)
        {
            builder.Append($" {ItemWrapperClass(item)}");
        }

        var activeItem = DragDropService.ActiveItem;
        if (activeItem == null)
        {
            return builder.ToString();
        }

        if (item.Equals(activeItem))
        {
            builder.Append(" no-pointer-events");
        }

        if (!item.Equals(activeItem) && item.Equals(DragDropService.DragTargetItem))
        {
            builder.Append(IsItemAccepted(DragDropService.DragTargetItem)
                ? " bb-dd-dragged-over"
                : " bb-dd-dragged-over-denied");
        }

        if (AllowsDrag != null && !AllowsDrag(item))
        {
            builder.Append(" bb-dd-noselect");
        }

        return builder.ToString();
    }

    private string GetClassesForSpacing(int spacerId)
    {
        var builder = new StringBuilder();
        builder.Append("bb-dd-spacing");
        if (DragDropService.ActiveItem == null)
        {
            return builder.ToString();
        }
        if (DragDropService.ActiveSpacerId == spacerId && Items.IndexOf(DragDropService.ActiveItem) == -1)
        {
            builder.Append(" bb-dd-spacing-dragged-over");
        }                 
        else if (DragDropService.ActiveSpacerId == spacerId && (spacerId != Items.IndexOf(DragDropService.ActiveItem)) && (spacerId != Items.IndexOf(DragDropService.ActiveItem) + 1))
        {
            builder.Append(" bb-dd-spacing-dragged-over");
        }

        return builder.ToString();
    }

    private string IsItemDragable(TItem? item)
    {
        if (item == null)
        {
            return "false";
        }
        if (AllowsDrag == null)
        {
            return "true";
        }

        return AllowsDrag(item).ToString();
    }

    private bool IsDropAllowed()
    {
        if (!IsValidItem())
        {
            return false;
        }

        var activeItem = DragDropService.ActiveItem;

        if (IsMaxItemLimitReached())
        {
            OnItemDropRejectedByMaxItemLimit.InvokeAsync(activeItem);
            return false;
        }

        if (!IsItemAccepted(activeItem))
        {
            OnItemDropRejected.InvokeAsync(activeItem);
            return false;
        }

        return true;
    }

    private bool IsItemAccepted(TItem? dragTargetItem)
    {
        if (Accepts == null)
        {
            return true;
        }

        return Accepts(DragDropService.ActiveItem, dragTargetItem);
    }

    private bool IsMaxItemLimitReached()
    {
        var activeItem = DragDropService.ActiveItem;
        return (!Items.Contains(activeItem!) && MaxItems.HasValue && MaxItems == Items.Count);
    }

    private bool IsValidItem()
    {
        return DragDropService.ActiveItem != null;
    }

    private void OnDropItemOnSpacing(int newIndex)
    {
        if (!IsDropAllowed())
        {
            DragDropService.Reset();
            return;
        }

        var activeItem = DragDropService.ActiveItem;

        bool sameDropZone = Equals(DragDropService.Items, Items);

        if (CopyItem == null || sameDropZone)
        {
            Items.Insert(newIndex, activeItem!);
            DragDropService.Commit();
        }
        else
        {
            Items.Insert(newIndex, CopyItem(activeItem!));
            DragDropService.Reset();
        }

        OnItemDrop.InvokeAsync(activeItem!);
    }

    private void OnDragStart(TItem item)
    {
        DragDropService.OldIndex = Items.IndexOf(item);
        DragDropService.ActiveItem = item;
        DragDropService.Items = Items;
        Items.Remove(item);
        if (DragDropService.OldIndex >= Items.Count)
        {
            Items.Add(default!);
        }
    }

    private void OnDragEnd()
    {
        if (DragDropService.Items != null)
        {
            if (DragDropService.OldIndex.HasValue)
            {
                if (DragDropService.ActiveItem != null)
                {
                    DragDropService.Items.Insert(DragDropService.OldIndex.Value, DragDropService.ActiveItem);
                }
            }
            StateHasChanged();
        }
        Items.Remove(default!);
    }

    private void OnDragEnter(TItem? item)
    {
        if (item == null)
        {
            return;
        }
        var activeItem = DragDropService.ActiveItem;
        if (activeItem == null)
        {
            return;
        }

        if (IsMaxItemLimitReached())
        {
            return;
        }

        if (!IsItemAccepted(item))
        {
            return;
        }

        DragDropService.DragTargetItem = item;

        StateHasChanged();
    }

    private void OnDragLeave()
    {
        DragDropService.DragTargetItem = default;
        StateHasChanged();
    }

    private void OnDrop()
    {
        if (!IsDropAllowed())
        {
            DragDropService.Reset();
            return;
        }

        var activeItem = DragDropService.ActiveItem;

        if (DragDropService.DragTargetItem == null)
        {
            if (!Equals(DragDropService.Items, Items) && CopyItem != null)
            {
                Items.Insert(Items.Count, CopyItem(activeItem!));
                DragDropService.Reset();
            }
            else
            {
                Items.Insert(Items.Count, activeItem!);
                DragDropService.Commit();
            }
        }
        else
        {
            OnReplacedItemDrop.InvokeAsync(DragDropService.DragTargetItem);
            if (!Equals(DragDropService.Items, Items) && CopyItem != null)
            {
                Swap(DragDropService.DragTargetItem, CopyItem(activeItem!));
                DragDropService.Reset();
            }
            else
            {
                Swap(DragDropService.DragTargetItem, activeItem!);
                DragDropService.Commit();
            }
        }

        StateHasChanged();
        OnItemDrop.InvokeAsync(activeItem);
    }

    private void Swap(TItem draggedOverItem, TItem activeItem)
    {
        var indexDraggedOverItem = Items.IndexOf(draggedOverItem);
        Items.Insert(indexDraggedOverItem + 1, activeItem);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        DragDropService.StateHasChanged += ForceRender;
    }

    private void ForceRender(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            DragDropService.StateHasChanged -= ForceRender;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
