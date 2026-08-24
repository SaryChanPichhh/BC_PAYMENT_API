namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Report.DailyPaymentReport.SummaryInvoice
{
    public class SummaryInvoiceReportRepository : ISummaryInvoiceRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public SummaryInvoiceReportRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<SummaryInvoiceReportModel>> GetSummaryInvoiceReportsByDateAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql = @$"SELECT PD.CREATE_DATE CreateDate,
                    D.DELIVERIES_NAME DeliveryName,
	                A.AREA_NAME_KHMER AreaNameKhmer,
                    COUNT(CASE WHEN PD.STATUS = '1' THEN 1 END) AS 'CreditInvoice',
                    COUNT(CASE WHEN PM.DIVDIE_INVOICE_ID = PD.DIVIDED_INVOICE_ID THEN 1 END) AS 'PaidInvoice',
                    COUNT(CASE WHEN PR.DIVIDED_INIOVICE_ID = PD.DIVIDED_INVOICE_ID THEN 1 END) AS 'Return',
                    COUNT(CASE WHEN PD.STATUS = '1' THEN 1 END) + COUNT(CASE WHEN PM.DIVDIE_INVOICE_ID = PD.DIVIDED_INVOICE_ID THEN 1 END) + COUNT(CASE WHEN PR.DIVIDED_INIOVICE_ID = PD.DIVIDED_INVOICE_ID THEN 1 END) AS Total
                FROM 
                    PC_DIVIDED_INVOICE PD
                    FULL OUTER JOIN PC_PAYMENT_INVOICE PM ON PM.DIVDIE_INVOICE_ID = PD.DIVIDED_INVOICE_ID 
                    FULL OUTER JOIN PC_RETURN_INVOICE PR ON PR.DIVIDED_INIOVICE_ID = PD.DIVIDED_INVOICE_ID
                    INNER JOIN NEW_INVOICE N ON N.ID = PD.INVOICE_ID 
                    INNER JOIN TB_BCDELIVERIES D ON D.DELIVERIES_ID = PD.DELIVERY_ID
	                INNER JOIN TB_AREA_DETAILS AD ON AD.DELIVERIES_ID = D.DELIVERIES_ID
	                INNER JOIN TB_AREAS A ON A.AREA_ID = AD.AREA_ID
                WHERE 
                    PD.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE
                    AND PD.DB_CODE = @DB_CODE
                GROUP BY 
                    D.DELIVERIES_NAME,A.AREA_NAME_KHMER,PD.CREATE_DATE";
            var param = new
            {
                FROM_DATE = fromDate,
                TO_DATE = toDate,
                DB_CODE = dbCode,
            };
            var execute = await _sqlDataAccess.LoadData<SummaryInvoiceReportModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
