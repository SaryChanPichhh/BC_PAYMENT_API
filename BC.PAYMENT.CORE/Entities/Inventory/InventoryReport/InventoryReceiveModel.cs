namespace BC.PAYMENT.CORE.Entities.Inventory.InventoryReport;

public class InventoryReceiveModel
{
    public int RowNumber { get; set; }
    public int Id { get; set; }
    public string DbCode { get; set; }
    public string WareCode { get; set; }
    public string WareDesc { get; set; }
    public string SupplierCode { get; set; }
    public string ItemCode { get; set; }
    public bool Completed { get; set; }
    public int OrderedQty { get; set; }
    public int ReceivedQty { get; set; }
    public int ReceivedAmount { get; set; }
    public int TotalReceived { get; set; }
    public int NotReceivedQty { get; set; }
    public int SubTotal { get; set; }
    public string OrderedBy { get; set; }
    public string ReceivedBy { get; set; }
    public DateTime OrderedDate { get; set; }
    public DateTime ReceivedDate { get; set; }
}