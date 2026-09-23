using BC.PAYMENT.APPLICATION.Interfaces.ConfirmBalance;
using BC.PAYMENT.CORE.Contracts.Request.ConfirmBalance;
using BC.PAYMENT.CORE.Contracts.Response.ConfirmBalance;
using BC.PAYMENT.CORE.Entities;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.SQL.Queries;
using ConfirmBalanceAccountReceivable = BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.AccountReceivable.ConfirmAccountReceivableModel;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.ConfirmBalance
{
    public class ConfirmBalanceRepository(ISqlDataAccess sqlDataAccess,IDbConnection dbConnection) : IConfirmBalanceRepository
    {
        public async Task<int> AddConfirmAccountReceivable(DtConfirmAccountReceivable request)
        {
            var parameters = new
            {
                DB_CODE = request.DbCode,
                CONFIRM_BALANCE_OWNER = request.ConfirmBalanceOwner,
                PARTICIPANTS = request.Participants,
                DESCRIPTION = request.Description,
                STATUS = "Inprogress",
                CREATED_DATE = request.CreatedDate,
                CREATED_BY = request.CreatedBy
            };
            return await sqlDataAccess.ExecuteAsync(ConfirmBalanceQueries.AddConfirmAccountReceivable, parameters);
        }

        public async Task<int> DeleteConfirmAccountReceivable(int id)
        {
            var param = new { ID = id };
            return await sqlDataAccess.ExecuteAsync(ConfirmBalanceQueries.DeleteConfirmAccountReceivable, param);
        }

        public async Task<int> UpdateConfirmAccountReceivable(DtConfirmAccountReceivable request)
        {
            var param = new
            {
                CONFIRM_BALANCE_OWNER = request.ConfirmBalanceOwner,
                PARTICIPANTS = request.Participants,
                DESCRIPTION = request.Description,
                ID = request.Id
            };
            return await sqlDataAccess.ExecuteAsync(ConfirmBalanceQueries.UpdateConfirmAccountReceivable, param);
        }

        public async Task<List<ConfirmBalanceResponse>> GetConfirmAccountReceivable(string dbCode)
        {
            var parameter = new { DB_CODE = dbCode };
            var confirmAccountReceivable =
                await sqlDataAccess.LoadData<ConfirmBalanceResponse, dynamic>(ConfirmBalanceQueries.GetConfirmAccountReceivable, parameter);
            return confirmAccountReceivable.ToList();
        }

        public async Task<List<DtConfirmAccountReceivableDetailResponse>> GetConfirmBalanceDetailsAsync(string dbCode, int confirmBalanceId)
        {
            var param = new { DB_CODE = dbCode, ID = confirmBalanceId };
            var results = await sqlDataAccess.LoadData<DtConfirmAccountReceivableDetailResponse, dynamic>(ConfirmBalanceQueries.GetConfirmBalanceDetail, param);
            return results.ToList();
        }

        public async Task<int> UpdateConfirmBalanceDetailsAsync(DtConfirmAccountReceivableDetail req)
        {
            
            var param = new
            {
                BALANCE = req.Balance,
                DESCRIPTION = req.Description,
                IS_AGREE = req.IsAgree,
                IS_MET = req.CustomerStatus,
                UPDATED_DATE = DateTime.Now,
                UPDATED_BY = req.UpdatedBy,
                INVOICE_CODE = req.InvoiceCode,
                HEADER_ID = req.HeaderId
            };
            var rowAffected = await sqlDataAccess.ExecuteAsync(ConfirmBalanceQueries.UpdateConfirmBalanceDetail, param);
            return rowAffected ;
        }

        public async Task<int> AddConfirmBalanceDetailsAsync(List<DtConfirmAccountReceivableDetail> requests)
        {
            if(dbConnection.State == ConnectionState.Closed)
            { 
                dbConnection.Open();
            }
            var transaction= dbConnection.BeginTransaction();
            try
            {
                var rowAffected = 0;
                foreach (var item in requests)
                {
                    var paramCheckExistInvoice = new
                    {
                        DB_CODE = item.DbCode,
                        HEADER_ID = item.HeaderId,
                        INVOICE_CODE = item.InvoiceCode
                    };
                    var exist = await dbConnection.ExecuteScalarAsync<bool>(ConfirmBalanceQueries.CheckExistInvoice, paramCheckExistInvoice, transaction);
                    if (exist) continue;
                    var param = new
                    {
                        HEADER_ID = item.HeaderId,
                        DB_CODE = item.DbCode,
                        INVOICE_CODE = item.InvoiceCode,
                        CUSTOMER_NAME = item.CustomerName,
                        CUSTOMER_CODE = item.CustomerCode,
                        INVOICE_AMOUNT = item.InvoiceAmount,
                        CREATED_DATE = DateTime.Now,
                        CREATED_BY = item.CreatedBy,
                        STATUS = "Pending"
                    };
                    rowAffected += await dbConnection.ExecuteAsync(ConfirmBalanceQueries.AddConfirmBalanceDetail, param, transaction);
                }
                if (rowAffected == requests.Count)
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
                return 0;
            }
            finally
            {
                dbConnection.Close();
            }
        }
    }
}
