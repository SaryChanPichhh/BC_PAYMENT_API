

using BC.PAYMENT.CORE.Enums;

namespace BC.PAYMENT.CORE.DTO.Transaction.Submitting.SubmittingInvoice
{
    public class SubmittingInvoiceDto
    {
        public List<SubmittedInvoicePostDto> SubmittedInvoices { get; set; } = new ();
        public List<AccountReceivableDto> AccountReceivables { get; set; } = new ();
        public bool IsAccountsReceivableCompleted { get; set; } = false;
    }

    public class AccountReceivableDto
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
    }

}
