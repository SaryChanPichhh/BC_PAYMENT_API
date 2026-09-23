namespace BC.PAYMENT.CORE.Contracts.Request.Invoice;

public class UpdateHeaderValueRequest
{
    public int DividedId { get; set; }
    public double OldAmount { get; set; }
    public double NewAmount { get; set; }
    public string Description { get; set; } = string.Empty;
}