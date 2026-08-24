namespace BC.PAYMENT.CORE.DTO.General
{
    public record ContextDTO
    {
        public int UserId { get; set; }
        public string? DbCode { get; set; }
        public string? AppCode { get; set; }
    }
}
