namespace BC.PAYMENT.CORE.Entities.Report.DailyPaymentReport.ExpenseInvoice
{
    public class ExpenseInvoiceReportModel
    {
        public DateTime CreateDate { get; set; }
        public string DeliveryName { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string ExpenseDescription { get; set; }
    }
}
