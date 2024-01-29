namespace Undersoft.SDK.Service.Application.Components;

public class InvoiceEntity
{
    public string? ServiceType { get; set; }

    public string? SheetNum { get; set; }

    public string? InvoiceType { get; set; }

    public string? InvoiceTypeOrg { get; set; }

    public string InvoiceCode { get; set; } = string.Empty;

    public string InvoiceNum { get; set; } = string.Empty;

    public string? InvoiceTag { get; set; }

    public string InvoiceDate { get; set; } = string.Empty;

    public string? MachineNum { get; set; }

    public string? MachineCode { get; set; }

    public string? CheckCode { get; set; }

    public string? PurchaserName { get; set; }

    public string? PurchaserRegisterNum { get; set; }

    public string? PurchaserAddress { get; set; }

    public string? PurchaserBank { get; set; }

    public string? Password { get; set; }

    public string? Province { get; set; }

    public string? City { get; set; }

    public string? Agent { get; set; }

    public string? Remarks { get; set; }

    public string? SellerName { get; set; }

    public string? SellerAddress { get; set; }

    public string? SellerBank { get; set; }

    public string? SellerRegisterNum { get; set; }

    public string? TotalAmount { get; set; }

    public string? TotalTax { get; set; }

    public string? AmountInWords { get; set; }

    public string? AmountInFiguers { get; set; }

    public string? Payee { get; set; }

    public string? Checker { get; set; }

    public string? NoteDrawer { get; set; }

    public List<CommodityEntity>? CommodityName { get; set; }

    public List<CommodityTaxRateEntity>? CommodityTaxRate { get; set; }
}
