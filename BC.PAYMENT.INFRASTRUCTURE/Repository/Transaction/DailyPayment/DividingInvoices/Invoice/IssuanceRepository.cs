namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class IssuanceRepository : IIssuanceInvoiceRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public IssuanceRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<IssuanceModel>> GetIssuanceInvoiceAsync(string dbCode, string areaId)
        {
            var sql = $@"SELECT * FROM(
                SELECT A.AREA_ID,N.ID Id,S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,N.TRANSACTION_REF [Transaction],M.MARKET_KHMER_NAME Market,S.STORE Store
                ,A.AREA_NAME_KHMER Area,N.HEADER_TRANSACTION_VALUES [Amount],CASE WHEN N.STATUS = 'N'THEN N'ប៉ុងថ្មី' WHEN N.STATUS = 'C' THEN N'ប៉ុងដូរ' ELSE N'ប៉ុងចាស់' END as 'Type' FROM NEW_INVOICE N LEFT JOIN SIADD S ON S.ADD_CODE  = N.CUSTOMER_CODE 
                INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                WHERE N.DB_CODE = @DB_CODE AND N.CREATED_DATE = CONVERT(DATE, GETDATE()) AND N.IS_DIVIDED = 'TRUE'
                UNION 
                SELECT R.AREA_ID,N.ID, N.CUSTOMER_CODE, N.ACC_NAME_KH, N.TRANSACTION_REF AS 'Code',M.MARKET_KHMER_NAME,Customer.STORE,R.AREA_NAME_KHMER, convert(money, N.HEADER_TRANSACTION_VALUES) as 'Money',
                CASE WHEN N.STATUS = 'N'THEN N'ប៉ុងថ្មី' WHEN N.STATUS = 'C' THEN N'ប៉ុងដូរ' ELSE N'ប៉ុងចាស់' END as 'Type'
                FROM BCMC AS C  INNER JOIN TB_BCMARKET AS M
                ON M.MARKET_ID = C.MARKET_ID INNER JOIN TB_AREAS AS R
                ON R.AREA_ID = M.AREA_ID 
                INNER JOIN NEW_INVOICE AS N ON N.CUSTOMER_CODE = C.ADD_CODE
                INNER JOIN (SELECT ADD_CODE, ADD_LINE_1, STORE
                FROM SIADD WHERE DB_CODE = @DB_CODE) Customer on Customer.ADD_CODE = N.CUSTOMER_CODE
                WHERE 
                N.CREATED_DATE = CONVERT(DATE, GETDATE())
                AND N.IS_DIVIDED = 'TRUE'
                AND N.DB_CODE = @DB_CODE AND R.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE)
                T
                WHERE AREA_ID = @AREA_ID
                ORDER BY [Transaction]";
            var param = new
            {
                DB_CODE = dbCode,
                AREA_ID = areaId,
            };
            var execute = await _sqlDataAccess.LoadData<IssuanceModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<IssuanceModel>> GetIssuanceInvoiceByTransactionAsync(string dbCode, string areaId, string transactionCode)
        {
            var sql = $@"SELECT * FROM(
                SELECT A.AREA_ID,N.ID Id,S.ADD_CODE CustomerCode,S.ADD_LINE_1KH CustomerName,N.TRANSACTION_REF [Transaction],M.MARKET_KHMER_NAME Market,S.STORE Store
                ,A.AREA_NAME_KHMER Area,N.HEADER_TRANSACTION_VALUES [Amount],CASE WHEN N.STATUS = 'N'THEN N'ប៉ុងថ្មី' WHEN N.STATUS = 'C' THEN N'ប៉ុងដូរ' ELSE N'ប៉ុងចាស់' END as 'Type' FROM NEW_INVOICE N LEFT JOIN SIADD S ON S.ADD_CODE  = N.CUSTOMER_CODE 
                INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                WHERE N.DB_CODE = @DB_CODE AND N.CREATED_DATE = CONVERT(DATE, GETDATE()) AND N.IS_DIVIDED = 'TRUE'
                UNION 
                SELECT R.AREA_ID,N.ID, N.CUSTOMER_CODE, N.ACC_NAME_KH, N.TRANSACTION_REF AS 'Code',M.MARKET_KHMER_NAME,Customer.STORE,R.AREA_NAME_KHMER, convert(money, N.HEADER_TRANSACTION_VALUES) as 'Money',
                CASE WHEN N.STATUS = 'N'THEN N'ប៉ុងថ្មី' WHEN N.STATUS = 'C' THEN N'ប៉ុងដូរ' ELSE N'ប៉ុងចាស់' END as 'Type'
                FROM BCMC AS C  INNER JOIN TB_BCMARKET AS M
                ON M.MARKET_ID = C.MARKET_ID INNER JOIN TB_AREAS AS R
                ON R.AREA_ID = M.AREA_ID 
                INNER JOIN NEW_INVOICE AS N ON N.CUSTOMER_CODE = C.ADD_CODE
                INNER JOIN (SELECT ADD_CODE, ADD_LINE_1, STORE
                FROM SIADD WHERE DB_CODE = @DB_CODE) Customer on Customer.ADD_CODE = N.CUSTOMER_CODE
                WHERE 
                N.CREATED_DATE = CONVERT(DATE, GETDATE())
                AND N.IS_DIVIDED = 'TRUE'
                AND N.DB_CODE = @DB_CODE AND R.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE)
                T
                WHERE AREA_ID = @AREA_ID AND [Transaction] NOT IN ({transactionCode})
                ORDER BY [Transaction]";

            var param = new
            {
                DB_CODE = dbCode,
                AREA_ID = areaId,
            };
            var execute = await _sqlDataAccess.LoadData<IssuanceModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
