using BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DeliveryPaid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
