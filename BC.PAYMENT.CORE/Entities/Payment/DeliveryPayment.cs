namespace BC.PAYMENT.CORE.Entities.Payment
{
    public class DeliveryPayment:Customer
    { 
        public int DividedInvoiceId { get; set; }
        public DateTime DividedDate { get; set; }
        public string? TransRef { get; set; }
        public string? Delivery { get; set; }
        public int Old { get; set; }
        public double TransValue { get; set; }
        public int IsReturn { get; set; }
        public int IsPaid { get; set; }
        public string? Description { get; set; }
        public double PaidAmount { get; set; }
        public string? Status { get; set; }
    }
}
