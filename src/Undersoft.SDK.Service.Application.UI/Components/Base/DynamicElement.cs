using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics.CodeAnalysis;

namespace Undersoft.SDK.Service.Application.Components;

public class DynamicElement : Component
{
    [Parameter]
    [NotNull]
    public string? TagName { get; set; } = "div";

    [Parameter]
    public bool TriggerClick { get; set; } = true;

    [Parameter]
    public bool PreventDefault { get; set; }

    [Parameter]
    public bool StopPropagation { get; set; }

    [Parameter]
    public Func<Task>? OnClick { get; set; }

    [Parameter]
    public bool TriggerDoubleClick { get; set; } = true;

    [Parameter]
    public Func<Task>? OnDoubleClick { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool GenerateElement { get; set; } = true;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (GenerateElement || IsTriggerClick() || IsTriggerDoubleClick())
        {
            builder.OpenElement(0, TagName);
            if (AdditionalAttributes != null)
            {
                builder.AddMultipleAttributes(1, AdditionalAttributes);
            }
        }

        if (IsTriggerClick())
        {
            builder.AddAttribute(2, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, e => OnClick!()));
        }

        if (IsTriggerDoubleClick())
        {
            builder.AddAttribute(3, "ondblclick", EventCallback.Factory.Create<MouseEventArgs>(this, e => OnDoubleClick!()));
        }

        if (IsTriggerClick() || IsTriggerDoubleClick())
        {
            builder.AddEventPreventDefaultAttribute(4, "onclick", PreventDefault);
            builder.AddEventStopPropagationAttribute(5, "onclick", StopPropagation);
        }

        builder.AddContent(6, ChildContent);

        if (GenerateElement || IsTriggerClick() || IsTriggerDoubleClick())
        {
            builder.CloseElement();
        }

        bool IsTriggerClick() => TriggerClick && OnClick != null;
        bool IsTriggerDoubleClick() => TriggerDoubleClick && OnDoubleClick != null;
    }
}
