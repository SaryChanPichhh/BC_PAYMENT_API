namespace BC.PAYMENT.CORE.DTO.CommondityExchange.ItemRepairReport;

public class ReportItemRepairDto
{
    public string Id { get; set; }
    public string DetailId { get; set; }
    public DateTime Date { get; set; }
    public DateTime SubmittedDate { get; set; }
    public string BranchName { get; set; }
    public string BranchCode { get; set; }
    public string Sale { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string ChangeType { get; set; }
    public string Seller { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public string StatusRepair { get; set; }
    public string ItemTransaction { get; set; }
    public string ItemStatus { get; set; }
    public string ItemStatusText => ItemStatus.Trim() == "New" ? "ថ្មី" : "ចាស់";
    public string Status => StatusRepair.Trim() == "Repairable" ? "បានជួសជុសហើយ" : "ទំនិញមិនអាចជួសជុលបាន";
    public string Note { get; set; }
}