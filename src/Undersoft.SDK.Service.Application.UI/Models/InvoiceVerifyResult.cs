namespace Undersoft.SDK.Service.Application.Components;

public class InvoiceVerifyResult
{
    public string? VerifyFrequency { get; set; }

    public string? MachineCode { get; set; }

    public string? InvoiceNum { get; set; }

    public string? InvoiceDate { get; set; }

    public string? VerifyResult { get; set; }

    public string? InvalidSign { get; set; }

    public string? InvoiceCode { get; set; }

    public string? InvoiceType { get; set; }

    public string? CheckCode { get; set; }

    public string? VerifyMessage { get; set; }

    public InvoiceEntity? Invoice { get; set; }

    public bool Valid => VerifyResult == "0001";
}
