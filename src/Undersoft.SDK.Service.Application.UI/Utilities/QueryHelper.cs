using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Encodings.Web;

namespace Undersoft.SDK.Service.Application.Components;

[ExcludeFromCodeCoverage]
public static class QueryHelper
{
    public static string AddQueryString(string uri, string name, string value) => AddQueryString(
            uri, new[] { new KeyValuePair<string, string?>(name, value) });

    public static string AddQueryString(string uri, IDictionary<string, string?> queryString) => AddQueryString(uri, (IEnumerable<KeyValuePair<string, string?>>)queryString);

    public static string AddQueryString(string uri, IEnumerable<KeyValuePair<string, StringValues>> queryString) => AddQueryString(uri, queryString.SelectMany(kvp => kvp.Value, (kvp, v) => KeyValuePair.Create<string, string?>(kvp.Key, v)));

    public static string AddQueryString(string uri, IEnumerable<KeyValuePair<string, string?>> queryString)
    {
        var anchorIndex = uri.IndexOf('#');
        var uriToBeAppended = uri;
        var anchorText = "";
        if (anchorIndex != -1)
        {
            anchorText = uri[anchorIndex..];
            uriToBeAppended = uri[..anchorIndex];
        }

        var queryIndex = uriToBeAppended.IndexOf('?');
        var hasQuery = queryIndex != -1;

        var sb = new StringBuilder();
        sb.Append(uriToBeAppended);
        foreach (var parameter in queryString)
        {
            if (parameter.Value == null)
            {
                continue;
            }

            sb.Append(hasQuery ? '&' : '?');
            sb.Append(UrlEncoder.Default.Encode(parameter.Key));
            sb.Append('=');
            sb.Append(UrlEncoder.Default.Encode(parameter.Value));
            hasQuery = true;
        }

        sb.Append(anchorText);
        return sb.ToString();
    }

    public static Dictionary<string, StringValues> ParseQuery(string? queryString) => ParseNullableQuery(queryString) ?? new Dictionary<string, StringValues>();

    public static Dictionary<string, StringValues>? ParseNullableQuery(string? queryString)
    {
        Dictionary<string, StringValues>? ret = null;
        var q = queryString.AsMemory();
        var payload = q.IsEmpty || q.Span[0] != '?'
            ? q
            : q[1..];

        while (!payload.IsEmpty)
        {
            ReadOnlyMemory<char> segment;
            var delimiterIndex = payload.Span.IndexOf('&');
            if (delimiterIndex >= 0)
            {
                segment = payload[..delimiterIndex];
                payload = payload[(delimiterIndex + 1)..];
            }
            else
            {
                segment = payload;
                payload = default;
            }

            var equalIndex = segment.Span.IndexOf('=');
            if (equalIndex >= 0)
            {
                ret ??= new();
                var v = Uri.UnescapeDataString(segment[(equalIndex + 1)..].ToString());
                ret.Add(segment[..equalIndex].ToString(), v);
            }
            else if (!segment.IsEmpty)
            {
                ret ??= new();
                ret.Add(segment.ToString(), default);
            }
        }
        return ret;
    }
}
