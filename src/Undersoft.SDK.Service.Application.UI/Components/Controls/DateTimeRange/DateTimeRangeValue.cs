namespace Undersoft.SDK.Service.Application.Components;

public class DateTimeRangeValue
{
    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public override string ToString()
    {
        var ret = "";
        if (Start != DateTime.MinValue)
        {
            ret = Start.ToString();
        }
        if (End != DateTime.MinValue)
        {
            ret = $"{ret} - {End}";
        }
        return ret;
    }
}
