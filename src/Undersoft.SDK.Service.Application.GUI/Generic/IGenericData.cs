using Microsoft.FluentUI.AspNetCore.Components;
using Undersoft.SDK.Instant.Proxies;
using Undersoft.SDK.Instant.Rubrics;

namespace Undersoft.SDK.Service.Application.UI.Generic
{
    public interface IGenericData<TModel> where TModel : class, IOrigin, IInnerProxy
    {
        TModel Data { get; set; }
        string? Description { get; set; }
        string? Note { get; set; }
        string? Status { get; set; }
        bool HaveNext { get; set; }
        string Height { get; set; }
        Icon? Icon { get; set; }
        bool IsCanceled { get; set; }
        string? Logo { get; set; }
        string? NextInvoke { get; set; }
        string? NextPath { get; set; }
        IRubrics DisplayRubrics { get; set; }
        string? Title { get; set; }
        string Width { get; set; }

        void SetRequired(params string[] requiredList);
        void SetVisible(params string[] visibleList);
        void SetEditable(params string[] editableList);
        void SetDisplayNames(params DisplayPair[] displayPairs);
    }
}