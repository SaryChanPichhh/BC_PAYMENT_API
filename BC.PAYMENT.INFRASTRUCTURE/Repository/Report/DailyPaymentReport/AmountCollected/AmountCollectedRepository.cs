namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Report.DailyPaymentReport.AmountCollected
{
    public class AmountCollectedRepository : IAmountCollectedRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public AmountCollectedRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<ExpenseDetailModel>> GetAmountCollectedReportsByDateAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql = @$"SELECT ID Id,EXCHANGE ExchangeRate,D.DELIVERIES_NAME DeliveryName,TAB1.PAID SubTotal,SUM(DOLLAR) Dollar,SUM(RIEL) Riel,SUM(EXP_AMOUNT_1+EXP_AMOUNT_2+EXP_AMOUNT_3) ExpenseRiel,(SUM(EXP_AMOUNT_1+EXP_AMOUNT_2+EXP_AMOUNT_3)/EXCHANGE)ExpenseDollar,SUM(TOTAL) Total,MONEY_BIAS Misaligned,EXPNSE_DESCRIPTION ExpenseDescription
                    FROM BCPAYMENTDETAILA PD INNER JOIN TB_BCDELIVERIES D ON D.DELIVERIES_ID = PD.DELIVERYID INNER JOIN(SELECT D.DELIVERY_ID, SUM(P.AMOUNT) PAID FROM PC_DIVIDED_INVOICE D 
                    INNER JOIN PC_PAYMENT_INVOICE P ON P.DIVDIE_INVOICE_ID = D.DIVIDED_INVOICE_ID WHERE D.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND P.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE GROUP BY D.DELIVERY_ID) 
                TAB1 ON TAB1.DELIVERY_ID = D.DELIVERIES_ID WHERE PD.CREATED_DATE = @FROM_DATE OR PD.CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE AND PD.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE GROUP BY ID,EXCHANGE,D.DELIVERIES_NAME,MONEY_BIAS,EXPNSE_DESCRIPTION,TAB1.PAID";

            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate,
            };
            var execute = await _sqlDataAccess.LoadData<ExpenseDetailModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
