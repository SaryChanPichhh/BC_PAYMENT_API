namespace BC.PAYMENT.CORE.Contracts.Request.ConfirmBalance;

public class UpdateConfirmBalanceDetailRequest
{
    public int ConfirmBalanceId { get; set; }
    public double Balance { get; set; }
    public string Description { get; set; } = string.Empty;
    public string InvoiceCode { get; set; } = string.Empty;
    public string IsCustomerAgreed { get; set; } = string.Empty;
    public string IsMet { get; set; } = string.Empty;
}