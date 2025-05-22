using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC.PAYMENT.CORE.Entities.Transaction.Submitting.SubmittingInvoice;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Submitting.SubmittingInvoice
{
    public interface ISubmittedInvoiceRepository
    {
        Task<List<SubmittedInvoiceModel>> GetAllNotSubmitPaidInvoice(string dbCode, string fromDate, string toDate);
        Task<int> UpdateInvoiceFromPendingToCancelAsync(string dbCode,string createBy, string submittedId);
    }
}
