namespace BC.PAYMENT.CORE.DTO.ClosingInventoryAndInvoice
{
    public class OpeningBalanceDto
    {
        public string ItemCode { get; set; }
        public string Location { get; set; }
        public string ItemDesc { get; set; }
        public int Physical { get; set; }
        public string DbCode { get; set; }
    }
}
