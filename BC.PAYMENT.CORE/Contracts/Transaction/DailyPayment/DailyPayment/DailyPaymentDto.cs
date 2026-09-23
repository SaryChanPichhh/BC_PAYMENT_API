using BC.PAYMENT.CORE.Contracts.Transaction.DailyPayment.DeliveryPaid;

namespace BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DailyPayment
{
    public class DailyPaymentDto
    {
    }

    public class DailyPaymentUpdateDto : DeliveryInvoicePaidUpdateDto
    {
        public int PaymentId { get; set; }
        public double OldPaidAmount { get; set; }
        public double NewPaidAmount { get; set; }
    }
}
