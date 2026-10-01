namespace BC.PAYMENT.CORE.DTO.Inventory;

public class InventoryReportRequestDto
{
    public List<string> TranTypes { get; set; }
    public string Location { get; set; }
    public string DbCode { get; set; }

    public class RequestByDateDto : InventoryReportRequestDto
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class RequestByPeriodDto : InventoryReportRequestDto
    {
        public int FromPeriod { get; set; }
        public int ToPeriod { get; set; }
    }
}