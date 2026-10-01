namespace BC.PAYMENT.CORE.Entities.Report.DailyPaymentReport.SummaryInvoice;

public class SummaryInvoiceReportModel
{
    public DateTime CreateDate { get; set; }
    public string DeliveryName { get; set; }
    public string AreaNameKhmer { get; set; }
    public double CreditInvoice { get; set; }
    public double PaidInvoice { get; set; }
    public double Return { get; set; }
    public double Total { get; set; }
}