namespace BC.PAYMENT.CORE.DTO.Filter
{
    public record BaseFilterDTO
    {
        [JsonIgnore]
        public string? DbCode { get; set; }

        [Required]
        public int Page{ get; set; } = 1;

        [Required] public int PageSize { get; set; } = 10;
    }
}
