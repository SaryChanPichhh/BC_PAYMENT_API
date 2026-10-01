namespace BC.PAYMENT.CORE.Entities.Inventory;

public class WarehousePresetModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Note { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public DateTime UpdatedDate { get; set; }
    public string UpdatedBy { get; set; }
}