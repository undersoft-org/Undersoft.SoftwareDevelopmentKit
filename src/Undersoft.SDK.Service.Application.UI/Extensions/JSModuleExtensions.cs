namespace Undersoft.SDK.Service.Application.Components;

public static class JSModuleExtensions
{
    private static string? _tick;
    private static string? GetVersion()
    {
        _tick ??= DateTime.Now.ToString("HHmmss");
        return _tick;
    }

    public static async Task<JSModule> LoadModule2(this IJSRuntime jsRuntime, string fileName, bool relative = true)
    {
        var filePath = relative ? $"./_content/BootstrapBlazor/modules/{fileName}.js" : fileName;
        var jSObjectReference = await jsRuntime.InvokeAsync<IJSObjectReference>(identifier: "import", $"{filePath}?v={GetVersion()}");
        return new JSModule(jSObjectReference);
    }

    public static async Task<JSModule2<TValue>> LoadModule2<TValue>(this IJSRuntime jsRuntime, string fileName, TValue value, bool relative = true) where TValue : class
    {
        var filePath = relative ? $"./_content/BootstrapBlazor/modules/{fileName}.js" : fileName;
        var jSObjectReference = await jsRuntime.InvokeAsync<IJSObjectReference>(identifier: "import", $"{filePath}?v={GetVersion()}");
        return new JSModule2<TValue>(jSObjectReference, value);
    }

    public static async Task<JSModule> LoadModule(this IJSRuntime jsRuntime, string fileName, bool relative = true)
    {
        var filePath = relative ? $"./_content/BootstrapBlazor/Components/{fileName}/{fileName}.razor.js" : fileName;
        var jSObjectReference = await jsRuntime.InvokeAsync<IJSObjectReference>(identifier: "import", $"{filePath}?v={GetVersion()}");
        return new JSModule(jSObjectReference);
    }

    public static async Task<JSModule2<TValue>> LoadModule3<TValue>(this IJSRuntime jsRuntime, string fileName, TValue value, bool relative = true) where TValue : class
    {
        var filePath = relative ? $"./_content/BootstrapBlazor/Components/{fileName}/{fileName}.razor.js" : fileName;
        var jSObjectReference = await jsRuntime.InvokeAsync<IJSObjectReference>(identifier: "import", $"{filePath}?v={GetVersion()}");
        return new JSModule2<TValue>(jSObjectReference, value);
    }

    public static string GetTypeModuleName(this Type type)
    {
        var name = type.Name;
        if (type.IsGenericType)
        {
            var index = type.Name.IndexOf('`');
            name = type.Name[..index];
        }
        return name;
    }
}
