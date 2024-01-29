using Microsoft.Extensions.Localization;
using System.Reflection.Metadata;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Console
{
    private string? ClassString => CssBuilder.Default("card console")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private string? BodyStyleString => CssBuilder.Default()
        .AddClass($"height: {Height}px;", Height > 0)
        .Build();

    private static string? GetClassString(ConsoleMessageItem item) => CssBuilder.Default()
        .AddClass($"text-{item.Color.ToDescriptionString()}", item.Color != Color.None)
        .Build();

    private string? AutoScrollString => IsAutoScroll ? "auto" : null;

    [Parameter]
    [NotNull]
    public IEnumerable<ConsoleMessageItem>? Items { get; set; }

    [Parameter]
    public string? HeaderText { get; set; }

    [Parameter]
    public string? LightTitle { get; set; }

    [Parameter]
    public bool IsFlashLight { get; set; } = true;

    [Parameter]
    public Color LightColor { get; set; } = Color.Success;

    [Parameter]
    public bool ShowLight { get; set; } = true;

    [Parameter]
    public string? AutoScrollText { get; set; }

    [Parameter]
    public bool ShowAutoScroll { get; set; }

    [Parameter]
    public bool IsAutoScroll { get; set; } = true;

    [Parameter]
    public string? ClearButtonText { get; set; }

    [Parameter]
    [NotNull]
    public string? ClearButtonIcon { get; set; }

    [Parameter]
    public Color ClearButtonColor { get; set; } = Color.Secondary;

    [Parameter]
    public Action? OnClear { get; set; }

    [Parameter]
    public int Height { get; set; }

    [Parameter]
    public RenderFragment? FooterTemplate { get; set; }

    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    [Parameter]
    public RenderFragment<ConsoleMessageItem>? ItemTemplate { get; set; }

    protected bool ShowFooter => OnClear != null || ShowAutoScroll || FooterTemplate != null;

    [Inject]
    [NotNull]
    private IStringLocalizer<Console>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        HeaderText ??= Localizer[nameof(HeaderText)];
        LightTitle ??= Localizer[nameof(LightTitle)];
        ClearButtonText ??= Localizer[nameof(ClearButtonText)];
        AutoScrollText ??= Localizer[nameof(AutoScrollText)];

        ClearButtonIcon ??= IconTheme.GetIconByKey(ComponentIcons.ConsoleClearButtonIcon);
        Items ??= Enumerable.Empty<ConsoleMessageItem>();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(!firstRender)
        {
            await InvokeVoidAsync("update", Id);
        }
    }

    public void ClearConsole()
    {
        OnClear?.Invoke();
    }
}
