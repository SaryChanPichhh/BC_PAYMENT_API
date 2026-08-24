namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.Submitting.SubmittingInvoice
{
    public class SubmittedInvoiceRepository : ISubmittedInvoiceRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public SubmittedInvoiceRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<SubmittedInvoiceModel>> GetAllNotSubmitPaidInvoice(string dbCode, string fromDate, string toDate)
        {
            var sql = $@"SELECT S.ID,DE.DELIVERIES_KHMER,N.CUSTOMER_CODE,ACC_NAME_KH,N.TRANSACTION_REF,1.00 AS 'AMOUNT',S.MONEY,PAID,HEADER_TRANSACTION_VALUES - PAID TOTAL,SUBMITTED_DATE,SUBMITTED_BY,N'កំពុងដំណើរការ' STATUS,
            CONVERT(bit,CASE WHEN S.MONEY = PAID THEN 1 ELSE 0 END) [FULL_PAID]
            FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID INNER JOIN BCINVOICE_SUMITTED S ON S.INVOICE_ID = N.ID
            INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
            WHERE D.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND N.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE AND S.DB_CODE = @DB_CODE
            AND DE.DB_CODE = @DB_CODE AND S.STATUS = '1' AND S.SUBMISSION_STATUS != 'Cancel'
            GROUP BY N.ID,S.ID,DE.DELIVERIES_KHMER,N.CUSTOMER_CODE,ACC_NAME_KH,N.TRANSACTION_REF,S.MONEY,PAID,HEADER_TRANSACTION_VALUES - PAID,SUBMITTED_DATE,SUBMITTED_BY";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var execute = await _sqlDataAccess.LoadData<SubmittedInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> UpdateInvoiceFromPendingToCancelAsync(string dbCode,string createBy,string submittedId)
        {
            var sql = $@"UPDATE BCINVOICE_SUMITTED SET SUBMISSION_STATUS = 'Cancel',UPDATED_BY = @UPDATED_BY,UPDATED_DATE = @UPDATED_DATE WHERE ID = @ID AND DB_CODE = @DB_CODE";
            var param = new
            {
                ID = submittedId,
                DB_CODE = dbCode,
                UPDATED_BY = createBy,
                UPDATED_DATE = DateTime.Now
            };

            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }
    }
}
