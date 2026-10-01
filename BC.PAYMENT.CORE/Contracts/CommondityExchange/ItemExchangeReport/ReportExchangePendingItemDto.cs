namespace BC.PAYMENT.CORE.DTO.CommondityExchange.ItemExchangeReport;

public class ReportExchangePendingItemDto : Customer
{
    public string Description { get; set; }
    public string ItemCode { get; set; }
    public int Quantity { get; set; }
    public string Seller { get; set; }
    public string Status { get; set; }
    public string StatusText => Status == "Yes".Trim() ? "បានទទួល" : "ឥណទាន";
    public DateTime CreatedDate { get; set; }
}