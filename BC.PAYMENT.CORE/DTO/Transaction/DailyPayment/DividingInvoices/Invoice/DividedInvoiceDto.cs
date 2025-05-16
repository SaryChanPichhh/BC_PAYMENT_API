using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class DividedInvoiceDto
    {
    }

    public class DividedInvoiceDeleteDto
    {
        public string InvoiceId { get; set; } = string.Empty;
        public string TransactionCode { get; set; } = string.Empty;
        public string DeliveryId { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }
}
