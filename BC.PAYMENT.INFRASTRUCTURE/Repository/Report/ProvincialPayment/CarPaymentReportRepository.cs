namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Report.ProvincialPayment;

public class CarPaymentReportRepository : ICarPaymentReportRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public CarPaymentReportRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<CarPaymentReportModel>> GetCarPaymentReportByDateAsync(DateTime fromDate, DateTime toDate)
    {
        var sql =
            @"SELECT ID Id,TRANSACTION_DATE​ TransactionDate,CODE TransactionCode,CUSTOMER_CODE CustomerCode,CUSTOMER_NAME CustomerName,VALUE InvoiceValue,ISNULL(CASE WHEN VALUE > Payment.AmountPayment THEN Payment.AmountPayment END,0)  as HalfPayment,               
                 ISNULL(CASE WHEN VALUE = Payment.AmountPayment THEN Payment.AmountPayment END,0) as FullPayment,VALUE - Payment.AmountPayment Total,
                 CASE WHEN TYPE = 'N' THEN N'ថ្មី' WHEN TYPE = 'C' THEN N'ដូរ' WHEN TYPE = 'O' THEN N'ចាស់' END INVOICE 
                 FROM BCSTOCK_CAR BC INNER JOIN 
                 (SELECT REQUEST_ID, INVOICE_ID, SUM(PAID_AMOUNT) AmountPayment FROM BC_STOCK_CAR_REQUESTS_INVOICE_PAID 
                 GROUP BY INVOICE_ID, REQUEST_ID) Payment On Payment.INVOICE_ID = BC.ID 
                 WHERE TRANSACTION_DATE  BETWEEN @FROM_DATE and @TO_DATE";
        var param = new
        {
            FROM_DATE = fromDate,
            TO_DATE = toDate
        };
        var execute = await _sqlDataAccess.LoadData<CarPaymentReportModel, dynamic>(sql, param);
        return execute.ToList();
    }

    public async Task<List<CarPaymentReportModel>> GetCarPaymentReportByPeriodAsync(int period)
    {
        var sql =
            @"SELECT ID Id,TRANSACTION_DATE​ TransactionDate,CODE TransactionCode,CUSTOMER_CODE CustomerCode,CUSTOMER_NAME CustomerName,VALUE InvoiceValue,ISNULL(CASE WHEN VALUE > Payment.AmountPayment THEN Payment.AmountPayment END,0)  as HalfPayment,               
                 ISNULL(CASE WHEN VALUE = Payment.AmountPayment THEN Payment.AmountPayment END,0) as FullPayment,VALUE - Payment.AmountPayment Total,
                 CASE WHEN TYPE = 'N' THEN N'ថ្មី' WHEN TYPE = 'C' THEN N'ដូរ' WHEN TYPE = 'O' THEN N'ចាស់' END INVOICE 
                 FROM BCSTOCK_CAR BC INNER JOIN (SELECT REQUEST_ID, INVOICE_ID, SUM(PAID_AMOUNT) AmountPayment FROM BC_STOCK_CAR_REQUESTS_INVOICE_PAID 
                 GROUP BY INVOICE_ID, REQUEST_ID) Payment On Payment.INVOICE_ID = BC.ID 
                 WHERE PERIOD = @PERIOD";
        var param = new
        {
            PERIOD = period
        };
        var execute = await _sqlDataAccess.LoadData<CarPaymentReportModel, dynamic>(sql, param);
        return execute.ToList();
    }
}