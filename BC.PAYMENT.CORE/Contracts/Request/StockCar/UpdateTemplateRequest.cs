namespace BC.PAYMENT.CORE.Contracts.Request.StockCar;

public class UpdateTemplateRequest
{
    public int Id { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}