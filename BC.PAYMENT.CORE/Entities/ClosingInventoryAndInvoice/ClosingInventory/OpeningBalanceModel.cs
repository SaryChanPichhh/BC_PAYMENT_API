namespace BC.PAYMENT.CORE.Entities.ClosingInventoryAndInvoice.ClosingInventory;

public class OpeningBalanceModel
{
    public string ItemCode { get; set; }
    public string Location { get; set; }
    public string ItemDesc { get; set; }
    public int UnitStock { get; set; }
    public int Physical { get; set; }
    public int OnOrder { get; set; }
    public int PickQty { get; set; }
    public int Free { get; set; }
    public string CreatedBy { get; set; }
    public string DbCode { get; set; }
}