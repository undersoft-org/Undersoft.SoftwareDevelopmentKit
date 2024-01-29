namespace Undersoft.SDK.Service.Application.Components;

public interface IEditorItem
{
    Type PropertyType { get; }

    bool Editable { get; set; }

    bool Readonly { get; set; }

    bool IsReadonlyWhenAdd { get; set; }

    bool IsReadonlyWhenEdit { get; set; }

    public bool SkipValidate { get; set; }

    string? Text { get; set; }

    bool? ShowLabelTooltip { get; set; }

    string? PlaceHolder { get; set; }

    IEnumerable<SelectedItem>? Items { get; set; }

    object? Step { get; set; }

    int Rows { get; set; }

    RenderFragment<object>? EditTemplate { get; set; }

    Type? ComponentType { get; set; }

    IEnumerable<KeyValuePair<string, object>>? ComponentParameters { get; set; }

    IEnumerable<SelectedItem>? Lookup { get; set; }

    bool ShowSearchWhenSelect { get; set; }

    bool IsPopover { get; set; }

    StringComparison LookupStringComparison { get; set; }

    string? LookupServiceKey { get; set; }

    List<IValidator>? ValidateRules { get; set; }

    string GetDisplayName();

    string GetFieldName();

    int Order { get; set; }

    string? GroupName { get; set; }

    int GroupOrder { get; set; }
}
