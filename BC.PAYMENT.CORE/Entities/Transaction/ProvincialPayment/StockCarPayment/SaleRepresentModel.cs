namespace BC.PAYMENT.CORE.Entities.Transaction.ProvincialPayment.StockCarPayment;

public class SaleRepresentModel
{
    public int Id { get; set; }
    public string? DbCode { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string? CreatedBy { get; set; }
    public string? Employee { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? EmployeeId { get; set; }
}