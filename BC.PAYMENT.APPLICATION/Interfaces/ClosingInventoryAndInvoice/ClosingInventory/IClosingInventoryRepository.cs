namespace BC.PAYMENT.APPLICATION.Interfaces.ClosingInventoryAndInvoice.ClosingInventory;

public interface IClosingInventoryRepository
{
    #region Daily Closing Entry

    Task<List<ClosingStockEntryModel>> GetClosingEntryDay(string dbCode, string date);
    Task<bool> CheckStockQuantityAsync(string dbCode, string location, string itemCode);
    Task<int> InsertNewItem(NewItemModel model);
    Task<bool> ExistItem(NewItemModel model);
    Task<bool> CheckDateIsAlreadyClosingEntry(string dbCode, DateTime date);
    Task<int> InsertDailyClosingEntryAsync(List<ClosingStockEntryModel> closingEntryModel, bool status);

    #endregion

    #region Monthly Closing Entry

    Task<List<ClosingStockEntryModel>> GetAllStockBalanceByBranchCode(string dbCode, string closingDate);

    Task<int> InsertClosingMonthlyAndYearlyAsync(List<ClosingStockEntryModel> closingEntryModel,
        ClosingEntryType closeType);

    #endregion

    #region Yearly Closing Entry

    Task<List<ClosingStockEntryModel>> GetAllStockBalanceMonthlyByBranchCode(string dbCode, string closingDate);

    #endregion
}