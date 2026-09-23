namespace BC.PAYMENT.CORE.Contracts.Response.StockCar;

public class TemplateResponse
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } =  string.Empty;
    public string Employee { get; set; } =  string.Empty;
    public string EmployeeId { get; set; } =  string.Empty;
    public string Description { get; set; } =   string.Empty;
    public DateTime? ClosingDate { get; set; }
}