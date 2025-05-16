using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public interface IDividedInvoiceRepository
    {
        Task<List<DividedInvoiceModel>> GetDividedInvoicesByDeliveryIdAndDateAsync(string dbCode, string deliveryId, DateTime date);
        Task<int> DeleteDividedInvoiceAsync(string invoiceId, string transactionCode, string deliveryId, string note);
    }
}
