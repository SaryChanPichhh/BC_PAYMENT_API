namespace BC.PAYMENT.CORE.DTO.Inventory;

public class WarehousePresetDto
{
    public string Name { get; set; }
    public string Note { get; set; }
    public bool Status { get; set; }
}

public class WarehousePresetPutDto : WarehousePresetDto
{
    public string Id { get; set; }
}