namespace Undersoft.SDK.Service.Application.Components;

public class BaiDuIPLocator : DefaultIPLocator
{
    public BaiDuIPLocator()
    {
        Url = "https://sp0.baidu.com/8aQDcjqpAAV3otqbppnN2DJv/api.php?resource_id=6006&query={0}";
    }

    public IEnumerable<LocationInfo>? Data { get; set; }

    public string? Status { get; set; }

    public override Task<string?> Locate(IPLocatorOption option) => Locate<BaiDuIPLocator>(option);

    public override string ToString()
    {
        return Status == "0" ? (Data?.FirstOrDefault().Location ?? "XX XX") : "Error";
    }
}

public struct LocationInfo
{
    public string? Location { get; set; }
}
