namespace BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.StockCarPayment;

public class SaleRepresentDto
{
    public string? EmployeeId { get; set; }
    public string? Description { get; set; }
}

public class SaleRepresentUpdateDto : SaleRepresentDto
{
    [Required(ErrorMessage = "Id is required")]
    [MinLength(5, ErrorMessage = "Id must be greater than 0")]
    public int TemplateId { get; set; }
}