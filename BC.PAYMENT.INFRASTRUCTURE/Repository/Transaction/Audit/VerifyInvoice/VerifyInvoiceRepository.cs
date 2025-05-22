

using BC.PAYMENT.APPLICATION.Interfaces.Transaction.Audit.VerifyInvoice;
using BC.PAYMENT.CORE.Entities.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.Audit.VerifyInvoice;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.Audit.VerifyInvoice
{
    public class VerifyInvoiceRepository : IVerifyInvoiceRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public VerifyInvoiceRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<VerifyInvoiceModel>> GetVerifyInvoicesAsync(string dbCode)
        {
            const string sql = @"SELECT CODE as 'Transaction',PERIOD 'Period',CUSTOMER_CODE as 'CustomerCode',
            ACC_NAME_KH as 'CustomerName',MONEY as 'Total',EMPLOYEE as 'Employee',IS_VERIFY Status FROM OLD_INVOICE 
            WHERE CREATE_DATE = CONVERT(DATE,GETDATE()) AND DB_CODE = @DB_CODE ORDER BY EMPLOYEE";
            var param = new { DB_CODE = dbCode };
            var execute = await _sqlDataAccess.LoadData<VerifyInvoiceModel,dynamic>(sql,param);
            return execute.ToList();
        }

        public async Task<int> InsertVerifyInvoicesAsync(List<OldInvoicesModel> model)
        {
            var affectedRow = 0;
            var sql =
                $@"IF NOT EXISTS (SELECT CODE FROM OLD_INVOICE WHERE CODE = @CODE AND CREATE_DATE = CONVERT(DATE,GETDATE()) AND DB_CODE = @DB) 
                        INSERT INTO OLD_INVOICE(DB_CODE,CODE,CUSTOMER_CODE,ACC_NAME_KH,MONEY,EMPLOYEE,CREATE_DATE,CREATE_BY,STATUS,IS_VERIFY)
                        VALUES(@DB_CODE, @CODE,@CUS_CODE, @ACC_NAME, @MONEY, @EMP, @CD, @CB, @STATUS, @VERIFY)";
            foreach(var item in model)
            {
                var param = new
                {
                    DB_CODE = item.DbCode,
                    CODE = item.TransRef,
                    CUS_CODE = item.CustomerCode,
                    ACC_NAME = item.CustomerName,
                    MONEY = item.InvoiceValue,
                    EMP = item.Employee,
                    CD = DateTime.Now,
                    CB = item.CreateBy,
                    STATUS = true,
                    VERIFY = true
                };
                 affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            }
            return affectedRow;
        }
    }
}
