using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Rendering;

namespace Undersoft.SDK.Service.Application.Components;

public class Block : Component
{
    [Parameter]
    public string? Name { get; set; }

    [Parameter]
    public IEnumerable<string>? Roles { get; set; }

    [Parameter]
    public IEnumerable<string>? Users { get; set; }

    [Parameter]
    public Func<string?, Task<bool>>? OnQueryCondition { get; set; }

    [Parameter]
    public bool? Condition { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? Authorized { get; set; }

    [Parameter]
    public RenderFragment? NotAuthorized { get; set; }

    [Inject]
    private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }

    private bool IsShow { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (Users != null || Roles != null)
        {
            IsShow = await ProcessAuthorizeAsync();
        }
        else if (Condition.HasValue)
        {
            IsShow = Condition.Value;
        }
        else if (OnQueryCondition != null)
        {
            IsShow = await OnQueryCondition(Name);
        }
    }

    private async Task<bool> ProcessAuthorizeAsync()
    {
        bool isAuthenticated = false;
        AuthenticationState? state = null;
        if (AuthenticationStateProvider != null)
        {
            state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        }
        if (state != null)
        {
            var identity = state.User.Identity;
            if (identity != null)
            {
                isAuthenticated = identity.IsAuthenticated;
            }
        }
        if (isAuthenticated)
        {
            if (Users?.Any() ?? false)
            {
                var userName = state!.User.Identity!.Name;
                isAuthenticated = Users.Any(i => i.Equals(userName, StringComparison.OrdinalIgnoreCase));
            }
            if (Roles?.Any() ?? false)
            {
                isAuthenticated = Roles.Any(i => state!.User.IsInRole(i));
            }
        }
        return isAuthenticated;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (IsShow)
        {
            builder.AddContent(0, Authorized ?? ChildContent);
        }
        else
        {
            builder.AddContent(0, NotAuthorized);
        }
    }
}
