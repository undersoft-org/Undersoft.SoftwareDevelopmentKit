namespace Undersoft.SDK.Service.Application.Components;

public partial class Avatar
{
    private string? ClassName => CssBuilder.Default("avatar")
        .AddClass("avatar-circle", IsCircle)
        .AddClass($"avatar-{Size.ToDescriptionString()}", Size != Size.None && Size != Size.Medium)
        .AddClass("border border-info", IsBorder)
        .AddClass("border-success", IsBorder && IsLoaded.HasValue && IsLoaded.Value && !IsIcon && !IsText)
        .AddClass("border-danger", IsBorder && IsLoaded.HasValue && !IsLoaded.Value)
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? ImgClassString => (IsLoaded.HasValue && IsLoaded.Value) ? null : "d-none";

    [Parameter]
    public bool IsCircle { get; set; }

    [Parameter]
    public string? Url { get; set; }

    [Parameter]
    public bool IsIcon { get; set; }

    [Parameter]
    public string? Icon { get; set; }

    [Parameter]
    public bool IsText { get; set; }

    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public Size Size { get; set; } = Size.Medium;

    [Parameter]
    public bool IsBorder { get; set; }

    [Parameter]
    public Func<Task<string>>? GetUrlAsync { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    private bool? IsLoaded { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (string.IsNullOrEmpty(Url) && GetUrlAsync != null)
        {
            Url = await GetUrlAsync();
        }
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Icon ??= IconTheme.GetIconByKey(ComponentIcons.AvatarIcon);
    }

    private void OnError()
    {
        IsIcon = true;
        IsLoaded = false;
    }

    private void OnLoad()
    {
        IsLoaded = true;
    }
}
