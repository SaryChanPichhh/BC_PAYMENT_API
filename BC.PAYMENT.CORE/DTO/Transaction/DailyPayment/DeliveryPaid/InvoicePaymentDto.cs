using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DeliveryPaid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC.PAYMENT.CORE.DTO.Expense;

namespace BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DeliveryPaid
{
    public  class InvoicePaymentDto
    {
    }

    public class DeliveryInvoicePaidPostDto
    {
        public PaymentInvoiceHeaderDto? PaymentInvoiceHeader { get; set; } 
        public List<ExpenseDto>  Expenses { get; set; }
    }
    public class PaymentInvoiceHeaderDto
    {
        public string? DeliveryId { get; set; }
        public string? EntriesCode { get; set; }
        public int Period { get; set; }
        public DateTime InvoiceDividendDate { get; set; }
    }

    public class DeliveryInvoicePaidUpdateDto
    {
        public string? Description { get; set; }
        public int DividedInvoiceId { get; set; } 
        public double OldAmount { get; set; }
        public double NewAmount { get; set; }
    }
    
}
