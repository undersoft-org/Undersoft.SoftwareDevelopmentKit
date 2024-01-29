namespace Undersoft.SDK.Service.Application.Components;

public class GeolocationItem
{
    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public decimal Accuracy { get; set; }

    public decimal Altitude { get; set; }

    public decimal AltitudeAccuracy { get; set; }

    public decimal Heading { get; set; }

    public decimal Speed { get; set; }

    public long Timestamp { get; set; }

    public DateTime LastUpdateTime { get => UnixTimeStampToDateTime(Timestamp); }

    public decimal CurrentDistance { get; set; } = 0.0M;

    public decimal TotalDistance { get; set; } = 0.0M;

    public decimal LastLat { get; set; }

    public decimal LastLong { get; set; }

    private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }
}
