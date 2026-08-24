namespace BC.PAYMENT.CORE.DTO.ViewStock;

public sealed class ItemSaleStockDto
{
    public string? DbCode { get; set; }
    public string? ItemCode { get; set; }
    public string? ItemDesc { get; set; }
    public decimal SaleQty { get; set; }
    public string? ItemNameKhmer { get; set; }
    public decimal AvailableQty { get; set; }
    public bool StockStatus { get; set; }
    public string? StockStatusDesc { get; set; }
    public string? SaleStatus { get; set; }
    public string? ImageUrl { get; set; }
}