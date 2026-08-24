namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CommondityExchange.CreditNoteReport
{
    public class CreditNoteReportRepository : ICreditNoteReportRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        public CreditNoteReportRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<CreditNoteReportDto>> GetCreditNoteReportsAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql = $@"SELECT C.CREATED_BY UserCode,C.CREATED_DATE [Date],C.ID Id,M.CUST_CODE CustomerCode,M.TRANS_REF TransactionCode,D.LOCATION [Location],Location.AREA_NAME_KHMER Area,C.OLD_TRANSACTION OldTransactionCode,Location.MARKET_KHMER_NAME Market,Location.CustomerName,M.TRANS_VAL TransactionValue,
                Store,D.ITEM_CODE ItemCode,D.VALUE_1 Quantity,TBCD.DESCRIPION ReasonDesc,
                C.STATUS Status
                FROM TB_BC_CHANGEINVOICE_CREDIT_NOTE C INNER JOIN {dbCode}SISOHDR M ON M.TRANS_REF = C.NEW_TRANSACTION
				INNER JOIN {dbCode}SISODET D ON D.TRANS_REF = M.TRANS_REF
                LEFT JOIN 
                (SELECT C.ADD_CODE CustomerCode,C.ADD_LINE_1KH CustomerName,A.AREA_NAME_KHMER,M.MARKET_KHMER_NAME,STORE Store
                FROM SIADD C LEFT JOIN TB_AREAS A ON A.AREA_ID = C.AREA_ID INNER JOIN TB_BCMARKET M 
                ON M.MARKET_ID = C.MARKET_ID WHERE C.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE)
                Location ON Location.CustomerCode = M.CUST_CODE INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED	 TBCR ON TBCR.ID = C.CHANGEINVOICE_RECEIVED_ID
                INNER JOIN TB_BC_CHANGEINVOICE_DETAIL TBCD ON TBCD.ID = TBCR.CHANGE_INVOICE_DETAIL_ID
                INNER JOIN TB_BC_CHANGEINVOICE TBC ON TBC.ID = TBCD.CHANGE_INVOICE_ID
                WHERE  C.STATUS = 1 AND CONVERT(DATE,C.CREATED_DATE) BETWEEN @FROM_DATE AND @TO_DATE AND C.DB_CODE = @DB_CODE ORDER BY C.CREATED_DATE DESC";
            var param = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate,
            };
            var execute = await _sqlDataAccess.LoadData<CreditNoteReportDto, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
