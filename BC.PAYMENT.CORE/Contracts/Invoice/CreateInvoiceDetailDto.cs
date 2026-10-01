namespace BC.PAYMENT.CORE.DTO.Invoice;

public class CreateInvoiceDetailDto
{
    public string ItemCode { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; }
    public int RepairCompletedId { get; set; }
    public int RequestDetailId { get; set; }
    public int RequestRepairId { get; set; }
    public string TransactionRef { get; set; }
    public string ItemTransaction { get; set; }
    public decimal? UnitPrice { get; set; }
    public string Store { get; set; }
    public decimal? Total => Quantity * UnitPrice;
}