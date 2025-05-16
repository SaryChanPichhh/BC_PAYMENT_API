using System.Data;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.AccountReceivable;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.AccountReceivable;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.AccountReceivable
{
    public class ConfirmAccountReceivableRepository : IConfirmAccountReceivableRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IDbConnection _dbConnection;
        public ConfirmAccountReceivableRepository(ISqlDataAccess sqlDataAccess, IDbConnection dbConnection)
        {
            _sqlDataAccess = sqlDataAccess;
            _dbConnection = dbConnection;
        }

        public async Task<int> AddNewAsync(ConfirmAccountReceivableModel.ConfirmBalance model)
        {
            var sql =
                @"INSERT INTO DT_CONFIRM_ACCOUNT_RECEIVABLE(DB_CODE,CONFIRM_BALANCE_OWNER,PARTICIPANTS,DESCRIPTION,STATUS,CREATED_DATE,CREATED_BY)
                VALUES (@DB_CODE,@CONFIRM_BALANCE_OWNER,@PARTICIPANTS,@DESCRIPTION,@STATUS,@CREATED_DATE,@CREATED_BY)";
            var parameters = new
            {
                DB_CODE = model.DbCode,
                CONFIRM_BALANCE_OWNER = model.ConfirmBalanceOwner,
                PARTICIPANTS = model.Participants,
                DESCRIPTION = model.Description,
                STATUS = "Inprogress",
                CREATED_DATE = model.CreatedDate,
                CREATED_BY = model.CreatedBy
            };
            return await _sqlDataAccess.ExecuteAsync(sql, parameters);
        }

        public async Task<int> UpdateAsync(ConfirmAccountReceivableModel.ConfirmBalance model)
        {
            const string sql =
                @"UPDATE DT_CONFIRM_ACCOUNT_RECEIVABLE SET CONFIRM_BALANCE_OWNER = @CONFIRM_BALANCE_OWNER,PARTICIPANTS = @PARTICIPANTS,DESCRIPTION = @DESCRIPTION WHERE ID = @ID";
            var param = new
            {
                CONFIRM_BALANCE_OWNER = model.ConfirmBalanceOwner,
                PARTICIPANTS = model.Participants,
                DESCRIPTION = model.Description,
                ID = model.Id
            };
            return await _sqlDataAccess.ExecuteAsync(sql, param);
        }

        public async Task<List<ConfirmAccountReceivableModel.ConfirmBalance>> GetAsync(string dbCode)
        {
           var sql =
                @"SELECT ID Id,CONFIRM_BALANCE_OWNER ConfirmBalanceOwner,PARTICIPANTS Participants,DESCRIPTION Description,CREATED_DATE CreatedDate,CREATED_BY CreatedBy
                FROM DT_CONFIRM_ACCOUNT_RECEIVABLE
                WHERE STATUS = 'Inprogress' AND DB_CODE = @DB_CODE";
            var parameter = new { DB_CODE = dbCode };
            var confirmAccountReceivable =
                await _sqlDataAccess.LoadData<ConfirmAccountReceivableModel.ConfirmBalance, dynamic>(sql, parameter);
            return confirmAccountReceivable.ToList();
        }

        public async Task<int> DeleteAsync(string code)
        {
            const string sql = @"DELETE FROM DT_CONFIRM_ACCOUNT_RECEIVABLE WHERE ID = @ID";
            var param = new { ID = code };
            return await _sqlDataAccess.ExecuteAsync(sql, param);
        }

        public async Task<List<ConfirmAccountReceivableModel.ConfirmBalanceDetails>> GetConfirmBalanceAccountReceivableDetails(string dbCode, int confirmAccountReceivableId)
        {
            const string sql =
                @"SELECT D.ID ConfirmBalanceDetailsId,D.CUSTOMER_CODE CustomerCode,CUS.CustomerName,CUS.Area,CUS.Market,CUS.Store,D.INVOICE_CODE InvoicedCode,INVOICE_AMOUNT InvoicedAmount,
                D.BALANCE Balance,D.DESCRIPTION Description,D.IS_AGREE IsCustomerAgreed,UPDATED_DATE UpdatedDate,UPDATED_BY UpdatedBy,STATUS Status,D.CUSTOMER_STATUS IsMet
                FROM DT_CONFIRM_ACCOUNT_RECEIVABLEDET D
                LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = D.CUSTOMER_CODE
                WHERE D.DB_CODE = @DB_CODE AND D.HEADER_ID = @ID";

            var param = new { DB_CODE = dbCode, ID = confirmAccountReceivableId };
            var results = await _sqlDataAccess.LoadData<ConfirmAccountReceivableModel.ConfirmBalanceDetails, dynamic>(sql, param);
            return results.ToList();
        }

        public async Task<int> UpdateConfirmBalanceAccountReceivableDetails(string dbCode, ConfirmAccountReceivableModel.ConfirmBalanceDetails balanceDetails)
        {
            const string sql = @"UPDATE DT_CONFIRM_ACCOUNT_RECEIVABLEDET SET 
            BALANCE = @BALANCE,
            DESCRIPTION = @DESCRIPTION,
            IS_AGREE = @IS_AGREE,
            CUSTOMER_STATUS = @IS_MET,
            STATUS = 'Completed',
            UPDATED_DATE = @UPDATED_DATE,
            UPDATED_BY = @UPDATED_BY
            WHERE INVOICE_CODE = @INVOICE_CODE
            AND HEADER_ID = @HEADER_ID";
            var param = new
            {
                BALANCE = balanceDetails.Balance,
                DESCRIPTION = balanceDetails.Description,
                IS_AGREE = balanceDetails.IsCustomerAgreed,
                IS_MET = balanceDetails.IsMet,
                UPDATED_DATE = DateTime.Now,
                UPDATED_BY = balanceDetails.UpdatedBy,
                INVOICE_CODE = balanceDetails.InvoicedCode,
                HEADER_ID = balanceDetails.ConfirmBalanceId
            };
            var rowAffected = await _sqlDataAccess.ExecuteAsync(sql, param);
            return rowAffected ;
        }

        public async Task<int> AddConfirmAccountReceivableDetails(int confirmAccountReceivableId, List<ConfirmAccountReceivableModel.ConfirmBalanceDetails> confirmBalanceDetails)
        {
            if(_dbConnection.State == ConnectionState.Closed)
            { 
                _dbConnection.Open();
            }
            var transaction= _dbConnection.BeginTransaction();
            try
            {
                const string sql = @"INSERT INTO DT_CONFIRM_ACCOUNT_RECEIVABLEDET
               (HEADER_ID,DB_CODE,INVOICE_CODE,CUSTOMER_NAME,CUSTOMER_CODE,INVOICE_AMOUNT ,CREATED_DATE,CREATED_BY,STATUS)        
                VALUES (@HEADER_ID,@DB_CODE,@INVOICE_CODE,@CUSTOMER_NAME,@CUSTOMER_CODE,@INVOICE_AMOUNT,@CREATED_DATE,@CREATED_BY,@STATUS)";
                    const string sqlCheckExistInvoice = @"SELECT CAST(COUNT(*) AS BIT) FROM DT_CONFIRM_ACCOUNT_RECEIVABLEDET
                WHERE DB_CODE = @DB_CODE AND HEADER_ID = @HEADER_ID AND INVOICE_CODE = @INVOICE_CODE";
                var rowAffected = 0;
                foreach (var item in confirmBalanceDetails)
                {
                    var paramCheckExistInvoice = new
                    {
                        DB_CODE = item.DbCode,
                        HEADER_ID = confirmAccountReceivableId,
                        INVOICE_CODE = item.InvoicedCode
                    };
                    var exist = await _dbConnection.ExecuteScalarAsync<bool>(sqlCheckExistInvoice, paramCheckExistInvoice, transaction);
                    if (exist) continue;
                    var param = new
                    {
                        HEADER_ID = confirmAccountReceivableId,
                        DB_CODE = item.DbCode,
                        INVOICE_CODE = item.InvoicedCode,
                        CUSTOMER_NAME = item.CustomerName,
                        CUSTOMER_CODE = item.CustomerCode,
                        INVOICE_AMOUNT = item.InvoicedAmount,
                        CREATED_DATE = DateTime.Now,
                        CREATED_BY = item.CreatedBy,
                        STATUS = "Pending"
                    };
                    rowAffected += await _dbConnection.ExecuteAsync(sql, param, transaction);
                }
                if (rowAffected == confirmBalanceDetails.Count)
                {
                    transaction.Commit();
                    return rowAffected;
                }
                else
                {
                    transaction.Commit();
                    return rowAffected;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                _dbConnection.Close();
            }
        }
    }
}
