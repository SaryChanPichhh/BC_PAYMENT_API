using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DailyPayment;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.SubmittingInvoice
{
    public interface ISubmittedRejectedInvoicePerDeliveryRepository
    {
        Task<List<RejectedInvoicePerDelivery>> GetAllRejectedInvoicePerDeliveryByDateAsync(string dbCode, string fromDate, string toDate);
        Task<List<RejectedInvoicePerDelivery>> GetAllRejectedInvoicePerDeliveryByPeriodAsync(string dbCode, int month, int year);
    }
}
