namespace BC.PAYMENT.CORE.Entities;

public class BcStockCar
{
    public int Id { get; set; }
    public string DbCode { get; set; } = string.Empty;
    public string CustomerCode { get; set; } =  string.Empty;
    public string CustomerName { get; set; } =  string.Empty;
    public string Code { get; set; } =  string.Empty;
    public double Value { get; set; }
    public string? Type { get; set; } =   string.Empty;
    public string Period { get; set; } =    string.Empty;
    public DateTime TransactionDate { get; set; }
    public bool Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public int TemplateId { get; set; } 
    
}