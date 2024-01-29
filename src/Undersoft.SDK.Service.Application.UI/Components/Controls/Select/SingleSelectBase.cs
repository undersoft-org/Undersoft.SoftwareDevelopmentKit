namespace Undersoft.SDK.Service.Application.Components;

public abstract class SingleSelectBase<TValue> : SelectBase<TValue>
{
    protected SelectedItem? SelectedItem { get; set; }

    [Parameter]
    public SwalCategory SwalCategory { get; set; } = SwalCategory.Question;

    [Parameter]
    public string? SwalTitle { get; set; }

    [Parameter]
    public string? SwalContent { get; set; }

    [Parameter]
    public string? SwalFooter { get; set; }

    [Parameter]
    public Func<SelectedItem, Task<bool>>? OnBeforeSelectedItemChange { get; set; }

    [Parameter]
    public Func<SelectedItem, Task>? OnSelectedItemChanged { get; set; }
}
