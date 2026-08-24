namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Report.DailyPaymentReport.CreditInvoice
{
    public class CreditInvoiceRepository : ICreditInvoiceRepository

    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public CreditInvoiceRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<CreditInvoiceReportModel>> GetCreditInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql = @"SELECT N.STATUS Status,P.CREATE_DATE CreateDate,
                            D.DELIVERIES_KHMER Delivery,
                            N.CUSTOMER_CODE CustomerCode,
                            N.ACC_NAME_KH CustomerName,
                            Customer.Market,
                            Customer.Area,
			                Store,
                            N.TRANSACTION_REF TransactionCode,
                            CONVERT(BIT,
                            CASE WHEN R.RETURN_ID IS NULL 
                            THEN 0 ELSE 1 END) AS IsReturn
                            ,N.HEADER_TRANSACTION_VALUES InvoiceValue,
                            R.DESCRIPTION Description
                            FROM PC_DIVIDED_INVOICE P LEFT OUTER JOIN PC_RETURN_INVOICE R ON
                            R.DIVIDED_INIOVICE_ID = P.DIVIDED_INVOICE_ID
                            INNER JOIN NEW_INVOICE N ON N.ID = P.INVOICE_ID
                            INNER JOIN TB_BCDELIVERIES D ON D.DELIVERIES_ID = P.DELIVERY_ID
                            LEFT JOIN (SELECT ADD_CODE CustomerCode,STORE Store,ADD_LINE_1KH CustomerName,MARKET_KHMER_NAME Market,
                            AREA_NAME_KHMER Area FROM SIADD S INNER JOIN 
                            TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID 
                            INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID 
                            WHERE S.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE) 
                            Customer ON Customer.CustomerCode = N.CUSTOMER_CODE
                            WHERE P.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND P.DB_CODE = @DB_CODE AND
                            P.DIVIDED_INVOICE_ID NOT IN (SELECT PM.DIVDIE_INVOICE_ID FROM PC_PAYMENT_INVOICE PM)
                            AND N.DB_CODE = @DB_CODE
                            ";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var execute = await _sqlDataAccess.LoadData<CreditInvoiceReportModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
