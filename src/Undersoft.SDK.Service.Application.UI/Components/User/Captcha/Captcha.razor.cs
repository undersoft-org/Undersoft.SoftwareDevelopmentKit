using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Captcha : IDisposable
{
    private static Random ImageRandomer { get; } = new Random();

    private int OriginX { get; set; }

    private JSInterop<Captcha>? Interop { get; set; }

    private ElementReference CaptchaElement { get; set; }

    private string? StyleString => CssBuilder.Default()
        .AddClass($"width: {Width + 42}px;", Width > 0)
        .Build();

    private string? FailedStyle => CssBuilder.Default()
        .AddClass($"width: {Width}px;", Width > 0)
        .AddClass($"height: {Height}px;", Height > 0)
        .Build();

    [Parameter]
    [NotNull]
    public string? HeaderText { get; set; }

    [Parameter]
    [NotNull]
    public string? BarText { get; set; }

    [Parameter]
    [NotNull]
    public string? FailedText { get; set; }

    [Parameter]
    [NotNull]
    public string? LoadText { get; set; }

    [Parameter]
    public string? TryText { get; set; }

    [Parameter]
    public Action<bool>? OnValid { get; set; }

    [Parameter]
    public string ImagesPath { get; set; } = "images";

    [Parameter]
    public string ImagesName { get; set; } = "Pic.jpg";

    [Parameter]
    public Func<string>? GetImageName { get; set; }

    [Parameter]
    public int Offset { get; set; } = 5;

    [Parameter]
    public int Width { get; set; } = 280;

    [Parameter]
    public int SideLength { get; set; } = 42;

    [Parameter]
    public int Diameter { get; set; } = 9;

    [Parameter]
    public int Height { get; set; } = 155;

    [Parameter]
    [NotNull]
    public string? RefreshIcon { get; set; }

    [Parameter]
    [NotNull]
    public string? BarIcon { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<Captcha>? Localizer { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        HeaderText ??= Localizer[nameof(HeaderText)];
        BarText ??= Localizer[nameof(BarText)];
        FailedText ??= Localizer[nameof(FailedText)];
        LoadText ??= Localizer[nameof(LoadText)];
        TryText ??= Localizer[nameof(TryText)];

        RefreshIcon ??= IconTheme.GetIconByKey(ComponentIcons.CaptchaRefreshIcon);
        BarIcon ??= IconTheme.GetIconByKey(ComponentIcons.CaptchaBarIcon);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await Reset();
        }
    }

    protected Task OnClickRefresh() => Reset();

    [JSInvokable]
    public Task<bool> Verify(int offset, IEnumerable<int> trails)
    {
        var ret = Math.Abs(offset - OriginX) < Offset && CalcStddev(trails);
        OnValid?.Invoke(ret);
        return Task.FromResult(ret);
    }

    private CaptchaOption GetCaptchaOption()
    {
        var option = new CaptchaOption()
        {
            Width = Width,
            Height = Height,
            SideLength = SideLength,
            Diameter = Diameter
        };
        option.BarWidth = option.SideLength + option.Diameter * 2 + 6;  
        var start = option.BarWidth + 10;
        var end = option.Width - start;
        option.OffsetX = Convert.ToInt32(Math.Ceiling(ImageRandomer.Next(0, 100) / 100.0 * (end - start) + start));
        OriginX = option.OffsetX;

        start = 10 + option.Diameter * 2;
        end = option.Height - option.SideLength - 10;
        option.OffsetY = Convert.ToInt32(Math.Ceiling(ImageRandomer.Next(0, 100) / 100.0 * (end - start) + start));

        if (GetImageName == null)
        {
            var index = Convert.ToInt32(ImageRandomer.Next(0, 8) / 1.0);
            var imageName = Path.GetFileNameWithoutExtension(ImagesName);
            var extendName = Path.GetExtension(ImagesName);
            var fileName = $"{imageName}{index}{extendName}";
            option.ImageUrl = Path.Combine(ImagesPath, fileName);
        }
        else
        {
            option.ImageUrl = GetImageName();
        }

        return option;
    }

    private static bool CalcStddev(IEnumerable<int> trails)
    {
        var ret = false;
        if (trails.Any())
        {
            var average = trails.Sum() * 1.0 / trails.Count();
            var dev = trails.Select(t => t - average);
            var stddev = Math.Sqrt(dev.Sum() * 1.0 / dev.Count());
            ret = stddev != 0;
        }
        return ret;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (Interop != null)
            {
                Interop.Dispose();
                Interop = null;
            }
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task Reset()
    {
        var option = GetCaptchaOption();
        if (Interop == null)
        {
            Interop = new JSInterop<Captcha>(JSRuntime);
        }

        await Interop.InvokeVoidAsync(this, CaptchaElement, "captcha", nameof(Verify), option);
    }
}
