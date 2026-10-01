namespace BC.PAYMENT.CORE.Contracts.Request.ConfirmBalance;

public class CreateConfirmBalanceRequest
{
    public string ConfirmBalanceOwner { get; set; } = string.Empty;
    public string Participants { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}