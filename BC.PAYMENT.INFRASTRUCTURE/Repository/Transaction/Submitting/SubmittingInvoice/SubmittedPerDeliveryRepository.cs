namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.Submitting.SubmittingInvoice
{
    public class SubmittedPerDeliveryRepository : ISubmittedPerDeliveryRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public SubmittedPerDeliveryRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<ExpenseDetailModel>> GetSubmittedInvoicePerDeliveryAsync(string dbCode, string fromDate, string toDate)
        {
            var sql = $@"SELECT D.SUBMITTED_ID Id,DL.DELIVERIES_KHMER DeliveryName,P.CREATED_DATE CreateDate,D.SUBMITTED_BY CreateBy,D.DOLLAR Dollar,D.RIEL Riel,D.TOTAL SubTotal,D.MONEY_BIAS Misaligned,D.EXPENSE_DOLLAR ExpenseDollar,D.EXPENSE_RIEL ExpenseRiel,
            D.TOTAL - D.MONEY_BIAS/D.EXCHANGE-D.EXPENSE_DOLLAR Total,D.EXCHANGE ExchangeRate,N'កំពុង​ដំណើរការ' OTHER,D.SUBMITTED_DATE CreateDate
            FROM BCSUBMITTED_PAID_DETAIL D
            INNER JOIN BCPAYMENTDETAILA P ON
            P.ID = D.PAID_DETAIL_ID
            INNER JOIN TB_BCDELIVERIES DL ON
            DL.DELIVERIES_ID = P.DELIVERYID
            WHERE D.STATUS = '1'
            AND
            DL.DB_CODE = @DB_CODE
            AND
            P.DB_CODE = @DB_CODE
            AND
            D.DB_CODE = @DB_CODE
            AND
            P.CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var execute = await _sqlDataAccess.LoadData<ExpenseDetailModel,dynamic>(sql,param);
            return execute.ToList();
        }
        
        public async Task<List<ExpenseDetailModel>> GetSubmittedInvoicePerDeliveryByPeriodAsync(string dbCode, int month, int year)
        {
            var sql = $@"SELECT D.SUBMITTED_ID Id,DL.DELIVERIES_KHMER DeliveryName,P.CREATED_DATE CreateDate,D.SUBMITTED_BY CreateBy,D.DOLLAR Dollar,D.RIEL Riel,D.TOTAL SubTotal,D.MONEY_BIAS Misaligned,D.EXPENSE_DOLLAR ExpenseDollar,D.EXPENSE_RIEL ExpenseRiel,
            D.TOTAL - D.MONEY_BIAS/D.EXCHANGE-D.EXPENSE_DOLLAR Total,D.EXCHANGE ExchangeRate,N'កំពុង​ដំណើរការ' OTHER,D.SUBMITTED_DATE CreateDate
            FROM BCSUBMITTED_PAID_DETAIL D
            INNER JOIN BCPAYMENTDETAILA P ON
            P.ID = D.PAID_DETAIL_ID
            INNER JOIN TB_BCDELIVERIES DL ON
            DL.DELIVERIES_ID = P.DELIVERYID
            WHERE D.STATUS = '1'
            AND
            DL.DB_CODE = @DB_CODE
            AND
            P.DB_CODE = @DB_CODE
            AND
            D.DB_CODE = @DB_CODE
            AND
            MONTH(P.CREATED_DATE) = @MONTH AND YEAR(P.CREATED_DATE) = @YEAR";
            var param = new
            {
                DB_CODE = dbCode,
                MONTH = month,
                YEAR = year
            };
            var execute = await _sqlDataAccess.LoadData<ExpenseDetailModel,dynamic>(sql,param);
            return execute.ToList();
        }

        public async Task<int> DeleteSubmittedInvoiceAsync(string code)
        {
            const string sql = @"DELETE FROM BCSUBMITTED_PAID_DETAIL WHERE SUBMITTED_ID = @ID";
            var param = new
            {
                ID = code,
            };
            var rowAffected = await _sqlDataAccess.ExecuteAsync(sql, param);
            return rowAffected;
        }
    }
}
