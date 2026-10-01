namespace BC.PAYMENT.CORE.Contracts.Response.ConfirmBalance;

public class ConfirmBalanceResponse
{
    public int Id { get; set; }
    public string ConfirmBalanceOwner { get; set; } = string.Empty;
    public string Participants { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}