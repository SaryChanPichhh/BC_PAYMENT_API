namespace BC.PAYMENT.CORE.DTO.Preset.OwedInvoiceDto
{
    public class OwedInvoiceDto
    {
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public double Amount { get; set; }
    }

    public record OwedInvoiceFilterDto  
    {
        public List<BranchDTO> BranchDtos { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
