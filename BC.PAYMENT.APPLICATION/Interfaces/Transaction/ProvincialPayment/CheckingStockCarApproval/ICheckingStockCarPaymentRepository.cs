using BC.PAYMENT.CORE.Entities.Transaction.ProvincialPayment.CheckingStockCarPayment;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.ProvincialPayment.CheckingStockCarApproval
{
    public interface ICheckingStockCarPaymentRepository
    {
        #region Submitted
        Task<List<CheckingStockSubmittingInvoiceModel>> GetAllStockCarSubmittingInvoiceByPeriodAsync(string dbCode,int month,int year);
        Task<List<CheckingStockSubmittingInvoiceModel>> GetAllStockCarSubmittingInvoiceByDateAsync(string dbCode,DateTime fromDate,DateTime toDate,bool status);


        #region Total Amount of Collection

        


        #endregion
        #endregion
    }
}
