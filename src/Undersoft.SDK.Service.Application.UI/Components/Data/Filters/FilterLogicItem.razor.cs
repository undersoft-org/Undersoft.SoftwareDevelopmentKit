using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

public partial class FilterLogicItem
{
    private FilterLogic _value;
    private FilterLogic Value
    {
        get
        {
            _value = Logic;
            return _value;
        }
        set
        {
            _value = value;
            if (LogicChanged.HasDelegate) LogicChanged.InvokeAsync(value);
        }
    }

    [Parameter]
    public FilterLogic Logic { get; set; }

    [Parameter]
    public EventCallback<FilterLogic> LogicChanged { get; set; }

    private IEnumerable<SelectedItem>? Items { get; set; }

    [Inject]
    [NotNull]
    private IStringLocalizer<FilterLogicItem>? Localizer { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Items = new List<SelectedItem>()
        {
            new SelectedItem("And",Localizer["And"].Value),
            new SelectedItem("Or",Localizer["Or"].Value)
        };
    }
}
