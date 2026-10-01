using System.Text;
using BC.PAYMENT.CORE.Contracts.Response.Expense;
using BC.PAYMENT.CORE.Contracts.Response.Paid;
using BC.PAYMENT.CORE.Entities;
using Dapper;
using NewInvoiceModel = BC.PAYMENT.CORE.Entities.Invoice.NewInvoiceModel;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Invoice;

public class PaymentInvoiceRepository(ISqlDataAccess sqlDataAccess) : IPaymentInvoiceRepository
{
    public async Task<bool> IsExistsPaymentHeaderId(string dbCode, DateTime invoiceDate, string deliveryId)
    {
        return await sqlDataAccess.LoadSingleData<bool, dynamic>(PaymentInvoiceQueries.IsExistsHeaderId, new
        {
            DELIVERY_ID = deliveryId,
            INVOICE_DIVIDEND_DATE = invoiceDate,
            DB_CODE = dbCode
        });
    }

    public async Task<int> CreatePaymentHeader(PaymentInvoiceHeader headerModel)
    {
        var param = new
        {
            DB_CODE = headerModel.DbCode,
            DELIVERY_ID = headerModel.DeliveryId,
            PERIOD = headerModel.Period,
            ENTRIES_CODE = headerModel.EntriesCode,
            CREATED_DATE = headerModel.CreatedDate,
            CREATED_BY = headerModel.CreatedBy,
            STATUS = "1",
            INVOICE_DIVIDEND_DATE = headerModel.InvoiceDividendDate
        };
        return await sqlDataAccess.ExecuteScalarAsync<int, dynamic>(PaymentInvoiceQueries.CreatePaymentHeader, param);
    }

    public async Task<int> GetPaymentHeaderId(DateTime invoiceDividendDate, string deliveryId, string dbCode)
    {
        var param = new
        {
            DB_CODE = dbCode,
            DELIVERY_ID = deliveryId,
            INVOICE_DIVIDEND_DATE = invoiceDividendDate
        };
        return await sqlDataAccess.ExecuteScalarAsync<int, dynamic>(PaymentInvoiceQueries.GetPaymentHeaderId, param);
    }

    public async Task<int> CreatePaymentDetailAsync(BcPaymentDetail model)
    {
        var argument = new
        {
            DB = model.DbCode,
            DID = model.DeliveryId,
            TT = model.Total,
            D = model.Dollar,
            R = model.Riel,
            E = model.Exchange,
            D1 = string.IsNullOrWhiteSpace(model.DescExp1) ? null : model.DescExp1,
            EA1 = model.ExpAmount1,
            D2 = string.IsNullOrWhiteSpace(model.DescExp2) ? null : model.DescExp2,
            EA2 = model.ExpAmount2,
            D3 = string.IsNullOrWhiteSpace(model.DescExp3) ? null : model.DescExp3,
            EA3 = model.ExpAmount3,
            MB = model.MoneyBias,
            CD = model.CreatedDate?.Date,
            CB = model.CreatedBy,
            EC = model.EntriesCode
        };
        return await sqlDataAccess.ExecuteAsync(PaymentInvoiceQueries.CreateBcPaymentDetail, argument);
    }

    public async Task<int> CreatePcPaymentInvoiceAsync(PcPaymentInvoice item)
    {
        var rowAffected = 0;
        var param = new
        {
            DB = item.DbCode,
            DB_CODE = item.DbCode,
            DDID = item.DividedInvoiceId,
            DIVIDED_ID = item.DividedInvoiceId,
            PAYMENT_HEADER_ID = item.PaymentHeaderId,
            AMOUNT = item.Amount,
            CREATED_BY = item.CreatedBy,
            CREATED_DATE = item.CreatedDate
        };
        rowAffected += await sqlDataAccess.ExecuteAsync(PaymentInvoiceQueries.CreatePaymentInvoice, param);
        return rowAffected;
    }

    public async Task<List<PaymentInvoiceResponse>> GetPaymentInvoiceDetailByDateAsync(string dbCode, DateTime fromDate,
        DateTime toDate)
    {
        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate,
            TO_DATE = toDate
        };
        var condition = $@"WHERE P.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE
        AND P.DB_CODE = @DB_CODE";
        var results =
            await sqlDataAccess.LoadData<PaymentInvoiceResponse, dynamic>(
                PaymentInvoiceQueries.GetPaymentInvoiceDetail(condition), param);
        return results.ToList();
    }

    public async Task<List<PaymentInvoiceResponse>> GetPaymentInvoiceDetailByPeriodAsync(string dbCode, int month,
        int year)
    {
        var param = new
        {
            DB_CODE = dbCode,
            MONTH = month,
            YEAR = year
        };
        var condition = $@"MONTH(P.CREATE_DATE) = @MONTH AND YEAR(P.CREATE_DATE) = @YEAR AND P.DB_CODE = @DB_CODE";
        var results =
            await sqlDataAccess.LoadData<PaymentInvoiceResponse, dynamic>(
                PaymentInvoiceQueries.GetPaymentInvoiceDetail(condition), param);
        return results.ToList();
    }

    public async Task<List<PaymentInvoiceResponse>> GetPaymentInvoiceDetailByInvoiceCodeAsync(string dbCode,
        DateTime date, string invoiceCode)
    {
        var param = new
        {
            DB_CODE = dbCode,
            DATE = date,
            INVOICE_CODE = invoiceCode
        };
        var addOnField =
            $@", PAID.CREATED_DATE PaymentDate, CASE WHEN A.SUBMITTED_ID IS NULL THEN 'Submitted' ELSE 'Approved' END [StatusDesc] ";
        var addReference = $@"INNER JOIN BCINVOICE_SUMITTED S ON S.INVOICE_ID = N.ID and S.DB_CODE = @DB_CODE
            LEFT JOIN BCAPPROVAL_INVOICE A ON A.SUBMITTED_ID = S.ID";
        var criteria = $@"WHERE N.DB_CODE = @DB_CODE AND P.CREATE_DATE <= @DATE AND N.TRANSACTION_REF = @INVOICE_CODE";
        var sortBy = $@"ORDER BY Total";
        var groupBy = $@",PAID.CREATED_DATE,A.SUBMITTED_ID";
        var results = await sqlDataAccess.LoadData<PaymentInvoiceResponse, dynamic>
        (PaymentInvoiceQueries.GetPaymentInvoiceDetail(criteria, addReference: addReference, sortBy: sortBy, addOnFields: addOnField, addOnGroupBy: groupBy),
            param);
        return results.ToList();
    }

    public async Task<int> UpdatePaidValueAsync(PcPaymentInvoice paymentInvoice, NewInvoiceModel invoice,
        PcEditDividedInvoice editDividedInvoice)
    {
        var totalAffected = 0;
        var oldMoney = editDividedInvoice.OldAmount != 0 ? editDividedInvoice.OldAmount : invoice.InvoiceAmount;
        var newMoney = editDividedInvoice.NewAmount;
        var dividedId = editDividedInvoice.DividedId != 0
            ? editDividedInvoice.DividedId
            : paymentInvoice.DividedInvoiceId;
        var invoiceId = invoice.InvoiceId != 0 ? invoice.InvoiceId : invoice.Id;
        var dbCode = !string.IsNullOrWhiteSpace(editDividedInvoice.DbCode)
            ? editDividedInvoice.DbCode
            : !string.IsNullOrWhiteSpace(paymentInvoice.DbCode)
                ? paymentInvoice.DbCode
                : invoice.DbCode ?? string.Empty;
        var userName = !string.IsNullOrWhiteSpace(editDividedInvoice.CreatedBy)
            ? editDividedInvoice.CreatedBy
            : !string.IsNullOrWhiteSpace(paymentInvoice.CreatedBy)
                ? paymentInvoice.CreatedBy
                : invoice.CreatedBy ?? string.Empty;
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
            var insertResult = await sqlDataAccess.ExecuteAsync(insertEditSql, insertParam);
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
                totalAffected += await sqlDataAccess.ExecuteAsync(updateInvoiceSql, updateInvoiceParam);
            }
        }

        var paidId = paymentInvoice.PaymentId;
        if (paidId <= 0 && paymentInvoice.DividedInvoiceId > 0)
        {
            const string getPaymentIdSql =
                "SELECT PAYMENT_ID FROM PC_PAYMENT_INVOICE WHERE DIVDIE_INVOICE_ID = @DIVIDED_ID;";
            paidId = await sqlDataAccess.ExecuteScalarAsync<int, dynamic>(getPaymentIdSql,
                new { DIVIDED_ID = paymentInvoice.DividedInvoiceId });
        }

        if (paidId > 0)
        {
            const string getPaidSql = "SELECT AMOUNT FROM PC_PAYMENT_INVOICE WHERE PAYMENT_ID = @ID;";
            var currentPaid = await sqlDataAccess.ExecuteScalarAsync<double?, dynamic>(getPaidSql, new { ID = paidId });
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
                totalAffected += await sqlDataAccess.ExecuteAsync(updatePaymentSql, updatePaymentParam);
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
        return await sqlDataAccess.ExecuteAsync(PaymentInvoiceQueries.DeletePaymentInvoice, param);
    }

    public async Task<List<BcPaymentDetailResponse>> LoadBcPaymentDetailAsync(
        string dbCode,
        string? deliveryId = null,
        DateTime? date = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int? month = null,
        int? year = null,
        int? period = null)
    {
        var parameters = new DynamicParameters();
        parameters.Add("DB_CODE", dbCode);

        var conditionBuilder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(deliveryId))
        {
            conditionBuilder.Append(" AND DELIVERYID = @DELIVERY_ID");
            parameters.Add("DELIVERY_ID", deliveryId);
        }

        if (date.HasValue)
        {
            conditionBuilder.Append(" AND CREATED_DATE >= @DATE AND CREATED_DATE < DATEADD(DAY, 1, @DATE)");
            parameters.Add("DATE", date.Value.Date);
        }
        else if (fromDate.HasValue && toDate.HasValue)
        {
            conditionBuilder.Append(" AND CREATED_DATE >= @FROM_DATE AND CREATED_DATE < DATEADD(DAY, 1, @TO_DATE)");
            parameters.Add("FROM_DATE", fromDate.Value.Date);
            parameters.Add("TO_DATE", toDate.Value.Date);
        }
        else if (fromDate.HasValue)
        {
            conditionBuilder.Append(" AND CREATED_DATE >= @FROM_DATE");
            parameters.Add("FROM_DATE", fromDate.Value.Date);
        }
        else if (toDate.HasValue)
        {
            conditionBuilder.Append(" AND CREATED_DATE < DATEADD(DAY, 1, @TO_DATE)");
            parameters.Add("TO_DATE", toDate.Value.Date);
        }
        else
        {
            var filterMonth = month;
            var filterYear = year;

            if (period.HasValue && !filterMonth.HasValue)
            {
                if (period.Value > 10000)
                {
                    filterYear ??= period.Value / 100;
                    filterMonth = period.Value % 100;
                }
                else
                {
                    filterMonth = period.Value;
                }
            }

            if (filterMonth.HasValue && filterYear.HasValue)
            {
                conditionBuilder.Append(" AND MONTH(CREATED_DATE) = @MONTH AND YEAR(CREATED_DATE) = @YEAR");
                parameters.Add("MONTH", filterMonth.Value);
                parameters.Add("YEAR", filterYear.Value);
            }
            else if (filterMonth.HasValue)
            {
                conditionBuilder.Append(" AND MONTH(CREATED_DATE) = @MONTH");
                parameters.Add("MONTH", filterMonth.Value);
            }
            else if (filterYear.HasValue)
            {
                conditionBuilder.Append(" AND YEAR(CREATED_DATE) = @YEAR");
                parameters.Add("YEAR", filterYear.Value);
            }
        }

        var sql = PaymentInvoiceQueries.GetBcPaymentDetail(conditionBuilder.ToString());
        var results = await sqlDataAccess.LoadData<BcPaymentDetailResponse, dynamic>(sql, parameters);
        return results.ToList();
    }

    public async Task<List<PaymentInvoiceResponse>> GetPaymentInvoiceDetailExcludeSubmitInvoiceAsync(string dbCode,
        DateTime fromDate, DateTime toDate)
    {
        var addReference =
            $@"LEFT JOIN (SELECT TRANS_REF FROM {dbCode}SISOHDR WHERE VOID_STATUS = 'N') H ON H.TRANS_REF = N.TRANSACTION_REF ";
        var criteria = $@"WHERE
            PAID.STATUS = '1'
            AND P.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND P.DB_CODE = @DB_CODE
            AND N.ID NOT IN (SELECT INVOICE_ID FROM BCINVOICE_SUMITTED S WHERE S.SUBMISSION_STATUS != 'Cancel'  AND S.DB_CODE = @DB_CODE)
			AND (LEFT(N.TRANSACTION_REF,2) = 'FF' OR H.TRANS_REF IS NOT NULL)";
        var addOnFields =
            $@",ISNULL(CASE WHEN N.HEADER_TRANSACTION_VALUES = PAID.AMOUNT THEN PAID.AMOUNT END, 0) 'Paid',N.STATUS [InvoiceType],P.CREATE_DATE CreateDate";
        var addOnGroupBy = $@",N.STATUS,P.CREATE_DATE";
        var response = await sqlDataAccess.LoadData<PaymentInvoiceResponse, dynamic>(
            PaymentInvoiceQueries.GetPaymentInvoiceDetail(criteria, addOnFields, addOnGroupBy, addReference), new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            });
        return response.ToList();
    }
}