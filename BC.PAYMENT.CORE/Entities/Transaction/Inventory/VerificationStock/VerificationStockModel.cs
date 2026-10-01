namespace BC.PAYMENT.CORE.Entities.Transaction.Inventory.VerificationStock;

public class VerificationStockModel
{
    public string DbCode { get; set; }
    public string SubmitCode { get; set; }
    public string StockChecker { get; set; }
    public string Stocker { get; set; }
    public string Admin { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ExpiredDate { get; set; }
    public string ItemCode { get; set; }
    public string ItemDescription { get; set; }
    public string Location { get; set; }
    public int? UnitStock { get; set; }
    public int? Physical { get; set; }
    public int? OnOrder { get; set; }
    public int? SubTotal { get; set; }
    public int Quantity { get; set; }
    public int Total { get; set; }
    public string CreateBy { get; set; }
    public string Status => Total == 0 ? "ត្រឹមត្រូវ" : Total > 0 ? "ខ្វះ" : Total < 0 ? "លើស" : "";

    public int AdjustQty => Status switch
    {
        "ត្រឹមត្រូវ" => 0,
        "ខ្វះ" => Total * 1,
        "លើស" => Total * -1,
        _ => 0
    };

    public string StatusType => Status switch
    {
        "ត្រឹមត្រូវ" => "",
        "ខ្វះ" => "ADJ-",
        "លើស" => "ADJ+",
        _ => ""
    };
}