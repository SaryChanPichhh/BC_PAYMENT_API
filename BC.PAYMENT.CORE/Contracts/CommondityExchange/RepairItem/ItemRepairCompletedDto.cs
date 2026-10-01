namespace BC.PAYMENT.CORE.DTO.CommondityExchange.RepairItem;

public class ItemRepairCompletedDto : Customer
{
    public int Id { get; set; }
    public int RepairCompletedId { get; set; }
    public int RequestDetailId { get; set; }
    public int RequestRepairId { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; }
    public string RepairNote { get; set; }
    public string RepairStatus { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Total { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Transaction { get; set; }
    public string Note { get; set; }

    public string RepairDescription => RepairStatus == "Repairable"
        ? Enums.RepairStatus.Repairable.GetDescription()
        : Enums.RepairStatus.Unrepairable.GetDescription();
}

public class SwitchItemTypeDto
{
    public int RequestDetailId { get; set; }
    public string Description { get; set; }
}

public class IssuanceParamDto
{
    public List<IssuanceInvoiceDto> OldInvoiceIssuance { get; set; }
    public NewInvoiceCompletedDto NewInvoiceIssuance { get; set; }
    public CustomerRespondDto CustomerWhoRepairGoods { get; set; }
}

public class IssuanceInvoiceDto
{
    public int RepairCompletedId { get; set; }
    public int RequestDetailId { get; set; }
    public int RequestRepairId { get; set; }
    public string ItemCode { get; set; }
    public int Quantity { get; set; }
    public string Description { get; set; }
    public decimal? Total => UnitPrice * Quantity;
    public decimal? UnitPrice { get; set; }
    public string ItemTransaction { get; set; }
}

public class NewInvoiceCompletedDto
{
    public string TransactionCode { get; set; }
    public string SaleType { get; set; }
    public string Warehouse { get; set; }
    public string Description { get; set; }
    public DateTime InvoiceDate { get; set; }
}

public class CompletedRepairItemDto
{
    public string? TransactionCode { get; set; }
    public int? Quantity { get; set; }
    public string? Description { get; set; }
    public string? CustomerCode { get; set; }
    public string? ItemCode { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class CompletedRepairItemPostDto
{
    public string TransactionCode { get; set; }
    public string CustomerCode { get; set; }
    public string RepairStatus { get; set; }
    public string Reason { get; set; }
    public string? RepairToolCode { get; set; }
    public string ItemCode { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public string? ItemDescription { get; set; }
    public string ItemStatus { get; set; }
}

public class ItemBeingRepaired
{
    public string ItemCode { get; set; }
    public string CustomerCode { get; set; }
    public string Description { get; set; }
    public string Transaction { get; set; }
    public string CreatedBy { get; set; }
    public int? CompletedQuantity { get; set; }
    public int RepairQuantity { get; set; }
    public int CurrentQuantity => RepairQuantity - (CompletedQuantity ?? 0);
    public string RepairDescription { get; set; }
    public string RepairToolCode { get; set; }
    public string ItemStatus { get; set; }
}

public class ItemRepairItemReceivedRespondDto : Customer
{
    public DateTime CreatedDate { get; set; }
    public string Status { get; set; }

    public string StatusText => Status.Trim() switch
    {
        "Completed" or "Yes" => "បានទទួល",
        "Pending" or "No" => "មិនទាន់បានទទួល",
        _ => "មិនទាន់បានទទួល"
    };

    public string TransactionCode { get; set; }
    public int Id { get; set; }
    public string ItemCode { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
}

public class ItemRepairCompletedRespondDto
{
    public int RepairCompletedId { get; set; }
    public string Description { get; set; }
    public int RequestDetailId { get; set; }
    public string ItemCode { get; set; }
    public string Area { get; set; }
    public string Market { get; set; }
    public string Store { get; set; }
    public int Quantity { get; set; }
    public string RepairStatus { get; set; }
    public string Transaction { get; set; }
    public string Note { get; set; }
    public DateTime CreatedDate { get; set; }
    public decimal? Total { get; set; }
    public int RequestRepairId { get; set; }

    public string RepairDescription => RepairStatus == "Repairable"
        ? Enums.RepairStatus.Repairable.GetDescription()
        : Enums.RepairStatus.Unrepairable.GetDescription();
}