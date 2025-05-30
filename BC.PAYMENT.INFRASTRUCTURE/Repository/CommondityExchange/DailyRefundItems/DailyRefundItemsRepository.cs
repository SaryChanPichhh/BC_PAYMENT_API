using BC.PAYMENT.APPLICATION.Interfaces.CommondityExchange.DailyRefundItems;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC.PAYMENT.CORE.Entities.CommondityExchange.DailyRefundItems;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Dapper;
using System.Runtime.CompilerServices;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CommondityExchange.DailyRefundItems
{
    public class DailyRefundItemsRepository : IDailyRefundItemRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IDbConnection _dbConnection;
        public DailyRefundItemsRepository(ISqlDataAccess sqlDataAccess, IDbConnection dbConnection)
        {
            _sqlDataAccess = sqlDataAccess;
            _dbConnection = dbConnection;
        }

        public async Task<List<ItemModel>> GetItemsRefundByAllBranchAsync()
        {
            const string sql = @"
                SELECT TBC.ID MasterId,TBCD.ID Id,TBC.CREATED_DATE RequestDate,ADD_CODE CustomerCode,ADD_LINE_1 CustomerName,
           MARKET_KHMER_NAME Market,AREA_NAME_KHMER Area,ITEM.ITEM_CODE ItemCode,ITEM.ITEM_NAME ItemName,
		   TBCD.DESCRIPION ChangeType,QUANTITY Quantity,dbo.RETURN_RECEIVED_STATUS_EXCHANGE(TBCD.IS_RECEIVED) Status,CASE WHEN TBCD.TYPE = 'REPAIR' 
		   THEN N'ជួសជុល' ELSE N'ផ្លាស់ប្តូរ' END Type,STORE Store,INVOICE_NUMBER InvoiceNumber
				FROM TB_BC_CHANGEINVOICE TBC 
					INNER JOIN TB_BC_CHANGEINVOICE_DETAIL TBCD 
						ON TBCD.CHANGE_INVOICE_ID = TBC.ID
					INNER JOIN (
						SELECT A.AREA_ID,M.MARKET_ID,ADD_CODE,ADD_LINE_1,M.MARKET_KHMER_NAME,A.AREA_NAME_KHMER,STORE
							FROM SIADD CUS INNER JOIN TB_BCMARKET M ON M.MARKET_ID = CUS.MARKET_ID
					INNER JOIN TB_AREAS A
						ON A.AREA_ID = CUS.AREA_ID ) CUSTOMER 
						ON CUSTOMER.ADD_CODE = CUST_CODE
					INNER JOIN (SELECT DIStinct ITEM_CODE,CASE WHEN ITEM_CUS10_KH = '' 
					THEN ITEM_DESC ELSE ITEM_CUS10_KH END 'ITEM_NAME' FROM SIITEMS
                    ) ITEM
                ON ITEM.ITEM_CODE = TBCD.ITEM_CODE
				WHERE TBCD.IS_RECEIVED = 'Pending'";
            var execute = await _sqlDataAccess.LoadData<ItemModel, dynamic>(sql, new { });
            return execute.ToList();
        }

        public async Task<List<ItemModel>> GetItemsRefundByByBranchAsync(string dbCode)
        {
            const string sql = @"
                SELECT S.LAST_NAME +'' + S.FIRST_NAME Seller,TBC.ID MasterId,TBCD.ID Id,TBC.CREATED_DATE RequestDate,ADD_CODE CustomerCode,ADD_LINE_1 CustomerName,
                MARKET_KHMER_NAME Market,AREA_NAME_KHMER Area,ITEM.ITEM_CODE ItemCode,ITEM.ITEM_NAME ItemName,
                TBCD.DESCRIPION ChangeType,QUANTITY Quantity,dbo.RETURN_RECEIVED_STATUS_EXCHANGE(TBCD.IS_RECEIVED) Status,CASE WHEN TBCD.TYPE = 'REPAIR' 
                THEN N'ជួសជុល' ELSE N'ផ្លាស់ប្តូរ' END Type,STORE Store
		                FROM TB_BC_CHANGEINVOICE TBC 
			                INNER JOIN TB_BC_CHANGEINVOICE_DETAIL TBCD 
				                ON TBCD.CHANGE_INVOICE_ID = TBC.ID
			                INNER JOIN (
				                SELECT A.AREA_ID,M.MARKET_ID,ADD_CODE,ADD_LINE_1,M.MARKET_KHMER_NAME,A.AREA_NAME_KHMER,STORE
					                FROM SIADD CUS INNER JOIN TB_BCMARKET M ON M.MARKET_ID = CUS.MARKET_ID
			                INNER JOIN TB_AREAS A
				                ON A.AREA_ID = CUS.AREA_ID
				                WHERE A.DB_CODE = @DB_CODE 
					                AND M.DB_CODE = @DB_CODE 
					                AND CUS.DB_CODE = @DB_CODE) CUSTOMER 
				                ON CUSTOMER.ADD_CODE = CUST_CODE
			                INNER JOIN (SELECT DIStinct ITEM_CODE,CASE WHEN ITEM_CUS10_KH = '' 
			                THEN ITEM_DESC ELSE ITEM_CUS10_KH END 'ITEM_NAME' FROM SIITEMS
			                WHERE DB_CODE = @DB_CODE) ITEM
                     ON ITEM.ITEM_CODE = TBCD.ITEM_CODE
		                INNER JOIN BCUSERS S ON S.USER_ID = TBC.USER_CODE
		                WHERE TBCD.IS_RECEIVED = 'Pending'";
            var param = new
            {
                DB_CODE = dbCode,
            };
            var execute = await _sqlDataAccess.LoadData<ItemModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> DeleteRequestItem(int requestMaster, int requestDetail)
        {
            const string sql = @"DELETE FROM TB_BC_CHANGEINVOICE_DETAIL WHERE ID = @DetailId
            IF NOT EXISTS (SELECT * FROM TB_BC_CHANGEINVOICE_DETAIL WHERE CHANGE_INVOICE_ID = @MasterId)
	            DELETE FROM TB_BC_CHANGEINVOICE WHERE ID = @MasterId";
            var param = new
            {
                DetailId = requestDetail,
                MasterId = requestMaster
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<int> InsertReceivedItem(string userName, int detailId)
        {
            if (_dbConnection.State == ConnectionState.Closed)
                _dbConnection.Open();
            var transaction = _dbConnection.BeginTransaction();
            try
            {
                const string insertChangeInvoiceReceived =
                    @"INSERT INTO TB_BC_CHANGEINVOICE_RECEIVED SELECT ID,ITEM_CODE,QUANTITY,DESCRIPION,0,GETDATE(),@CREATED_BY FROM TB_BC_CHANGEINVOICE_DETAIL WHERE ID = @ID";
                var changeInvoiceReceivedParam = new
                {
                    ID = detailId,
                    CREATED_BY = userName,
                };
                var affectedRow = await _dbConnection.ExecuteAsync(insertChangeInvoiceReceived, changeInvoiceReceivedParam, transaction);
                if (affectedRow > 0)
                {
                    const string updateChangeInvoiceDetail = @"UPDATE TB_BC_CHANGEINVOICE_DETAIL SET IS_RECEIVED = 'Yes' WHERE ID = @ID";
                    var changeInvoiceDetailParam = new
                    {
                        ID = detailId
                    };
                    var updateAffected = await _dbConnection.ExecuteAsync(updateChangeInvoiceDetail, changeInvoiceDetailParam, transaction);
                    if (updateAffected > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    { 
                        transaction.Rollback();
                        return 0;
                    }
                    return affectedRow;
                }
                else
                {
                    transaction.Rollback();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine(ex.Message);
            }
            return 0;
        }

        public async Task<byte[]> GetImageByDetailIdAsync(int detailId)
        {
            var sql = $@"SELECT IMAGE FROM TB_BC_CHANGEINVOICE_DETAIL WHERE ID = @DETAIL_ID";
            var param = new
            {
                DETAIL_ID = detailId
            };
            var execute = await _sqlDataAccess.LoadData<byte[], dynamic>(sql, param);

            return execute.FirstOrDefault() ?? Array.Empty<byte>();
        }
    }
}
