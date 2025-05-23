
using BC.PAYMENT.APPLICATION.Interfaces.Items;
using BC.PAYMENT.CORE.DTO.Items;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Items
{
    public class ItemRepository : IItemRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ItemRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        public async Task<List<ItemDto>> GetItemListAsync(string dbCode)
        {
            var sql = $@"SELECT ITEM_CODE ItemCode,ITEM_NAME ItemName FROM SIITEMS WHERE DB_CODE = @DB_CODE";
            var param = new
            {
                DB_CODE = dbCode,
            };
            var execute = await _sqlDataAccess.LoadData<ItemDto, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
