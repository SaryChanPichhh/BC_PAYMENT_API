namespace BC.PAYMENT.CORE.DTO.Filter;

public record IssueInvoiceFilterDTO : BaseFilterDTO
{
    [Required] public string AreaId { get; set; }
}