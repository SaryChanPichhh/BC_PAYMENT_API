namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Report.DailyPaymentReport.ExpenseInvoice
{
    public class ExpenseInvoiceReportRepository : IExpenseInvoiceReportRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ExpenseInvoiceReportRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        public async Task<List<ExpenseInvoiceReportModel>> GetExpenseInvoiceReportsByDateAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql =
                $@"SELECT PD.CREATED_DATE CreateDate,D.DELIVERIES_NAME DeliveryName,(PD.DESC_EXP_1 + ' ' + PD.DESC_EXP_2+ ' ' + PD.DESC_EXP_3) Description,SUM(PD.EXP_AMOUNT_1+PD.EXP_AMOUNT_2+PD.EXP_AMOUNT_3)Amount,PD.EXPNSE_DESCRIPTION ExpenseDescription 
                FROM BCPAYMENTDETAILA PD INNER JOIN TB_BCDELIVERIES D ON D.DELIVERIES_ID = PD.DELIVERYID 
                WHERE PD.CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE AND PD.DB_CODE = @DB_CODE
                GROUP BY D.DELIVERIES_NAME, PD.DESC_EXP_1 + ' ' + PD.DESC_EXP_2 + ' ' + PD.DESC_EXP_3, PD.EXPNSE_DESCRIPTION,PD.CREATED_DATE;";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate,
            };
            var execute = await _sqlDataAccess.LoadData<ExpenseInvoiceReportModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
