using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using System.Collections.ObjectModel;

namespace Undersoft.SDK.Service.Application.Components;

public class PresenterAuthorizeView : ComponentBase
{
    [Parameter]
    [NotNull]
    public Type? Type { get; set; }

    [Parameter]
    public IReadOnlyDictionary<string, object>? Parameters { get; set; }

    [Parameter]
    public RenderFragment? NotAuthorized { get; set; }

    [Parameter]
    public object? Resource { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationState { get; set; }

    [Inject]
    private IAuthorizationPolicyProvider? AuthorizationPolicyProvider { get; set; }

    [Inject]
    private IAuthorizationService? AuthorizationService { get; set; }

#if NET6_0_OR_GREATER
    [Inject]
    [NotNull]
    private NavigationManager? NavigationManager { get; set; }
#endif

    private bool Authorized { get; set; }

    protected override async Task OnInitializedAsync()
    {
        Authorized = Type == null
            || await Type.IsAuthorizedAsync(AuthenticationState, AuthorizationPolicyProvider, AuthorizationService, Resource);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Authorized && Type != null)
        {
            var index = 0;
            builder.OpenComponent(index++, Type);
            foreach (var kv in (Parameters ?? new ReadOnlyDictionary<string, object>(new Dictionary<string, object>())))
            {
                builder.AddAttribute(index++, kv.Key, kv.Value);
            }
#if NET6_0_OR_GREATER
            BuildQueryParameters();
#endif
            builder.CloseComponent();
        }
        else
        {
            builder.AddContent(0, NotAuthorized);
        }

#if NET6_0_OR_GREATER
        void BuildQueryParameters()
        {
            var queryParameterSupplier = QueryParameterValueSupplier.ForType(Type);
            if (queryParameterSupplier is not null)
            {
                var url = NavigationManager.Uri;
                var queryStartPos = url.IndexOf('?');
                var query = queryStartPos < 0 ? default : url.AsMemory(queryStartPos);
                queryParameterSupplier.RenderParametersFromQueryString(builder, query);
            }
        }
#endif
    }
}
