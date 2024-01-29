using Microsoft.AspNetCore.Components.Web;

namespace Undersoft.SDK.Service.Application.Components;

public partial class Button
{
    [Parameter]
    public bool IsAutoFocus { get; set; }

    protected EventCallback<MouseEventArgs> OnClickButton { get; set; }

    protected ElementReference ButtonElement { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        OnClickButton = EventCallback.Factory.Create<MouseEventArgs>(this, async () =>
        {
            if (IsAsync && ButtonType == ButtonType.Button)
            {
                IsAsyncLoading = true;
                ButtonIcon = LoadingIcon;
                IsDisabled = true;
            }

            if (IsAsync)
            {
                await Task.Run(() => InvokeAsync(HandlerClick));
            }
            else
            {
                await HandlerClick();
            }

            if (IsAsync && ButtonType == ButtonType.Button)
            {
                ButtonIcon = Icon;
                IsDisabled = false;
                IsAsyncLoading = false;
            }
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            if (IsAutoFocus)
            {
                await FocusAsync();
            }
        }
    }

    public ValueTask FocusAsync() => ButtonElement.FocusAsync();

    protected virtual async Task HandlerClick()
    {
        if (OnClickWithoutRender != null)
        {
            if (!IsAsync)
            {
                IsNotRender = true;
            }
            await OnClickWithoutRender.Invoke();
        }
        if (OnClick.HasDelegate)
        {
            await OnClick.InvokeAsync();
        }
    }
}
