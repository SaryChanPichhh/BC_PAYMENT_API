namespace BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.Repairer
{
    public interface ICompletedRepairRepository
    {
        Task<List<CompletedRepairItemDto>> GetAllCompletedRepairItemsAsync(string dbCode);
        Task<int> UpdatedAfterRepairerReceivedAsyc(ItemRepairReceivedModel model);
        Task<ItemBeingRepaired> GetItemByTransactionCode(string dbCode,string barCode);
        Task<int> InsertCompletedRepairItemAsync(ItemRepairReceivedModel model);
    }
}
