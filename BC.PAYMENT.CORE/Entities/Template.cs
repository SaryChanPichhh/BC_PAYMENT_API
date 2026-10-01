namespace BC.PAYMENT.CORE.Entities;

public class Template
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string Employee { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string DbCode { get; set; } = string.Empty;
    public bool IsEnable { get; set; }
    public DateTime ClosingDate { get; set; }
    public string ClosingBy { get; set; } = string.Empty;
}