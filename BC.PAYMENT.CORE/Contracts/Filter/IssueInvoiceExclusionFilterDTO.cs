namespace BC.PAYMENT.CORE.DTO.Filter;

public record IssueInvoiceExclusionFilterDTO : IssueInvoiceFilterDTO
{
    [Required] public string TransRef { get; set; }
}