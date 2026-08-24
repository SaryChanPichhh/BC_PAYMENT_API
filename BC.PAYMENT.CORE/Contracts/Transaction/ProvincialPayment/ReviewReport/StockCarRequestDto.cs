namespace BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.ReviewReport
{
    public class StockCarRequestDto
    {
        public int RequestId { get; set; }
        public string? Employee { get; set; }
        public string? Title { get; set; }
        public int Action { get; set; }
        public DateTime Date { get; set; }
    }

    public class StockCarRequestParamDto
    {
        public int ActionId { get; set; }
        public List<int> RoleId { get; set; }
    }
}
