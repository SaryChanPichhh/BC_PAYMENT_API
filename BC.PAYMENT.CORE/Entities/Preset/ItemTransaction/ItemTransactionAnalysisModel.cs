namespace BC.PAYMENT.CORE.Entities.Preset.ItemTransaction;

public class ItemTransactionAnalysisModel
{
    public string No { get; set; }
    public string Location { get; set; }
    public string DbCode { get; set; }
    public string ItemCode { get; set; }
    public string ItemDesc { get; set; }
    public string DESCRIPTN { get; set; }
    public int? UnitStock { get; set; }
    public int Physical { get; set; }
    public int Free { get; set; }
    public int OnOrder { get; set; }
    public int PurQty { get; set; }
    public int SalQty { get; set; }
    public int TraQty { get; set; }
    public int CreQty { get; set; }
    public int AdjQty { get; set; }

    public string ITEM_CODE { get; set; }
    public string ITEM_DESC { get; set; }
    public int B16_PO { get; set; }
    public int BT7_PO { get; set; }
    public int KC7_PO { get; set; }
    public int SR7_PO { get; set; }
    public int SP7_PO { get; set; }
    public int KP7_PO { get; set; }
    public int SV7_PO { get; set; }
    public int B16_SO { get; set; }
    public int BT7_SO { get; set; }
    public int KC7_SO { get; set; }
    public int SR7_SO { get; set; }
    public int SP7_SO { get; set; }
    public int KP7_SO { get; set; }
    public int SV7_SO { get; set; }
    public int TOTAL { get; set; }
    public int TOTAL_SALE { get; set; }

    public class BranchesData
    {
        public string BranchName { get; set; }
        public int Purchased { get; set; }
        public int Sold { get; set; }
        public int Reserved { get; set; }
    }
}