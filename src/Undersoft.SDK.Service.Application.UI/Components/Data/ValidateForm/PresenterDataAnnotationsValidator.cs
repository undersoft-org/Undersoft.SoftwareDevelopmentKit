using Microsoft.AspNetCore.Components.Forms;

namespace Undersoft.SDK.Service.Application.Components;

public class PresenterDataAnnotationsValidator : ComponentBase
{
    [CascadingParameter]
    [NotNull]
    private EditContext? CurrentEditContext { get; set; }

    [CascadingParameter]
    private ValidateForm? ValidateForm { get; set; }

    [Inject]
    [NotNull]
    private IServiceProvider? Provider { get; set; }

    protected override void OnInitialized()
    {
        if (ValidateForm == null)
        {
            throw new InvalidOperationException($"{nameof(Components.PresenterDataAnnotationsValidator)} requires a cascading " +
                $"parameter of type {nameof(Components.ValidateForm)}. For example, you can use {nameof(Components.PresenterDataAnnotationsValidator)} " +
                $"inside an {nameof(Components.ValidateForm)}.");
        }

        CurrentEditContext.AddEditContextDataAnnotationsValidation(ValidateForm, Provider);
    }

    internal bool Validate() => CurrentEditContext.Validate();
}
