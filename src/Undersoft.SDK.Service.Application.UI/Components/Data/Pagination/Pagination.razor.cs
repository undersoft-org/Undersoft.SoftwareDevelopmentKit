using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Pagination
{
    protected int InternalPageCount => Math.Max(1, PageCount);

    protected string? ClassString => CssBuilder.Default("nav nav-pages")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    protected string? PaginationClassString => CssBuilder.Default("pagination")
        .AddClass($"justify-content-{Alignment.ToDescriptionString()}", Alignment != Alignment.None)
        .Build();

    protected int StartPageIndex => Math.Max(2, Math.Min(InternalPageCount - MaxPageLinkCount, InternalPageIndex - MaxPageLinkCount / 2));

    protected int EndPageIndex => Math.Min(InternalPageCount - 1, StartPageIndex + MaxPageLinkCount - 1);

    [Parameter]
    public Alignment Alignment { get; set; } = Alignment.Right;

    [Parameter]
    public string? PrevPageIcon { get; set; }

    [Parameter]
    public string? PrevEllipsisPageIcon { get; set; }

    [Parameter]
    public string? NextPageIcon { get; set; }

    [Parameter]
    public string? NextEllipsisPageIcon { get; set; }

    [Parameter]
    public int PageIndex { get; set; } = 1;

    [Parameter]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    public int PageCount { get; set; }

    [Parameter]
    public int MaxPageLinkCount { get; set; } = 5;

    [Parameter]
    public Func<int, Task>? OnPageLinkClick { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Pagination>? Localizer { get; set; }

    [Parameter]
    [NotNull]
    public string? PageInfoText { get; set; }

    [Parameter]
    public RenderFragment? GotoTemplate { get; set; }

    [Parameter]
    public bool ShowGotoNavigator { get; set; }

    [Parameter]
    public string? GotoNavigatorLabelText { get; set; }

    [Parameter]
    public bool ShowPageInfo { get; set; } = true;

    [Parameter]
    public RenderFragment? PageInfoTemplate { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private int InternalPageIndex => Math.Min(InternalPageCount, PageIndex);

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        GotoNavigatorLabelText ??= Localizer[nameof(GotoNavigatorLabelText)];
        PrevPageIcon ??= IconTheme.GetIconByKey(ComponentIcons.PaginationPrevPageIcon);
        NextPageIcon ??= IconTheme.GetIconByKey(ComponentIcons.PaginationNextPageIcon);
        PrevEllipsisPageIcon ??= IconTheme.GetIconByKey(ComponentIcons.PaginationPrevEllipsisPageIcon);
        NextEllipsisPageIcon ??= IconTheme.GetIconByKey(ComponentIcons.PaginationNextEllipsisPageIcon);
    }

    private async Task OnClick(int index)
    {
        await OnPageItemClick(index);
    }

    private async Task OnGoto(int index)
    {
        var pageIndex = Math.Max(1, Math.Min(index, PageCount));
        if (pageIndex != InternalPageIndex)
        {
            await OnPageItemClick(pageIndex);
        }
        StateHasChanged();
    }

    protected async Task MovePrev(int index)
    {
        var pageIndex = InternalPageIndex - index;
        if (pageIndex < 1)
        {
            pageIndex = InternalPageCount;
        }
        await OnPageItemClick(pageIndex);
    }

    protected async Task MoveNext(int index)
    {
        var pageIndex = InternalPageIndex + index;
        if (pageIndex > InternalPageCount)
        {
            pageIndex = 1;
        }
        await OnPageItemClick(pageIndex);
    }

    protected async Task OnPageItemClick(int pageIndex)
    {
        PageIndex = pageIndex;
        if (OnPageLinkClick != null)
        {
            await OnPageLinkClick(pageIndex);
        }
    }
}
