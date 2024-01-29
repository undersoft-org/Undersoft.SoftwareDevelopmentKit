namespace Undersoft.SDK.Service.Application.Components;

public interface ICheckableNode<TItem> : IExpandableNode<TItem>
{
    CheckboxState CheckedState { get; set; }
}
