
using BC.PAYMENT.CORE.DTO.CommondityExchange.RepairItem;
using BC.PAYMENT.CORE.DTO.General;

namespace BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.RepairItem
{
    public interface IRepairGoodsRepository
    {
        Task<List<RepairGoodsRespondDto>> GetReceivedRepairGoodsByBranchAsync(string dbCode);
        Task<int> DeleteReceivedRepairGoodAsync(int receivedId,int detailId);
        Task<string> GenerateTransactionCode(string dbCode);
        Task<int> TransferRepairGoodsAsync(RepairGoodsRespondDto repairGoodsDto);
        #region Repairing Goods

        Task<List<ItemRepairInprogressDto>> GetReparingGoodsByDateAsync(string dbCode,DateTime fromDate, DateTime toDate);
        Task<int> DeleteRepairItem(int receivedId, int repairId);

        #endregion

        #region RepairGoods Payment

        Task<List<CustomerDto>> GetAllCustomerHasCompletedRepair(string dbCode);
        Task<List<ItemRepairCompletedDto>> GetAllItemHasCompletedRepairByCustomerCode(string dbCode,string customerCode);
        Task<int> SwitchItemType(string dbCode, string userName, int id, string description, string fromType, string toType);
        Task<int> IssuanceRepairGoodCompletedInvoiceAsync(List<IssuanceInvoiceDto> model, NewRepairInvoiceCompletedDto newRepairInvoiceCompleted);
        Task<int> PaidRepairItemAsync(string createBy,double totalPrice,int repairCompletedId);

        #endregion
    }
}
