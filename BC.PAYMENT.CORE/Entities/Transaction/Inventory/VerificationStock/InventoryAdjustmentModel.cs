namespace BC.PAYMENT.CORE.Entities.Transaction.Inventory.VerificationStock;

public class InventoryAdjustmentModel
{
    public int Sequence { get; set; }
    public string RecType { get; set; }
    public int MovPrd { get; set; }
    public string MovRef { get; set; }
    public string MovLine { get; set; }
    public string Location { get; set; }
    public string ItemCode { get; set; }
    public string MovDate { get; set; }
    public string StatusInv { get; set; }
    public string IRStat { get; set; }
    public int BatchNo { get; set; }
    public string BatchLine { get; set; }
    public string LineRef { get; set; }
    public int Quantity { get; set; }
    public double Cost { get; set; }
    public double TotalPrice => Quantity * Cost;
    public string MovUnits { get; set; }
    public string MovType { get; set; }
    public string UpdatePhysical { get; set; }
    public string UpdateOrder { get; set; }
    public string AllocRef { get; set; }
    public string AccountCode { get; set; }
    public string AssetCode { get; set; }
    public string AnalM0 { get; set; }
    public string AnalM1 { get; set; }
    public string AnalM2 { get; set; }
    public string AnalM3 { get; set; }
    public string AnalM4 { get; set; }
    public string AnalM5 { get; set; }
    public string AnalM6 { get; set; }
    public string AnalM7 { get; set; }
    public string AnalM8 { get; set; }
    public string AnalM9 { get; set; }
    public string OrigLineNo { get; set; }
    public double PoValue { get; set; }
    public string IdEntered { get; set; }
    public string IdAlloc { get; set; }
    public string Status { get; set; }
    public string DbCode { get; set; }
    public int Period { get; set; }

    public string StatusType => Status switch
    {
        "ត្រឹមត្រូវ" => "",
        "ខ្វះ" => "ADJ+",
        "លើស" => "ADJ-",
        _ => ""
    };
}