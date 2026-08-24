namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Report.DailyPaymentReport.PaidInvoice
{
    public class PaidInvoiceRepository : IPaidInvoiceRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public PaidInvoiceRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<PaidInvoiceReportModel>> GetPaidInvoiceByDateAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql = $@"SELECT P.CREATE_DATE CreatedDate,D.DELIVERIES_KHMER Delivery,   
                 N.CUSTOMER_CODE CustomerCode,   
                 N.ACC_NAME_KH CustomerName,
                 Customer.Market,
                 Customer.Area,
				 Store,
                 N.TRANSACTION_REF TransactionCode,      
                 N.HEADER_TRANSACTION_VALUES InvoiceValue,
                 PAID.AMOUNT Paid
                 FROM  TB_BCDELIVERIES D      
			   INNER JOIN PC_DIVIDED_INVOICE P ON D.DELIVERIES_ID = P.DELIVERY_ID 
                 INNER JOIN NEW_INVOICE N ON N.ID = P.INVOICE_ID   
                 INNER JOIN PC_PAYMENT_INVOICE PAID ON PAID.DIVDIE_INVOICE_ID = P.DIVIDED_INVOICE_ID   
                 LEFT JOIN (SELECT ADD_CODE CustomerCode,STORE Store,ADD_LINE_1KH CustomerName,MARKET_KHMER_NAME Market,
                            AREA_NAME_KHMER Area FROM SIADD S INNER JOIN 
                            TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID 
                            INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID 
                            WHERE S.DB_CODE = @DB_CODE  AND M.DB_CODE = @DB_CODE  AND A.DB_CODE = @DB_CODE) 
                            Customer
			                ON Customer.CustomerCode = N.CUSTOMER_CODE
                 WHERE
                 P.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND 
                 P.DB_CODE = @DB_CODE AND N.DB_CODE = @DB_CODE ";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate,
            };
            var execute = await _sqlDataAccess.LoadData<PaidInvoiceReportModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
