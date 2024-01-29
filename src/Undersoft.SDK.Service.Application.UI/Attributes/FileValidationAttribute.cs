using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;

namespace Undersoft.SDK.Service.Application.Components;

[AttributeUsage(AttributeTargets.Property)]
public class FileValidationAttribute : ValidationAttribute
{
    private IStringLocalizer? Localizer { get; set; }

    public string[] Extensions { get; set; } = Array.Empty<string>();

    public long FileSize { get; set; }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ValidationResult? ret = null;

        if (value is IBrowserFile file)
        {
            Localizer = Utility.CreateLocalizer<UploadBase<object>>();
            if (Localizer != null)
            {
                if (Extensions.Any() && !Extensions.Contains(Path.GetExtension(file.Name), StringComparer.OrdinalIgnoreCase))
                {
                    var errorMessage = Localizer["FileExtensions", string.Join(", ", Extensions)];
                    ret = new ValidationResult(errorMessage.Value, GetMemberNames(validationContext));
                }
                if (ret == null && FileSize > 0 && file.Size > FileSize)
                {
                    var errorMessage = Localizer["FileSizeValidation", FileSize.ToFileSizeString()];
                    ret = new ValidationResult(errorMessage.Value, GetMemberNames(validationContext));
                }
            }
        }
        return ret;
    }

    private static IEnumerable<string>? GetMemberNames(ValidationContext validationContext)
    {
        return validationContext == null ? Enumerable.Empty<string>() : GetMemberNames();

        IEnumerable<string> GetMemberNames() => string.IsNullOrWhiteSpace(validationContext.MemberName) ? Enumerable.Empty<string>() : new string[] { validationContext.MemberName };
    }
}
