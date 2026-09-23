namespace BC.PAYMENT.CORE.Entities.Invoice
{
    public class Invoice : Customer
    {
        public int InvoiceId { get; set; }
        public string? TransRef { get; set; }
        public string? Employee { get; set; }
        public double InvoiceValue { get; set; }
        public bool Status { get; set; }
        
    }
}
