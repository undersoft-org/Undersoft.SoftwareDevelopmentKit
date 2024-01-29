namespace Undersoft.SDK.Service.Application.Components;

public sealed partial class Breadcrumb
{
    [Parameter]
    public IEnumerable<BreadcrumbItem> Value { get; set; } = Enumerable.Empty<BreadcrumbItem>();

    private string? GetItemClassName(BreadcrumbItem item) => CssBuilder.Default("breadcrumb-item")
        .Build();

    private string? CurrentPage(BreadcrumbItem item) => CssBuilder.Default()
        .Build();
}
