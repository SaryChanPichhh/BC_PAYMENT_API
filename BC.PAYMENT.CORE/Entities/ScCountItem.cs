namespace BC.PAYMENT.CORE.Entities;

public class ScCountItem
{
    public int Id { get; set; }
    public string DbCode { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ItemDesc { get; set; } = string.Empty;
    public string ItemDescKh { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public bool Status { get; set; }
    public int StockId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public string Period { get; set; } = string.Empty;
}