using Microsoft.FluentUI.AspNetCore.Components;
using Undersoft.SDK.Instant.Proxies;
using Undersoft.SDK.Instant.Rubrics;
using Undersoft.SDK.Uniques;

namespace Undersoft.SDK.Service.Application.UI.Generic;

public class GenericData<TModel> : Origin, IGenericData<TModel> where TModel : class, IOrigin, IInnerProxy
{
    public GenericData()
    {
        Data = typeof(TModel).New<TModel>();
    }

    public GenericData(TModel data, string title = "")
    {
        Data = data;
        Title = title;
    }

    public GenericData(TModel data, string title, params string[] displayList)
    {
        Data = data;
        Title = title;

        if (displayList != null && displayList.Any())
        {
            var rubrics = data.Proxy.Rubrics.Where(r => displayList.Contains(r.RubricName));
            rubrics.ForEach(r => r.Visible = true);
            DisplayRubrics.Add(rubrics);
        }
    }

    public TModel Data { get; set; } = default!;

    public string? Title { get; set; } = null;

    public string? Description { get; set; } = null;

    public string? Note { get; set; } = null;

    public string? Status { get; set; }

    public Icon Icon { get; set; } = new Icons.Regular.Size24.AppGeneric();

    public string? Logo { get; set; } = "~/img/logo.png";

    public string Height { get; set; } = "300px";

    public string Width { get; set; } = "300px";

    private string? nextPath = null;
    public string? NextPath { get { return nextPath; } set { nextPath = value; HaveNext = true; } }

    public string? NextInvoke { get; set; } = null;

    public bool IsCanceled { get; set; }

    public bool HaveNext { get; set; }

    public IRubric SelectedRubric { get; set; } = default!;

    public IRubrics DisplayRubrics { get; set; } = new MemberRubrics();

    public void SetRequired(params string[] requiredList)
    {
        var rubrics = Data.Proxy.Rubrics.Where(r => requiredList.Contains(r.RubricName));
        rubrics.ForEach(r => { r.Required = true; r.Visible = true; r.Editable = true; });
        DisplayRubrics.Put(rubrics);
    }

    public void SetVisible(params string[] visibleList)
    {
        var rubrics = Data.Proxy.Rubrics.Where(r => visibleList.Contains(r.RubricName));
        rubrics.ForEach(r => r.Visible = true);
        DisplayRubrics.Put(rubrics);
    }

    public void SetEditable(params string[] editableList)
    {
        var rubrics = Data.Proxy.Rubrics.Where(r => editableList.Contains(r.RubricName));
        rubrics.ForEach(r => r.Editable = true);
        DisplayRubrics.Put(rubrics);
    }

    public void SetDisplayName(params DisplayPair[] displayPairList)
    {
        var rubrics = displayPairList.ForEach(d => { var rubric = Data.Proxy.Rubrics[d.RubricName]; rubric.DisplayName = d.DisplayName; rubric.Visible = true; return rubric; });
        DisplayRubrics.Put(rubrics);
    }
}

public class DisplayPair : Identifiable
{
    public DisplayPair(string rubricName, string displayName)
    {
        RubricName = rubricName;
        DisplayName = displayName;
        Id = RubricName.UniqueKey32();
    }

    public string RubricName { get; set; }

    public string DisplayName { get; set; }
}
