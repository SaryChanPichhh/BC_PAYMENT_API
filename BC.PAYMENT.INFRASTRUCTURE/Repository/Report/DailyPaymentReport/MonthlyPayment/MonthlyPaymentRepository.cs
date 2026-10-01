namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Report.DailyPaymentReport.MonthlyPayment;

public class MonthlyPaymentRepository : ITotalMonthlyPaymentRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public MonthlyPaymentRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<MonthlyPaymentModel>> GetAllPaidInvoiceByDateAsync(string dbCode, DateTime fromDate,
        DateTime toDate)
    {
        var sql =
            $@"SELECT ROW_NUMBER() OVER (ORDER BY CUSTOMER_CODE) RowNum,PPI.CREATED_DATE CsreateDate,N.TRANSACTION_REF TransactionRef,
                       N.HEADER_TRANSACTION_VALUES TransactionValues,1 [AmountInvoice]
                       FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE PDI on N.ID = PDI.INVOICE_ID 
                       INNER JOIN PC_PAYMENT_INVOICE PPI ON PPI.DIVDIE_INVOICE_ID = PDI.DIVIDED_INVOICE_ID AND N.HEADER_TRANSACTION_VALUES = PPI.AMOUNT
                       WHERE N.DB_CODE = @DB_CODE AND PPI.DB_CODE = @DB_CODE AND PDI.DB_CODE = @DB_CODE
                       AND PDI.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE";
        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate,
            TO_DATE = toDate
        };
        var execute = await _sqlDataAccess.LoadData<MonthlyPaymentModel, dynamic>(sql, param);
        return execute.ToList();
    }

    public async Task<List<Dictionary<string, object>>> GetMonthlyHistoryPaidInvoiceByDateAsync(string dbCode,
        DateTime fromDate,
        DateTime toDate)
    {
        var storeProcedure = $@"BC_HISTORY_PAID_INVOICES";
        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate,
            TO_DATE = toDate
        };
        var execute =
            await _sqlDataAccess.LoadData<dynamic, dynamic>(storeProcedure, param, CommandType.StoredProcedure);
        var dictionaryList = execute
            .Select(row => (IDictionary<string, object>)row)
            .Select(dict => dict.ToDictionary(k => k.Key, v => v.Value))
            .ToList();
        return dictionaryList;
    }
}