namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.DividingInvoices.DailyReport
{
    public class CheckReturnInvoiceRepository(ISqlDataAccess sqlDataAccess) : ICheckReturnInvoiceRepository
    {
        public async Task<List<PaymentInvoiceModel>> GetAllNewAndChangeDividedInvoiceByDate(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql =
                $@"SELECT TRANSACTION_REF TransactionCode, CUSTOMER_CODE CustomerCode, ACC_NAME_KH CustomerName,ISNULL(Market,'-') Market,
                ISNULL(CASE WHEN N.STATUS = 'N' THEN '1' END, '-') NewInvoice,
                ISNULL(CASE WHEN N.STATUS = 'C' THEN '1' END, '-') ChangeInvoice,
                N.HEADER_TRANSACTION_VALUES InvoiceAmount,
                DIVIDED_INVOICE_ID DividedInvoiceId
                FROM NEW_INVOICE N
                INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
                LEFT JOIN (SELECT ADD_CODE,STORE +' '+ MARKET_KHMER_NAME Market FROM SIADD SI INNER JOIN TB_BCMARKET M ON M.MARKET_ID = SI.MARKET_ID
                WHERE SI.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE) TAB ON TAB.ADD_CODE = N.CUSTOMER_CODE
                WHERE D.STATUS = '1' AND N.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE
                AND D.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND N.STATUS IN ('C','N');";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate
            };
            var execute = await sqlDataAccess.LoadData<PaymentInvoiceModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
