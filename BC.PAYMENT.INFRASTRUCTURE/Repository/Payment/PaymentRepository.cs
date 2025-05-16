using System.Data;
using BC.PAYMENT.APPLICATION.Interfaces.Payment;
using BC.PAYMENT.CORE.DTO.Payment;
using BC.PAYMENT.CORE.Entities.Payment;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.INFRASTRUCTURE.Helper;
using Dapper;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Payment
{
    public class PaymentRepository:IPaymentRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        public PaymentRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        public async Task<List<DeliveryPayment>> GetDeliveryPaidInvoice(string dbCode, string deliveryId, DateTime divideDate)
        {
            var sql = "PM_SELECT_DELIVERY_PAID";
            var param = new
            {
                DB_CODE = dbCode,
                DATE = divideDate,
                DELIVERY_ID = deliveryId
            };
            var result = await _sqlDataAccess.LoadData<DeliveryPayment, dynamic>(sql, param, CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<int> CreatePaymentHeader(PaymentInvoiceHeaderDTO headerModel)
        {
            var sql = "PM_INSERT_PAYMENT_INVOICE_HEADER";
            var param = new
            {
                DB_CODE = headerModel.DbCode,
                DELIVERY_ID = headerModel.DeliveryId,
                PERIOD = headerModel.Period,
                ENTRIES_CODE = headerModel.EntriesCode,
                CREATED_DATE = headerModel.CreatedDate,
                CREATED_BY = headerModel.CreatedBy,
                STATUS = headerModel.Status,
                INVOICE_DIVIDEND_DATE = headerModel.InvoiceDividendDate,

            };

            var result = await _sqlDataAccess.ExecuteAsync(sql, param, commandType: CommandType.StoredProcedure);
            return result;

        }

        public async Task<int> GetPaymentHeaderId(DateTime invoiceDividendDate, string deliveryId, string dbCode)
        {
            const string sql =
                @"SELECT ID FROM PAYMENT_INVOICE_HEADER WHERE DB_CODE = @DB_CODE AND DELIVERY_ID = @DELIVERY_ID AND INVOICE_DIVIDEND_DATE = @INVOICE_DIVIDEND_DATE";
            var param = new
            {
                DB_CODE = dbCode,
                DELIVERY_ID = deliveryId,
                INVOICE_DIVIDEND_DATE = invoiceDividendDate
            };
            return await _sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, param);
        }

        public async Task<bool> CheckExistsPaymentHeader(DateTime invoiceDividendDate, string deliveryId, string dbCode)
        {
            const string sql =
                @"SELECT CAST(COUNT(*) AS BIT) FROM PAYMENT_INVOICE_HEADER WHERE INVOICE_DIVIDEND_DATE = @INVOICE_DIVIDEND_DATE AND DELIVERY_ID = @DELIVERY_ID
            AND DB_CODE = @DB_CODE";
            var param = new
            {
                INVOICE_DIVIDEND_DATE = invoiceDividendDate,
                DELIVERY_ID = deliveryId,
                DB_CODE = dbCode
            };
            return await _sqlDataAccess.ExecuteScalarAsync<bool, dynamic>(sql, param);
        }

        public async Task<int> CreatePaymentExpense(List<InvoiceExpenseDTO> dto)
        {
            const string sql = "PM_INSERT_INVOICE_EXPENSE";

            var dataTable = AppExtension.ConvertToDataTable(dto);

            // Define the parameter for the stored procedure
            var param = new DynamicParameters();
            param.Add("@ExpenseTable", dataTable.AsTableValuedParameter("PM_EXPENSE")); // Specify table type name

            var rowAffected = await _sqlDataAccess.ExecuteAsync(sql, param, CommandType.StoredProcedure);
            return rowAffected;
        }
    }
}
