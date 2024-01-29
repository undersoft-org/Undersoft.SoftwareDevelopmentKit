namespace Undersoft.SDK.Service.Application.Components;

public class QueryPageOptions
{
    public string? SearchText { get; set; }

    public string? SortName { get; set; }

    public SortOrder SortOrder { get; set; }

    public List<string> SortList { get; } = new(10);

    public object? SearchModel { get; set; }

    public int PageIndex { get; set; } = 1;

    public int StartIndex { get; set; }

    public int PageItems { get; set; } = 20;

    public bool IsPage { get; set; }

    public List<IFilterAction> Searchs { get; } = new(20);

    public List<IFilterAction> CustomerSearchs { get; } = new(20);

    public List<IFilterAction> AdvanceSearchs { get; } = new(20);

    public List<IFilterAction> Filters { get; } = new(20);
}
