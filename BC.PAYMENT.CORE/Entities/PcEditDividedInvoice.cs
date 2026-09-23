namespace BC.PAYMENT.CORE.Entities;

public class PcEditDividedInvoice
{
    public string DbCode { get; set; } = string.Empty;
    public int DividedId { get; set; }
    public double OldAmount { get; set; }
    public double NewAmount { get; set; }
    public string Description { get; set; } =  string.Empty;
    public DateTime CreateDate { get; set; }
    public string CreatedBy { get; set; } =  string.Empty;
}
