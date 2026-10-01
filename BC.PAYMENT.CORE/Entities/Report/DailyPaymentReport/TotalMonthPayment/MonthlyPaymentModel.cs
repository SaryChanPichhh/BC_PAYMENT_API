namespace BC.PAYMENT.CORE.Entities.Report.DailyPaymentReport.TotalMonthPayment;

public class MonthlyPaymentModel
{
    public int RowNum { get; set; }
    public DateTime CreateDate { get; set; }
    public string TransactionRef { get; set; }
    public double TransactionValue { get; set; }
    public int AmountInvoice { get; set; }
}

public class MonthlyHistoryPaidInvoiceModel
{
    public string InvoiceCode { get; set; }
    public decimal InvoiceValue { get; set; }
    public List<InvoicePayment> Payments { get; set; } = new();
    public decimal TotalPaid { get; set; }
    public decimal Balance { get; set; }
}

public class InvoicePayment
{
    public int PaymentNumber { get; set; }
    public decimal Amount { get; set; }
}