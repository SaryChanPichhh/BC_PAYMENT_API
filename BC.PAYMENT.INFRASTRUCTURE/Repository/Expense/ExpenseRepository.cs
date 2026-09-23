using BC.PAYMENT.APPLICATION.Interfaces.Expense;
using BC.PAYMENT.CORE.Contracts.Request.Expense;
using BC.PAYMENT.CORE.Contracts.Response.Expense;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Expense;

public class ExpenseRepository(ISqlDataAccess sqlDataAccess ) : IExpenseRepository
{
    public async Task<int> CreatePaymentExpense(PaymentInvoiceExpense model)
    {
        var argument = new
        {
            DB_CODE = model.DbCode,
            PAYMENT_HEADER_ID = model.PaymentHeaderId,
            NAME = model.Name,
            DESCRIPTION = model.Description,
            QUANTITY = model.Quantity,
            UNIT_PRICE = model.UnitPrice,
            TOTAL = model.Total,
            CREATED_DATE = model.CreatedDate,
            CREATED_BY = model.CreatedBy,
            CURRENCY_TYPE = model.CurrencyType,
            EXCHANGE_RATE = model.ExchangeRate
        };
        var rowAffected = await sqlDataAccess.ExecuteAsync(ExpenseQueries.CreatePaymentExpense, argument);
        return rowAffected;
    }

    public async Task<List<BcPaymentDetailResponse>> LoadBcPaymentDetailAsync(string dbCode,string deliveryId,DateTime date)
    {
        var argument = new
        {
            DATE = date,
            DELIVERY_ID = deliveryId,
            DB_CODE = dbCode
        };
        var condition = string.Empty;
        if (!string.IsNullOrEmpty(deliveryId))
            condition += @" AND DELIVERYID = @DELIVERY_ID";
        condition += @" AND CREATED_DATE >= @DATE AND CREATED_DATE < DATEADD(DAY, 1, @DATE)";
        var data = await sqlDataAccess.LoadData<BcPaymentDetailResponse, dynamic>(ExpenseQueries.GetBcPaymentDetail(condition), argument);
        return data.ToList();
    }

    public async Task<List<BcPaymentDetailResponse>> LoadBcPaymentDetailAsync(string dbCode, DateTime fromDate, DateTime toDate)
    {
        var argument = new
        {
            FROM_DATE = fromDate,
            TO_DATE = toDate,
            DB_CODE = dbCode
        };
        var condition = @" AND CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE";
        var data = await sqlDataAccess.LoadData<BcPaymentDetailResponse, dynamic>(ExpenseQueries.GetBcPaymentDetail(condition), argument);
        return data.ToList();
    }

    public async Task<List<BcPaymentDetailResponse>> LoadBcPaymentDetailWithPaidAsync(string dbCode, DateTime fromDate, DateTime toDate)
    {
        var argument = new
        {
            FROM_DATE = fromDate,
            TO_DATE = toDate,
            DB_CODE = dbCode
        };
        var addReference = $@"INNER JOIN(SELECT D.DELIVERY_ID, SUM(P.AMOUNT) PAID FROM PC_DIVIDED_INVOICE D INNER JOIN PC_PAYMENT_INVOICE P ON P.DIVDIE_INVOICE_ID = D.DIVIDED_INVOICE_ID
                WHERE D.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND P.DB_CODE = @DB_CODE
                GROUP BY D.DELIVERY_ID) TAB1
                ON TAB1.DELIVERY_ID = D.DELIVERIES_ID";
        var criteria = @" AND CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE";
        var data = await sqlDataAccess.LoadData<BcPaymentDetailResponse, dynamic>
            (ExpenseQueries.GetBcPaymentDetail(criteria:criteria,addReference:addReference), argument);
        return data.ToList();
    }

    public async Task<List<BcPaymentDetailResponse>> LoadBcPaymentDetailAsync(string dbCode, int month, int year)
    {
        var argument = new
        {
            YEAR = year,
            MONTH = month,
            DB_CODE = dbCode
        };
        var condition = @" AND YEAR(CREATED_DATE) = @YEAR AND MONTH(CREATED_DATE) = @MONTH";
        var data = await sqlDataAccess.LoadData<BcPaymentDetailResponse, dynamic>(ExpenseQueries.GetBcPaymentDetail(condition), argument);
        return data.ToList();
    }

    public async Task<int> UpdateBcPaymentDetailAsync(UpdateBcPaymentDetailRequest request)
    {
        var param = new
        {
            TT = request.Total,
            D = request.Dollar,
            R = request.Riel,
            E = request.Exchange,
            D1 = request.DescExp1,
            D2 = request.DescExp2,
            D3 = request.DescExp3,
            EA1 = request.ExpAmount1,
            EA2 = request.ExpAmount2,
            EA3 = request.ExpAmount3,
            B = request.MoneyBias,
            ID = request.Id
        };
        return await sqlDataAccess.ExecuteAsync(ExpenseQueries.UpdateBcPaymentDetail, param);
    }
}