namespace BC.PAYMENT.CORE.Entities.CommondityExchange.RepairItem;

public class CompletedRepairItemModel : RefundItemDto
{
    public string? Transaction { get; set; }
    public string? CreatedBy { get; set; }
    public int? CompletedQuantity { get; set; }
    public int RepairQuantity { get; set; }
    public int CurrentQuantity => RepairQuantity - (CompletedQuantity ?? 0);
    public string? RepairDescription { get; set; }
    public string? RepairToolCode { get; set; }
    public string? ItemStatus { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? Status { get; set; }
}