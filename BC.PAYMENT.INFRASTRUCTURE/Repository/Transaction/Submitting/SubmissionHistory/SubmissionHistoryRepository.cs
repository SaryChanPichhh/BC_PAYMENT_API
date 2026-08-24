namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.Submitting.HistoryApproval
{
    public class SubmissionHistoryRepository : ISubmissionHistoryRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public SubmissionHistoryRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public Task<List<SubmissionHistoryModel>> GetHistoryApprovalByPeriodAsync(string dbCode, int month, int year)
        {

            throw new NotImplementedException();
        }

        public async Task<List<SubmissionHistoryModel>> GetHistoryApprovalByDateAsync(string dbCode, string fromDate, string toDate)
        {
            var sql = $@"SELECT DL.DELIVERIES_KHMER DeliveryName,P.CREATED_DATE CreateDate,D.SUBMITTED_BY CreateBy,D.DOLLAR Dollar,D.RIEL Riel,D.TOTAL Total,D.MONEY_BIAS Misaligned,
                D.EXPENSE_DOLLAR ExpenseDollar,D.EXPENSE_RIEL ExpenseRiel,D.TOTAL - D.MONEY_BIAS/D.EXCHANGE-D.EXPENSE_DOLLAR SubTotal,D.EXCHANGE ExchangeRate,
                A.APPROVAL_STATUS ApprovalStatus,A.DESCRIPTION Description,A.APPROVAL_DATE ApprovalDate,A.APPROVAL_BY ApprovalBy
                FROM BCSUBMITTED_PAID_DETAIL D
                INNER JOIN BCPAYMENTDETAILA P ON P.ID = D.PAID_DETAIL_ID
                INNER JOIN BCAPPROVAL_PAID_DETAIL A ON A.SUBMITTED_ID = D.SUBMITTED_ID
                INNER JOIN TB_BCDELIVERIES DL ON DL.DELIVERIES_ID = P.DELIVERYID  WHERE
                D.DB_CODE = @DB_CODE AND P.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND
                P.CREATED_DATE = @FROM_DATE OR P.CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE ";

            var param = new  {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var execute = await _sqlDataAccess.LoadData<SubmissionHistoryModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
