

using System.Runtime.InteropServices.JavaScript;
using BC.PAYMENT.CORE.Entities.CashFlow.CashFlowData;

namespace BC.PAYMENT.APPLICATION.Interfaces.CashFlow.CashFlowData
{
    public interface ICashFlowSubmittedRepository
    {
       Task<List<PaymentCashFlowSubmittedModel>> GetPaymentCashFlowModelSubmittedAsync(string dbCode);
       Task<List<PaymentCashFlowSubmittedModel>> GetPaymentCashFlowModelSubmittedByDateAsync(string dbCode,DateTime fromDate,DateTime toDate);
    }
}
