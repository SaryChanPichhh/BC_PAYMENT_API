namespace BC.PAYMENT.CORE.DTO.CommondityExchange.ReportDividedInvoice
{
    public class ReportDividedInvoiceDto : Customer
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Delivery { get; set; }
        public string Transaction { get; set; }
        public double Total { get; set; }
        public string ShippedStatus { get; set; }
        public string Seller { get; set; }
        public string InvoiceNumber { get; set; }
        public string Status
        {
            get
            {
                return ShippedStatus.Trim() switch
                {
                    "Pending" => "កំពុងដំណើរការ",
                    "Completed" => "រួចរាល់",
                    "Return" => "ត្រឡប់",
                    _ => ""
                };
            }
        }
    }
}
