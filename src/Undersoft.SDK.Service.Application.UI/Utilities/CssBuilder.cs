namespace Undersoft.SDK.Service.Application.Components;

public class CssBuilder
{
    private readonly List<string> stringBuffer;

    public static CssBuilder Default(string? value = null) => new CssBuilder(value);

    protected CssBuilder(string? value)
    {
        stringBuffer = new List<string>();
        AddClass(value);
    }

    public CssBuilder AddClass(string? value)
    {
        if (!string.IsNullOrEmpty(value)) stringBuffer.Add(value);
        return this;
    }

    public CssBuilder AddClass(string? value, bool when = true) => when ? AddClass(value) : this;

    public CssBuilder AddClass(string? value, Func<bool> when) => AddClass(value, when());

    public CssBuilder AddClass(Func<string?> value, bool when = true) => when ? AddClass(value()) : this;

    public CssBuilder AddClass(Func<string?> value, Func<bool> when) => AddClass(value, when());

    public CssBuilder AddClass(CssBuilder builder, bool when = true) => when ? AddClass(builder.Build()) : this;

    public CssBuilder AddClass(CssBuilder builder, Func<bool> when) => AddClass(builder, when());

    public CssBuilder AddClassFromAttributes(IDictionary<string, object>? additionalAttributes)
    {
        if (additionalAttributes != null && additionalAttributes.TryGetValue("class", out var c))
        {
            var classList = c?.ToString();
            AddClass(classList);
        }
        return this;
    }

    public CssBuilder AddStyleFromAttributes(IDictionary<string, object>? additionalAttributes)
    {
        if (additionalAttributes != null && additionalAttributes.TryGetValue("style", out var c))
        {
            var styleList = c?.ToString();
            AddClass(styleList);
        }
        return this;
    }

    public string? Build() => stringBuffer.Any() ? string.Join(" ", stringBuffer) : null;
}
