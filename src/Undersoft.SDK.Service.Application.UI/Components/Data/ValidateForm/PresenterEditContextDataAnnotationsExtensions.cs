using Microsoft.AspNetCore.Components.Forms;

namespace Undersoft.SDK.Service.Application.Components;

internal static class PresenterEditContextDataAnnotationsExtensions
{
    public static EditContext AddEditContextDataAnnotationsValidation(this EditContext editContext, ValidateForm editForm, IServiceProvider provider)
    {
        var messages = new ValidationMessageStore(editContext);

        editContext.OnValidationRequested += async (sender, eventArgs) => await editForm.ValidateModel(sender as EditContext, messages, provider);
        editContext.OnFieldChanged += async (sender, eventArgs) => await editForm.ValidateField(editContext, messages, eventArgs, provider);
        return editContext;
    }

    private static async Task ValidateModel(this ValidateForm editForm, EditContext? editContext, ValidationMessageStore messages, IServiceProvider provider)
    {
        if (editContext != null)
        {
            var validationContext = new ValidationContext(editContext.Model, provider, null);
            var validationResults = new List<ValidationResult>();
            await editForm.ValidateObject(validationContext, validationResults);

            messages.Clear();
            foreach (var validationResult in validationResults.Where(v => !string.IsNullOrEmpty(v.ErrorMessage)))
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    messages.Add(editContext.Field(memberName), validationResult.ErrorMessage!);
                }
            }
            editContext.NotifyValidationStateChanged();
        }
    }

    private static async Task ValidateField(this ValidateForm editForm, EditContext editContext, ValidationMessageStore messages, FieldChangedEventArgs args, IServiceProvider provider)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(args.FieldIdentifier.Model, provider, null)
        {
            MemberName = args.FieldIdentifier.FieldName,
            DisplayName = args.FieldIdentifier.GetDisplayName()
        };

        await editForm.ValidateFieldAsync(validationContext, validationResults);

        messages.Clear(args.FieldIdentifier);
        messages.Add(args.FieldIdentifier, validationResults.Where(v => !string.IsNullOrEmpty(v.ErrorMessage)).Select(result => result.ErrorMessage!));

        editContext.NotifyValidationStateChanged();
    }
}
