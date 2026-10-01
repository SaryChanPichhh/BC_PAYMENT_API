namespace BC.PAYMENT.CORE.Entities;

public class PcReturnInvoice
{
    public int ReturnId { get; set; }
    public string DbCode { get; set; } = string.Empty;
    public int DividedId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool Status { get; set; }
    public int HeaderId { get; set; }
}