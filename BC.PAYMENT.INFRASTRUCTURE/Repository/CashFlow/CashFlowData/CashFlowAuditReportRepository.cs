namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CashFlow.CashFlowData;

public class CashFlowAuditReportRepository(ISqlDataAccess sqlDataAccess) : ICashFlowAuditReportRepository
{
    public async Task<List<PaymentCashFlowSubmittedModel>> GetPaymentCashFlowAuditReportByDateAsync(string dbCode,
        DateTime fromDate, DateTime toDate, string status)
    {
        const string sql =
            @"SELECT DATE Date,SUBMITTED_ID SubmittedId,S.NAME Name,S.AMOUNT Amount,S.CURRENCY_FORMAT CurrencyFormat,S.EXCHANGE_RATE ExchangeRate,S.CREATED_DATE CreatedDate,S.CREATED_BY CreatedBy,
            UPDATED_DATE UpdatedDate,UPDATED_BY UpdatedBy,S.STATUS Status
            FROM BC_PAYMENT_MONEY_CASH_FLOW_SUBMITTED S WHERE S.DB_CODE = @DB_CODE
			AND DATE BETWEEN @FROM_DATE AND @TO_DATE AND STATUS = @STATUS";
        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate,
            TO_DATE = toDate,
            STATUS = status
        };
        var results = await sqlDataAccess.LoadData<PaymentCashFlowSubmittedModel, dynamic>(sql, param);
        return results.ToList();
    }
}