using System.Data;
using BC.PAYMENT.APPLICATION.Interfaces.Payment;
using BC.PAYMENT.CORE.Contracts.Payment;
using BC.PAYMENT.CORE.Entities;
using NewInvoiceModel = BC.PAYMENT.CORE.Entities.Invoice.NewInvoiceModel;
using BC.PAYMENT.INFRASTRUCTURE.Helper;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.SQL.Queries;
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

        public async Task<int> UpdatePaidValueAsync(PcPaymentInvoice paymentInvoice, NewInvoiceModel invoice, PcEditDividedInvoice editDividedInvoice)
        {
            var totalAffected = 0;

            var oldMoney = editDividedInvoice.OldAmount != 0 ? editDividedInvoice.OldAmount : invoice.InvoiceAmount;
            var newMoney = editDividedInvoice.NewAmount;
            var dividedId = editDividedInvoice.DividedId != 0 ? editDividedInvoice.DividedId : paymentInvoice.DividedInvoiceId;
            var invoiceId = invoice.InvoiceId != 0 ? invoice.InvoiceId : invoice.Id;
            var dbCode = !string.IsNullOrWhiteSpace(editDividedInvoice.DbCode)
                ? editDividedInvoice.DbCode
                : (!string.IsNullOrWhiteSpace(paymentInvoice.DbCode) ? paymentInvoice.DbCode : (invoice.DbCode ?? string.Empty));
            var userName = !string.IsNullOrWhiteSpace(editDividedInvoice.CreatedBy)
                ? editDividedInvoice.CreatedBy
                : (!string.IsNullOrWhiteSpace(paymentInvoice.CreatedBy) ? paymentInvoice.CreatedBy : (invoice.CreatedBy ?? string.Empty));
            var createDate = editDividedInvoice.CreateDate != default ? editDividedInvoice.CreateDate : DateTime.Now;

            if (!oldMoney.Equals(newMoney))
            {
                const string insertEditSql = @"
                    INSERT INTO PC_EDIT_VALUE_DIVIDED_INVOICE (DB_CODE, DIVIDED_INVOICE_ID, OLD_VALUE, NEW_VALUE, DESCRIPTION, CREATE_DATE, CREATE_BY)
                    VALUES (@DB, @DDID, @OV, @NV, @D, @CD, @CB);";

                var insertParam = new
                {
                    DB = dbCode,
                    DDID = dividedId,
                    OV = oldMoney,
                    NV = newMoney,
                    D = editDividedInvoice.Description,
                    CD = createDate,
                    CB = userName
                };

                var insertResult = await _sqlDataAccess.ExecuteAsync(insertEditSql, insertParam);
                if (insertResult > 0)
                {
                    totalAffected += insertResult;

                    var updateInvoiceSql = invoiceId != 0
                        ? @"UPDATE NEW_INVOICE SET HEADER_TRANSACTION_VALUES = @VALUE WHERE ID = @ID;"
                        : @"UPDATE NEW_INVOICE SET HEADER_TRANSACTION_VALUES = @VALUE WHERE ID IN (SELECT INVOICE_ID FROM PC_DIVIDED_INVOICE WHERE DIVIDED_INVOICE_ID = @ID);";

                    var updateInvoiceParam = new
                    {
                        VALUE = newMoney,
                        ID = invoiceId != 0 ? invoiceId : dividedId
                    };

                    totalAffected += await _sqlDataAccess.ExecuteAsync(updateInvoiceSql, updateInvoiceParam);
                }
            }

            var paidId = paymentInvoice.PaymentId;
            if (paidId <= 0 && paymentInvoice.DividedInvoiceId > 0)
            {
                const string getPaymentIdSql = "SELECT PAYMENT_ID FROM PC_PAYMENT_INVOICE WHERE DIVDIE_INVOICE_ID = @DIVIDED_ID;";
                paidId = await _sqlDataAccess.ExecuteScalarAsync<int, dynamic>(getPaymentIdSql, new { DIVIDED_ID = paymentInvoice.DividedInvoiceId });
            }

            if (paidId > 0)
            {
                const string getPaidSql = "SELECT AMOUNT FROM PC_PAYMENT_INVOICE WHERE PAYMENT_ID = @ID;";
                var currentPaid = await _sqlDataAccess.ExecuteScalarAsync<double?, dynamic>(getPaidSql, new { ID = paidId });

                if (currentPaid.HasValue && !currentPaid.Value.Equals(paymentInvoice.Amount))
                {
                    const string updatePaymentSql = @"
                        UPDATE PC_PAYMENT_INVOICE 
                        SET AMOUNT = @AMOUNT 
                        WHERE PAYMENT_ID = @ID;";

                    var updatePaymentParam = new
                    {
                        AMOUNT = paymentInvoice.Amount,
                        ID = paidId
                    };

                    totalAffected += await _sqlDataAccess.ExecuteAsync(updatePaymentSql, updatePaymentParam);
                }
            }

            return totalAffected;
        }

        public async Task<int> DeletePaymentInvoiceAsync(int paymentId, int dividedId)
        {
            var param = new
            {
                PAYMENT_ID = paymentId,
                DII = dividedId
            };
            return await _sqlDataAccess.ExecuteAsync(PaymentInvoiceQueries.DeletePaymentInvoice, param);
        }
    }
}
