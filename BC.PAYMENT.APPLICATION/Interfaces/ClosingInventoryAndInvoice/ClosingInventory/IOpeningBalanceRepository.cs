namespace BC.PAYMENT.APPLICATION.Interfaces.ClosingInventoryAndInvoice.ClosingInventory;

public interface IOpeningBalanceRepository
{
    Task<List<OpeningBalanceModel>> LoadItemInStockByBranItemInStockByBranchCode(string code, string location);
    Task<int> OpenClosingEntryInventoryAsync(List<OpeningBalanceModel> ls);
}