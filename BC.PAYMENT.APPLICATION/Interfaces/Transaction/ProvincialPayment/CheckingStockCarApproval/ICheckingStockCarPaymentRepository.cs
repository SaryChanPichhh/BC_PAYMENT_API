using BC.PAYMENT.CORE.Entities.Transaction.ProvincialPayment.CheckingStockCarPayment;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.ProvincialPayment.CheckingStockCarApproval
{
    public interface ICheckingStockCarPaymentRepository
    {
        #region Submitting
        Task<List<CheckingStockSubmittingInvoiceModel>> GetAllSubmittingInvoiceByPeriodAsync(string dbCode,int month,int year);
        Task<List<CheckingStockSubmittingInvoiceModel>> GetAllSubmittingInvoiceByDateAsync(string dbCode,DateTime fromDate,DateTime toDate,bool status);

        #endregion
    }
}
