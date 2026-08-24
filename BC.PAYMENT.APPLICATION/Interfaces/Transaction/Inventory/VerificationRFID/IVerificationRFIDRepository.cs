namespace BC.PAYMENT.APPLICATION.Interfaces.Transaction.Inventory.VerificationRFID
{
    public interface IVerificationRFIDRepository
    {
        Task<int> UpdateSubmittedStatus(string dbCode, string submitCode);
        Task<List<VerificationStockModel>> GetRFIDSubmittedItemDetail(string dbCode, string submitCode, string location);
        Task<List<string>> GetAllWarehouse(string dbCode);
        Task<List<VerificationStockModel>> GetAllSubmitCodeEntries(string dbCode, string location);
        Task<List<VerificationStockModel>> GetAllStockItemByLocationAndSubmitCode(string dbCode, string submitCode,
            string location);
    }
}
