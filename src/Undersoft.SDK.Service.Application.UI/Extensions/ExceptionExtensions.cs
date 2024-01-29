using System.Collections.Specialized;
using System.Text;

namespace Undersoft.SDK.Service.Application.Components;

public static class ExceptionExtensions
{
    public static string Format(this Exception exception, NameValueCollection? collection = null)
    {
        var logger = new StringBuilder();

        if (collection != null)
        {
            foreach (string key in collection)
            {
                logger.AppendFormat("{0}: {1}", key, collection[key]);
                logger.AppendLine();
            }
        }
        logger.AppendFormat("{0}: {1}", nameof(Exception.Message), exception.Message);
        logger.AppendLine();

        logger.AppendLine(new string('*', 45));
        logger.AppendFormat("{0}: {1}", nameof(Exception.StackTrace), exception.StackTrace);
        logger.AppendLine();

        return logger.ToString();
    }

    public static MarkupString FormatMarkupString(this Exception exception, NameValueCollection? collection = null)
    {
        var message = Format(exception, collection);
        return new MarkupString(message.Replace(Environment.NewLine, "<br />"));
    }
}
