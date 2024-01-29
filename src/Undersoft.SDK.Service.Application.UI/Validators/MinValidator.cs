namespace Undersoft.SDK.Service.Application.Components;

public class MinValidator : MaxValidator
{
    protected override bool Validate(int count) => count >= Value;

    protected override string GetErrorMessage() => ErrorMessage ?? "Select at least {0} items";
}
