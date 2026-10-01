namespace BC.PAYMENT.CORE.Entities.ClosingInventoryAndInvoice.ClosingInventory;

public class ClosingStockEntryModel
{
    public int Id { get; set; }
    public string DbCode { get; set; }
    public DateTime ClosingDate { get; set; }
    public string Location { get; set; }
    public string ItemCode { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
    public int? Order { get; set; }
    public int? Print { get; set; }
    public int? OpeningBalance { get; set; } // opening qty
    public int? PurchaseOrder { get; set; }
    public int? Sale { get; set; }
    public int? Transfer { get; set; }
    public int? CreditNote { get; set; }
    public int? InventoryAdjustment { get; set; }
    public int? Free => (OpeningBalance ?? 0) - (Math.Abs(Order ?? 0) + Math.Abs(Print ?? 0));

    public int? ClosingBalance => (OpeningBalance ?? 0) + (PurchaseOrder ?? 0) + (CreditNote ?? 0) -
        (Math.Abs(Sale ?? 0) + Math.Abs(Transfer ?? 0)) + (InventoryAdjustment ?? 0);

    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public int Year { get; set; }
    public ClosingEntryType ClosingEntryType { get; set; }
}