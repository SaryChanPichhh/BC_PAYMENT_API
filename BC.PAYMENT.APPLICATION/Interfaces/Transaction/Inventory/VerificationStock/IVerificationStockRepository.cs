using System.ComponentModel;
using BC.PAYMENT.CORE.DTO.Transaction.Inventory.VerificationStock;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Entities.Transaction.Inventory.VerificationStock;

namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Inventory.VerificationStock
{
    public interface IVerificationStockRepository
    {
        Task<List<VerificationStockModel>> GetAllStockItemByLocationAndCountingDateAsync(string dbCode,string location, DateTime fromDate,
            DateTime toDate,int page,int pageSize);
        Task<List<VerificationStockModel>> GetAllStockItemByLocationAndCountingPeriodAsync(string dbCode,string location, int month,
            int year,int page,int pageSize);

        Task<int> SaveRecordStockItemAfterVerify(List<VerificationStockModel> model);
        Task<int> GetMaxSequence(string dbCode);
        Task<BcModels> GetRecTypes(string dbCode,string movType);
        Task<double> GetItemCostAsync(string dbCode,string itemCode);
        Task<int> AdjustInventory(InventoryAdjustmentModel inventory);

        #region Verification Stock Report

        Task<List<VerificationStockReportDto>> GetVerificationStockReport(string dbCode, DateTime fromDate,
            DateTime toDate);

        Task<List<VerificationStockReportDto>> GetVerificationStockReportByPeriod(string dbCode, int month,int year);

        #endregion
    }
}
