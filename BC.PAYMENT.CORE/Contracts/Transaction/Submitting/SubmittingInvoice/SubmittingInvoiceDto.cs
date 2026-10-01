namespace BC.PAYMENT.CORE.Contracts.Transaction.Submitting.SubmittingInvoice;

public class SubmittingInvoiceDto
{
    public List<SubmittedInvoicePostDto> SubmittedInvoices { get; set; } = new();
    public List<AccountReceivableParameterDto> AccountReceivables { get; set; } = new();
    public bool IsAutoAccountsReceivable { get; set; } = false;
}

public class AccountReceivableParameterDto
{
    public string? InvoiceType { get; set; }
    public string? TransactionCode { get; set; }
    public int Period { get; set; }
    public int InvoiceId { get; set; }
    public string? CustomerCode { get; set; }
    public string? InvoiceAmount { get; set; }
    public bool Status { get; set; }
    public double HalfPaid { get; set; }
    public double Paid { get; set; }
}

public class SubmittedInvoicePostDto
{
    public int InvoiceId { get; set; }
    public string? CustomerCode { get; set; }
    public string? TransactionCode { get; set; }
    public double InvoiceAmount { get; set; }
    public double HalfPaid { get; set; }
    public double Paid { get; set; }
    public double PaidAmount => HalfPaid + Paid;
}