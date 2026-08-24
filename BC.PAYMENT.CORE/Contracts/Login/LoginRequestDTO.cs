namespace BC.PAYMENT.CORE.Contracts.Login
{
    public record LoginRequestDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string AppCode { get; set; } = "PYS";
        [Required]
        public string DbCode { get; set; }
        [Required]
        public string CompanyCode { get; set; }
    }
}
