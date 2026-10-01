namespace BC.PAYMENT.CORE.Entities.Inventory.InventoryReport;

public class InventoryReportModel
{
    public string ItemCode { get; set; }
    public int Post { get; set; }
    public int Hold { get; set; }
    public int Release { get; set; }
    public int Total { get; set; }
}