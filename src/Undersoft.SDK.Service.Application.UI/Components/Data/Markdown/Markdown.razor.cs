using Microsoft.AspNetCore.Components;
using System.Reflection.Metadata;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Markdown : IAsyncDisposable
{
    [Parameter]
    public int Height { get; set; } = 300;

    [Parameter]
    public int MinHeight { get; set; } = 200;

    [Parameter]
    public InitialEditType InitialEditType { get; set; }

    [Parameter]
    public PreviewStyle PreviewStyle { get; set; }

    [Parameter]
    public string? Language { get; set; }

    [Parameter]
    public string? Placeholder { get; set; }

    [Parameter]
    public string? Html { get; set; }

    [Parameter]
    public EventCallback<string> HtmlChanged { get; set; }

    [Parameter]
    public bool? IsViewer { get; set; }

    [Parameter]
    public bool IsDark { get; set; } = false;

    [Parameter]
    public bool EnableHighlight { get; set; } = false;

    private MarkdownOption Option { get; } = new();

    [NotNull]
    private IJSObjectReference? Module { get; set; }

    [NotNull]
    private DotNetObjectReference<Markdown>? Interop { get; set; }

    private ElementReference Element { get; set; }

    protected string? GetClassString() => CssBuilder.Default()
        .AddClass(CssClass).AddClass(ValidCss)
        .Build();

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Option.PreviewStyle = PreviewStyle.ToDescriptionString();
        Option.InitialEditType = InitialEditType.ToDescriptionString();
        Option.Language = Language;
        Option.Placeholder = Placeholder;
        Option.Height = $"{Height}px";
        Option.MinHeight = $"{MinHeight}px";
        Option.InitialValue = Value ?? "";
        Option.Viewer = IsViewer;
        Option.Theme = IsDark ? "dark" : "light";
        Option.EnableHighlight = EnableHighlight;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Module = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BootstrapBlazor.Markdown/Components/Markdown/Markdown.razor.js");
            Interop = DotNetObjectReference.Create(this);
            await Module.InvokeVoidAsync("init", Element, Interop, Option, nameof(Update));
        }
    }

    [JSInvokable]
    public async Task Update(string[] vals)
    {
        if (vals.Length == 2)
        {
            CurrentValueAsString = vals[0];

            var hasChanged = !EqualityComparer<string>.Default.Equals(vals[1], Html);
            if (hasChanged)
            {
                Html = vals[1];
                if (HtmlChanged.HasDelegate)
                {
                    await HtmlChanged.InvokeAsync(Html);
                }
            }

            if (ValidateForm != null)
            {
                StateHasChanged();
            }
        }
    }

    public new async ValueTask SetValue(string value)
    {
        CurrentValueAsString = value;
        await Module.InvokeVoidAsync("update", Element, Value);
    }

    public ValueTask DoMethodAsync(string method, params object[] parameters) => Module.InvokeVoidAsync("invoke", Element, method, parameters);

    #region Dispose
    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            Interop?.Dispose();

            if (Module != null)
            {
                await Module.InvokeVoidAsync("dispose", Element);
                await Module.DisposeAsync();
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
