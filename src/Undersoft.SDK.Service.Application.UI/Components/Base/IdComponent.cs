using System.Diagnostics.CodeAnalysis;

namespace Undersoft.SDK.Service.Application.Components;

public abstract class IdComponent : Component
{
    [Parameter]
    [NotNull]
    public virtual string? Id { get; set; }

    [Inject]
    [NotNull]
    protected IComponentIdGenerator? ComponentIdGenerator { get; set; }

    protected virtual string? RetrieveId() => Id;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Id ??= ComponentIdGenerator.Generate(this);
    }
}
