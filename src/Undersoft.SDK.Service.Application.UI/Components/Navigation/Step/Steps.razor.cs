namespace Undersoft.SDK.Service.Application.Components;

public partial class Steps
{
    private string? ClassString => CssBuilder.Default("steps")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    private StepItem? CurrentStep { get; set; }

    [Parameter]
    [NotNull]
    public IEnumerable<StepItem>? Items { get; set; }

    [Parameter]
    public bool IsVertical { get; set; }

    [Parameter]
    public bool IsCenter { get; set; }

    [Parameter]
    public StepStatus Status { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public Func<StepStatus, Task>? OnStatusChanged { get; set; }

    [Inject]
    [NotNull]
    private IIconTheme? IconTheme { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        var origiContent = ChildContent;
        ChildContent = new RenderFragment(builder =>
        {
            var index = 0;
            builder.OpenElement(index++, "div");
            builder.AddAttribute(index++, "class", CssBuilder.Default("steps-header")
                .AddClass("steps-horizontal", !IsVertical)
                .AddClass("steps-vertical", IsVertical)
                .Build());
            foreach (var item in Items)
            {
                builder.AddContent(index++, RenderStep(item));
            }
            builder.AddContent(index++, origiContent);
            builder.CloseElement();

            if (CurrentStep?.Template != null)
            {
                builder.OpenElement(index++, "div");
                builder.AddAttribute(index++, "class", "steps-body");
                builder.AddContent(index++, CurrentStep.Template);
                builder.CloseElement();
            }
        });
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Items = Enumerable.Empty<StepItem>();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        CurrentStep = null;
        if (Items.Any())
        {
            CurrentStep = Items.Where(i => i.Status != StepStatus.Wait).LastOrDefault();
            var status = CurrentStep?.Status ?? StepStatus.Wait;
            if (Status != status)
            {
                Status = status;
                if (OnStatusChanged != null)
                {
                    await OnStatusChanged.Invoke(Status);
                }
            }
        }
    }

    protected virtual RenderFragment RenderStep(StepItem item) => new(builder =>
    {
        item.Space = ParseSpace(item.Space);
        var index = 0;
        builder.OpenComponent<Step>(index++);
        builder.SetKey(item);
        builder.AddAttribute(index++, nameof(Step.Title), item.Title);
        builder.AddAttribute(index++, nameof(Step.Icon), item.Icon);
        builder.AddAttribute(index++, nameof(Step.Description), item.Description);
        builder.AddAttribute(index++, nameof(Step.Space), item.Space);
        builder.AddAttribute(index++, nameof(Step.Status), item.Status);
        builder.AddAttribute(index++, nameof(Step.IsLast), item == Items.Last());
        builder.AddAttribute(index++, nameof(Step.IsCenter), IsCenter);
        builder.AddAttribute(index++, nameof(Step.StepIndex), Items.ToList().IndexOf(item));
        builder.CloseComponent();
    });

    private string ParseSpace(string? space)
    {
        if (!string.IsNullOrEmpty(space) && !double.TryParse(space.TrimEnd('%'), out _)) space = null;
        if (string.IsNullOrEmpty(space)) space = $"{Math.Round(100 * 1.0d / Math.Max(1, Items.Count() - 1), 2)}%";
        return space;
    }
}
