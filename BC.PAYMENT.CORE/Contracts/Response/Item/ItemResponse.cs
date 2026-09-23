namespace BC.PAYMENT.CORE.Contracts.Response.Item;

public class ItemResponse
{
    public string? ItemCode { get; set; }
    public string? ItemBarcode { get; set; }
    public string? ItemDesc { get; set; }
    public decimal? ItemCost { get; set; }
    public string? ItemDescKh { get; set; }
}