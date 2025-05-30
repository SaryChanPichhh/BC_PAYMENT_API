

using BC.PAYMENT.CORE.DTO.CommondityExchange.RepairItem;
using BC.PAYMENT.CORE.Entities.CommondityExchange.RepairItem;

namespace BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.Repairer
{
    public interface ICompletedRepairRepository
    {
        Task<List<CompletedRepairItemDto>> GetAllCompletedRepairItemsAsync(string dbCode);
        Task<int> UpdatedAfterRepairerReceivedAsyc(ItemRepairReceivedModel model);
    }
}
