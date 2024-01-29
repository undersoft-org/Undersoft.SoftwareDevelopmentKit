namespace Undersoft.SDK.Service.Application.Components;

public abstract class AutoGenerateBaseAttribute : Attribute
{
    public bool Editable { get; set; } = true;

    public bool Readonly { get; set; }

    public bool Sortable { get; set; }

    public bool Filterable { get; set; }

    public bool Searchable { get; set; }

    public bool TextWrap { get; set; }

    public bool TextEllipsis { get; set; }

    public Alignment Align { get; set; }

    public bool ShowTips { get; set; }

    public bool ShowCopyColumn { get; set; }
}
