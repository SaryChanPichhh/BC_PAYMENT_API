namespace BC.PAYMENT.CORE.Entities.StockCar;

public class InvoicesPayment
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public double Amount { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// For Update Payment Invoice
    /// </summary>
    /// <param name="id"></param>
    /// <param name="invoiceId"></param>
    /// <param name="amount"></param>
    public InvoicesPayment(int id, int invoiceId, double amount)
    {
        Id = id;
        InvoiceId = invoiceId;
        Amount = amount;
    }

    /// <summary>
    /// For Create Payment Invoice
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="amount"></param>
    public InvoicesPayment(int invoiceId, double amount)
    {
        InvoiceId = invoiceId;
        Amount = amount;
    }

    public InvoicesPayment()
    {
    }
}
