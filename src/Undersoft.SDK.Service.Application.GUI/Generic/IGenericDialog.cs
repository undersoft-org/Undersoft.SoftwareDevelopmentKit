using Microsoft.FluentUI.AspNetCore.Components;
using Undersoft.SDK.Instant.Proxies;

namespace Undersoft.SDK.Service.Application.UI.Generic
{
    public interface IGenericDialog<TModel> where TModel : class, IOrigin, IInnerProxy
    {
        IGenericData<TModel>? Content { get; }
        IDialogReference? Reference { get; }

        Task Show(IGenericData<TModel> data);

        Task Show(IGenericData<TModel> data, Action<DialogParameters> setup);

        Task Show(Action<DialogParameters<TModel>> setup);
    }
}