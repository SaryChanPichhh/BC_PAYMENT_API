namespace BC.PAYMENT.CORE.Entities;

public class BcInvoiceSubmitted
{
    public int Id { get; set; }
    public string DbCode { get; set; } = string.Empty;
    public int InvoiceId { get; set; }
    public double Money { get; set; }

    public double InvoiceAmount
    {
        get => Money;
        set => Money = value;
    }

    public double Paid { get; set; }
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime SubmittedDate { get; set; }
    public bool Status { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string TransactionCode { get; set; } = string.Empty;
    public string SubmittedStatus { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
}