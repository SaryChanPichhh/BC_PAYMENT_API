namespace BC.PAYMENT.CORE.Contracts.Login
{
    public record LoginResponseDTO
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string DbCode { get; set; }
    }
}
