namespace BC.PAYMENT.CORE.Entities;

public class ScStock
{
    public int StockId { get; set; }
    public string DbCode { get; set; } = string.Empty;
    public string StockName { get; set; } = string.Empty;
    public string Warehouse { get; set; } = string.Empty;
    public string StockController { get; set; } = string.Empty;
    public string Participation { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public DateTime? CountDate { get; set; }
    public bool Status { get; set; }
    public string CreatedBy { get; set; } = string.Empty;

    public string UserName
    {
        get => CreatedBy;
        set => CreatedBy = value;
    }

    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}