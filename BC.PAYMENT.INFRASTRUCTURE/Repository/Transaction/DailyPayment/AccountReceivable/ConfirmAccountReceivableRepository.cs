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
            throw new NotImplementedException();
        }

        public async Task<int> AddConfirmAccountReceivableDetails(int confirmAccountReceivableId, List<ConfirmAccountReceivableModel.ConfirmBalanceDetails> confirmBalanceDetails)
        {
            throw new NotImplementedException();
        }
    }
}
