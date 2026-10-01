namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.Submitting.SubmittingInvoice;

public class SubmittedRejectedPerDeliveryRepository : ISubmittedRejectedInvoicePerDeliveryRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public SubmittedRejectedPerDeliveryRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<RejectedInvoicePerDelivery>> GetAllRejectedInvoicePerDeliveryByDateAsync(string dbCode,
        string fromDate, string toDate)
    {
        var sql =
            $@"SELECT DL.DELIVERIES_KHMER DeliveryName,P.CREATED_DATE CreateDate,D.SUBMITTED_BY CreateBy,D.DOLLAR Dollar,D.RIEL Riel,D.TOTAL Total,D.MONEY_BIAS Misaligned,D.EXPENSE_DOLLAR ExpenseDollar,D.EXPENSE_RIEL ExpenseRiel,
                D.TOTAL - D.MONEY_BIAS/D.EXCHANGE-D.EXPENSE_DOLLAR Total,  D.EXCHANGE ExchangeRate,A.APPROVAL_STATUS ApprovalStatus,A.DESCRIPTION ExpenseDescription,A.APPROVAL_DATE ApprovalDate,A.APPROVAL_BY ApprovalBy
                FROM BCSUBMITTED_PAID_DETAIL D
                INNER JOIN BCPAYMENTDETAILA P ON
                P.ID = D.PAID_DETAIL_ID
                 INNER JOIN BCAPPROVAL_PAID_DETAIL A ON
                A.SUBMITTED_ID = D.SUBMITTED_ID
                INNER JOIN TB_BCDELIVERIES DL ON
                 DL.DELIVERIES_ID = P.DELIVERYID
                 WHERE D.DB_CODE = @DB_CODE AND P.DB_CODE = @DB_CODE AND A.DB_CODE =  @DB_CODE AND DL.DB_CODE = @DB_CODE AND
                 P.CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE";
        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate,
            YTO_DATEEAR = toDate
        };
        var execute = await _sqlDataAccess.LoadData<RejectedInvoicePerDelivery, dynamic>(sql, param);
        return execute.ToList();
    }

    public async Task<List<RejectedInvoicePerDelivery>> GetAllRejectedInvoicePerDeliveryByPeriodAsync(string dbCode,
        int month, int year)
    {
        var sql =
            $@"SELECT DL.DELIVERIES_KHMER DeliveryName,P.CREATED_DATE CreateDate,D.SUBMITTED_BY CreateBy,D.DOLLAR Dollar,D.RIEL Riel,D.TOTAL Total,D.MONEY_BIAS Misaligned,D.EXPENSE_DOLLAR ExpenseDollar,D.EXPENSE_RIEL ExpenseRiel,
                D.TOTAL - D.MONEY_BIAS/D.EXCHANGE-D.EXPENSE_DOLLAR Total,  D.EXCHANGE ExchangeRate,A.APPROVAL_STATUS ApprovalStatus,A.DESCRIPTION ExpenseDescription,A.APPROVAL_DATE ApprovalDate,A.APPROVAL_BY ApprovalBy
                FROM BCSUBMITTED_PAID_DETAIL D
                INNER JOIN BCPAYMENTDETAILA P ON
                P.ID = D.PAID_DETAIL_ID
                 INNER JOIN BCAPPROVAL_PAID_DETAIL A ON
                A.SUBMITTED_ID = D.SUBMITTED_ID
                INNER JOIN TB_BCDELIVERIES DL ON
                 DL.DELIVERIES_ID = P.DELIVERYID
                 WHERE D.DB_CODE = @DB_CODE AND P.DB_CODE = @DB_CODE AND A.DB_CODE =  @DB_CODE AND DL.DB_CODE = @DB_CODE AND
                 MONTH(P.CREATED_DATE) =@MONTH AND YEAR(P.CREATED_DATE) = @YEAR";
        var param = new
        {
            DB_CODE = dbCode,
            MONTH = month,
            YEAR = year
        };
        var execute = await _sqlDataAccess.LoadData<RejectedInvoicePerDelivery, dynamic>(sql, param);
        return execute.ToList();
    }
}