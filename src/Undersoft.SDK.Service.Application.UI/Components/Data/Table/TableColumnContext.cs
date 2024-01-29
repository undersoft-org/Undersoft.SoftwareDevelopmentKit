namespace Undersoft.SDK.Service.Application.Components;

public class TableColumnContext<TItem, TValue>
{
    public TableColumnContext(TItem model, TValue value)
    {
        Row = model ?? throw new ArgumentNullException(nameof(model));
        Value = value;
    }

    [NotNull]
    public TItem Row { get; }

    public TValue Value { get; }
}
