namespace BC.PAYMENT.CORE.Entities;

public class DtConfirmAccountReceivable
{
    public int Id { get; set; }
    public string DbCode { get; set; } = string.Empty;
    public string ConfirmBalanceOwner { get; set; } = string.Empty;
    public string Participants { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}