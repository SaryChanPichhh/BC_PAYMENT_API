namespace BC.PAYMENT.CORE.Contracts.Response.Invoice;

public class DividedInvoiceStatusResponse :  Entities.General.Customer
{
    public int DividedId { get; set; }
    public int Id { get => DividedId; set => DividedId = value; }
    public DateTime Date { get; set; }
    public string Delivery { get; set; } = string.Empty;
    public string TransactionCode { get; set; } =string.Empty;
    public double InvoiceValue { get; set; }
    public bool IsReturn {
        get;
        set;
    }
    public bool IsPaid {
        get;
        set;
    }
    public string Description { get; set; } = string.Empty;
    public double? PaidAmount {
        get;
        set;
    }
    public double? Total => InvoiceValue - PaidAmount;
    public string Status { get; set; } = string.Empty;

}