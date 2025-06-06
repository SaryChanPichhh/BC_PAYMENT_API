namespace BC.PAYMENT.CORE.Entities.Preset.AnnualPurchase
{
    public class DailySaleReportValueDetailsModel : SaleReportModel
    {
        public string ItemCode { get; set; }
    }
    public class DailySaleReportValueModel : SaleReportModel
    {
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
    }
    public class SaleReportModel
    {
        public double Invoiced { get; set; }
        public double Ordered { get; set; }
        public double Posted { get; set; }
        public double Total { get; set; }
    }
}
