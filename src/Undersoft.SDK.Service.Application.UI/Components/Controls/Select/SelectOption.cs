namespace Undersoft.SDK.Service.Application.Components;

public class SelectOption : ComponentBase
{
    [Parameter]
    public string? Text { get; set; }

    [Parameter]
    public string? Value { get; set; }

    [Parameter]
    public bool Active { get; set; }

    [Parameter]
    public bool IsDisabled { get; set; }

    [Parameter]
    public string? GroupName { get; set; }

    [CascadingParameter]
    private ISelect? Container { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Container?.Add(ToSelectedItem());
    }

    private SelectedItem ToSelectedItem() => new()
    {
        Active = Active,
        GroupName = GroupName ?? "",
        Text = Text ?? "",
        Value = Value ?? "",
        IsDisabled = IsDisabled
    };
}
