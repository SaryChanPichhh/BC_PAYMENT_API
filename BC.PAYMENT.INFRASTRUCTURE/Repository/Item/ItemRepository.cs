using BC.PAYMENT.CORE.Contracts.Response.Item;
using BC.PAYMENT.CORE.Entities;
using ItemDto = BC.PAYMENT.CORE.Contracts.Items.ItemDto;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Item
{
    public class ItemRepository(ISqlDataAccess sqlDataAccess) : IItemRepository
    {
        public async Task<List<ItemResponse>> GetItemListAsync(string dbCode)
        {
            var sql = $@"SELECT DB_CODE        AS DbCode
          ,ITEM_CODE       AS ItemCode
          ,ITEM_BCODE      AS ItemBarcode
          ,ITEM_DESC       AS ItemDesc
          ,ITEM_LOOKUP     AS ItemLookup
          ,ITEM_PRICE1     AS ItemPrice1
          ,ITEM_PRICE2     AS ItemPrice2
          ,ITEM_PRICE3     AS ItemPrice3
          ,ITEM_PRICE4     AS ItemPrice4
          ,ITEM_LEVEL      AS ItemLevel
          ,ITEM_TYPE       AS ItemType
          ,ITEM_DCOST      AS ItemCost
          ,UNIT_STOCK      AS UnitStock
          ,UNIT_SALE       AS UnitSale
          ,UNIT_WEIGHT     AS UnitWeight
          ,UPDT_PRICE      AS UpdatedPrice
          ,PROC_COMP       AS ProcComp
          ,ITEM_STAT       AS ItemStat
          ,ITEM_PRO        AS ItemPro
          ,ITEM_CUS1       AS ItemCus1
          ,ITEM_CUS2       AS ItemCus2
          ,ITEM_CUS3       AS ItemCus3
          ,ITEM_CUS4       AS ItemCus4
          ,ITEM_CUS5       AS ItemCus5
          ,ITEM_CUS6       AS ItemCus6
          ,ITEM_CUS7       AS ItemCus7
          ,ITEM_CUS8       AS ItemCus8
          ,ITEM_CUS9_KH    AS ItemCus9Kh
          ,ITEM_CUS10_KH   AS ItemDescKh
          ,TRANS_PRES      AS TransPres
          ,USER_CREA       AS UserCreated
          ,USER_UPDT       AS UserUpdated
          ,USER_CODE       AS UserCode
          ,IMG             AS Img FROM SIITEMS WHERE DB_CODE = @DB_CODE AND ITEM_STAT = 'A';";
            var param = new
            {
                DB_CODE = dbCode,
            };
            var execute = await sqlDataAccess.LoadData<ItemResponse, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
