namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.Submitting.SubmittingInvoice;

public class SubmittedRejectedRepository : ISubmittedRejectedInvoiceRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public SubmittedRejectedRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<RejectedInvoiceModel>> GetAllRejectedInvoicesByDateAsync(string dbCode, string fromDate,
        string toDate)
    {
        var sql =
            $@" SELECT S.ID SubmittedInvoiceId,N.ID InvoiceId,D.DIVIDED_INVOICE_ID DividedInvoiceId,DE.DELIVERIES_KHMER DeliveryName,N.CUSTOMER_CODE CustomerCode,ACC_NAME_KH CustomerName,N.TRANSACTION_REF TransactionCode,1.00 AS 'AMOUNT',S.MONEY Money,PAID Paid,HEADER_TRANSACTION_VALUES - PAID Total,AP.APPROVAL_DATE ApprovalDate ,AP.APPROVAL_BY ApprovalBy,AP.DESCRIPTION Description,CASE WHEN AP.APPROVAL_STATUS = 'Approved' THEN N'យល់ព្រម' ELSE N'បដិសេធ' END ApprovalStatus
                   FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID INNER JOIN BCINVOICE_SUMITTED S ON S.INVOICE_ID = N.ID INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
                   INNER JOIN BCAPPROVAL_INVOICE AP ON AP.SUBMITTED_ID = S.ID
                   WHERE  S.SUBMISSION_STATUS = 'Rejected' AND D.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND N.DB_CODE =@Db_CODE
                    AND D.DB_CODE =  @DB_CODE AND S.DB_CODE =  @DB_CODE
                    AND DE.DB_CODE = @DB_CODE  AND AP.DB_CODE = @DB_CODE ";
        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate,
            TO_DATE = toDate
        };
        var execute = await _sqlDataAccess.LoadData<RejectedInvoiceModel, dynamic>(sql, param);
        return execute.ToList();
    }

    public async Task<List<RejectedInvoiceModel>> GetAllRejectedInvoicesByPeriodAsync(string dbCode, int month,
        int year)
    {
        var sql =
            $@"SELECT S.ID SubmittedInvoiceId,N.ID InvoiceId,D.DIVIDED_INVOICE_ID DividedInvoiceId,DE.DELIVERIES_KHMER DeliveryName,N.CUSTOMER_CODE CustomerCode,ACC_NAME_KH CustomerName,N.TRANSACTION_REF TransactionCode,1.00 AS 'AMOUNT',S.MONEY Money,PAID Paid,HEADER_TRANSACTION_VALUES - PAID Total,AP.APPROVAL_DATE ApprovalDate ,AP.APPROVAL_BY ApprovalBy,AP.DESCRIPTION Description,CASE WHEN AP.APPROVAL_STATUS = 'Approved' THEN N'យល់ព្រម' ELSE N'បដិសេធ' END ApprovalStatus
                   FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID INNER JOIN BCINVOICE_SUMITTED S ON S.INVOICE_ID = N.ID INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
                   INNER JOIN BCAPPROVAL_INVOICE AP ON AP.SUBMITTED_ID = S.ID
                   WHERE  S.SUBMISSION_STATUS = 'Rejected' AND YEAR(D.CREATE_DATE) = @YEAR AND MONTH(D.CREATE_DATE) = @MONTH AND N.DB_CODE =@Db_CODE
                    AND D.DB_CODE =  @DB_CODE AND S.DB_CODE =  @DB_CODE
                    AND DE.DB_CODE = @DB_CODE  AND AP.DB_CODE = @DB_CODE ";
        var param = new
        {
            DB_CODE = dbCode,
            MONTH = month,
            YEAR = year
        };
        var execute = await _sqlDataAccess.LoadData<RejectedInvoiceModel, dynamic>(sql, param);
        return execute.ToList();
    }

    public async Task<int> UpdateSubmittedInvoiceFromRejectToCancel(string createBy, string transactionCode,
        string invoiceId)
    {
        var sql =
            @"UPDATE BCINVOICE_SUMITTED SET STATUS = '1', SUBMISSION_STATUS = 'Cancel',UPDATED_DATE = @UPDATED_DATE,UPDATED_BY=@UPDATED_BY WHERE ID = @ID AND TRANSACTION_CODE = @TRANSACTION_CODE";
        var param = new
        {
            ID = invoiceId,
            TRANSACTION_CODE = transactionCode,
            UPDATED_DATE = DateTime.Now,
            UPDATED_BY = createBy
        };
        var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
        return affectedRow;
    }
}