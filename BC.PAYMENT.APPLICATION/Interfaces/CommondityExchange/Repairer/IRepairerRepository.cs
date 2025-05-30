

using BC.PAYMENT.CORE.Entities.CommondityExchange.RepairItem;

namespace BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.Repairer
{
    public interface IRepairerRepository
    {
        Task<List<ItemRepairReceivedModel>> LoadItemRepairedToReparation(string dbCode);
        Task<int> UpdateAfterRepairerReceivedItemsAsync(string dbCode,string userName,string transactionCode);
    }
}
