namespace BC.PAYMENT.CORE.Contracts.Request.ConfirmBalance;

public class CreateConfirmBalanceDetailRequest
{
    public int HeaderId { get; set; }
    public string CustomerCode { get; set; } =  string.Empty;
    public string CustomerName { get; set; } =  string.Empty;
    public string InvoiceCode { get; set; } = string.Empty;
    public double InvoiceAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}