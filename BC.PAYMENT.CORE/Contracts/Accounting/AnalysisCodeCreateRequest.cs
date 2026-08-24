namespace BC.PAYMENT.CORE.DTO.Accounting
{
    public class AnalysisCodeCreateRequest
    {
        [Required]
        public string AnamCode { get; set; }

        [JsonIgnore]
        public string? DbCode { get; set; }
    }
}
