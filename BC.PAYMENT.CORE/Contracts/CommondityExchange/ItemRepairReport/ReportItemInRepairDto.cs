namespace BC.PAYMENT.CORE.DTO.CommondityExchange.ItemRepairReport
{
    public class ReportItemInRepairDto : ReportItemRepairDto
    {
        public new string Status { get; set; }
        public string StatusText => Status.Trim() == "Yes" ? "កំពុងជួសជុល" : "បានផ្ញើរទៅជាង";
    }
}
