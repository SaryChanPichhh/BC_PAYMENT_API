namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Preset.OwedInvoice;

public class OwedInvoiceRepository : IOwedInvoiceRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public OwedInvoiceRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<SummaryAccountsReceivableModel>> GetAccountReceivableSummaries(
        Dictionary<string, string> dbCodes, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;
        var offSet = (page - 1) * pageSize;
        List<SummaryAccountsReceivableModel> accountsReceivableModels = new();
        foreach (var dbCode in dbCodes)
        {
            var sql = @$"IF @T=''
            BEGIN
                SELECT N'{dbCode.Value}' Code,TAB.Reference TransactionCode, TAB.[Account Code] CustomerCode, CUS.[CustomerName] CustomerName, CUS.Area, CUS.Market, CUS.Store, TAB.Amount_Total InvoiceValue, [Analysis T0] AnalysisT0, U.username,
                    [Transaction Date] TransactionDate
                FROM (
            SELECT ACC_PERIOD AS [Account Period], ACC_CODE AS [Account Code],
                        (SELECT ACC_NAME
                        FROM SIACCNT
                        WHERE DB_CODE = @DB_CODE AND ACC_CODE = {dbCode.Key}SILEDG.ACC_CODE) AS [Account Name], (SELECT ACC_COM1
                        FROM SIACCNT
                        WHERE DB_CODE = @DB_CODE AND ACC_CODE = {dbCode.Key}SILEDG.ACC_CODE) AS [ACC_KHMER],
                        (SELECT ACC_TYPE
                        FROM SIACCNT
                        WHERE DB_CODE = @DB_CODE AND ACC_CODE = {dbCode.Key}SILEDG.ACC_CODE) AS [Account Type],
                        TRANS_DATE AS [Transaction Date], JRNAL_NO AS [Journal Number], JRNAL_LINE AS [Journal Line], --@BY_DATE [By Date],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'C', 0,0,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Current], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 0,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_0-30],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 1,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_1-30], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 31,60,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_31-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 61,90,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_61-90], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 91,120,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_91-120],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,30)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-30], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,60)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,90)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-90], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,120)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-120],
                        AMOUNT*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Total], JRNAL_TYPE AS [Journal Type], REFERENCE AS Reference, DESCRIPTN AS Description, ENTRY_DATE AS [Entry Date], ENTRY_PRD AS [Entry Period], DUE_DATE AS [Due Date],
                        (CASE WHEN DUE_DATE<>'' THEN DATEDIFF(DAY,DUE_DATE,@BY_DATE) ELSE 0 END) [Due Date Amount],
                        ASSET_CODE AS [Asset Code], ASSET_UPDT AS [Asset Update], CONV_CODE AS [Conversion Code], CONV_SIGN AS [Conversion Sign], CONV_RATE AS [Conversion Rate],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'C', 0,0,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Current], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 0,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_0-30],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 1,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_1-30], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 31,60,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_31-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 61,90,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_61-90], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 91,120,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_91-120],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,30)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-30], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,60)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,90)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-90], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,120)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-120],
                        OTHER_AMT*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Total],
                        ANAL_T0 [Analysis T0], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T0' AND ANAD_CODE=ANAL_T0) [Analysis T0 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T0' AND ANAD_CODE=ANAL_T0) [Analysis T0 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T0' AND ANAD_CODE=ANAL_T0) [Analysis T0 Comments],
                        ANAL_T1 [Analysis T1], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T1' AND ANAD_CODE=ANAL_T1) [Analysis T1 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T1' AND ANAD_CODE=ANAL_T1) [Analysis T1 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T1' AND ANAD_CODE=ANAL_T1) [Analysis T1 Comments],
                        ANAL_T2 [Analysis T2], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T2' AND ANAD_CODE=ANAL_T2) [Analysis T2 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T2' AND ANAD_CODE=ANAL_T2) [Analysis T2 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T2' AND ANAD_CODE=ANAL_T2) [Analysis T2 Comments],
                        ANAL_T3 [Analysis T3], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T3' AND ANAD_CODE=ANAL_T3) [Analysis T3 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T3' AND ANAD_CODE=ANAL_T3) [Analysis T3 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T3' AND ANAD_CODE=ANAL_T3) [Analysis T3 Comments],
                        ANAL_T4 [Analysis T4], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T4' AND ANAD_CODE=ANAL_T4) [Analysis T4 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T4' AND ANAD_CODE=ANAL_T4) [Analysis T4 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T4' AND ANAD_CODE=ANAL_T4) [Analysis T4 Comments],
                        ANAL_T5 [Analysis T5], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T5' AND ANAD_CODE=ANAL_T5) [Analysis T5 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T5' AND ANAD_CODE=ANAL_T5) [Analysis T5 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T5' AND ANAD_CODE=ANAL_T5) [Analysis T5 Comments],
                        ANAL_T6 [Analysis T6], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T6' AND ANAD_CODE=ANAL_T6) [Analysis T6 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T6' AND ANAD_CODE=ANAL_T6) [Analysis T6 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T6' AND ANAD_CODE=ANAL_T6) [Analysis T6 Comments],
                        ANAL_T7 [Analysis T7], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T7' AND ANAD_CODE=ANAL_T7) [Analysis T7 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T7' AND ANAD_CODE=ANAL_T7) [Analysis T7 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T7' AND ANAD_CODE=ANAL_T7) [Analysis T7 Comments],
                        ANAL_T8 [Analysis T8], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T8' AND ANAD_CODE=ANAL_T8) [Analysis T8 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T8' AND ANAD_CODE=ANAL_T8) [Analysis T8 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T8' AND ANAD_CODE=ANAL_T8) [Analysis T8 Comments],
                        ANAL_T9 [Analysis T9], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T9' AND ANAD_CODE=ANAL_T9) [Analysis T9 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T9' AND ANAD_CODE=ANAL_T9) [Analysis T9 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T9' AND ANAD_CODE=ANAL_T9) [Analysis T9 Comments],
                        TRAN_DESC1 AS Description1, TRAN_DESC2 AS Description2, TRAN_DESC3 AS Description3,
                        TRAN_DESC4 AS Description4, TRAN_DESC5 AS Description5, TRAN_DESC6 AS Description6, HOLD_REF AS [Held Reference], ALLOCATION AS [Allocation Status]
                    FROM dbo.{dbCode.Key}SILEDG
                    WHERE  ALLOCATION<>'A' AND ACC_CODE IN (SELECT ACC_CODE
                        FROM SIACCNT
                        WHERE DB_CODE=@DB_CODE AND ACC_TYPE=@ACC_TYPE) AND ANAL_T0 LIKE @T0 AND ANAL_T1 LIKE @T1 AND ANAL_T2 LIKE @T2 AND ANAL_T3 LIKE @T3 AND ANAL_T4 LIKE @T4 AND ANAL_T5 LIKE @T5 AND ANAL_T6 LIKE @T6 AND ANAL_T7 LIKE @T7 AND ANAL_T8 LIKE @T8 AND ANAL_T9 LIKE @T9
            ) TAB LEFT JOIN (SELECT UPPER(LEFT(USER_NAME,1))+LOWER(SUBSTRING(USER_NAME,2,LEN(USER_NAME))) username, USER_ID
                    FROM BCUSERS) U ON CONVERT(nvarchar,U.USER_ID)  =  TAB.[Analysis T0]
                    LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = TAB.[Account Code]
                WHERE [Amount_Total] >= 0 ORDER BY TransactionDate OFFSET {offSet} ROWS FETCH NEXT {pageSize} ROWS ONLY
            END
            ELSE
            BEGIN
                SELECT TAB.Reference TransactionCode, TAB.[Account Code] CustomerCode, TAB.[Account Name] CustomerName, TAB.Amount_Total InvoiceValue, [Analysis T0] AnalysisT0, U.username
                FROM (
             SELECT ACC_PERIOD AS [Account Period], ACC_CODE AS [Account Code],
                        (SELECT ACC_NAME
                        FROM SIACCNT
                        WHERE DB_CODE = @DB_CODE AND ACC_CODE = {dbCode.Key}SILEDG.ACC_CODE) AS [Account Name],
                        (SELECT ACC_TYPE
                        FROM SIACCNT
                        WHERE DB_CODE = @DB_CODE AND ACC_CODE = {dbCode.Key}SILEDG.ACC_CODE) AS [Account Type],
                        TRANS_DATE AS [Transaction Date], JRNAL_NO AS [Journal Number], JRNAL_LINE AS [Journal Line], --@BY_DATE [By Date],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'C', 0,0,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Current], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 0,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_0-30],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 1,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_1-30], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 31,60,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_31-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 61,90,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_61-90], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 91,120,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_91-120],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,30)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-30], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,60)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,90)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-90], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,120)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-120],
                        AMOUNT*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Total], JRNAL_TYPE AS [Journal Type], REFERENCE AS Reference, DESCRIPTN AS Description, ENTRY_DATE AS [Entry Date], ENTRY_PRD AS [Entry Period],
                        DUE_DATE AS [Due Date], (CASE WHEN DUE_DATE<>'' THEN DATEDIFF(DAY,DUE_DATE,@BY_DATE) ELSE 0 END) [Due Date Amount], ASSET_CODE AS [Asset Code], ASSET_UPDT AS [Asset Update], CONV_CODE AS [Conversion Code], CONV_SIGN AS [Conversion Sign], CONV_RATE AS [Conversion Rate],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'C', 0,0,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Current], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 0,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_0-30],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 1,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_1-30], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 31,60,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_31-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 61,90,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_61-90], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 91,120,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_91-120],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,30)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-30], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,60)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,90)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-90], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,120)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-120],
                        OTHER_AMT*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Total],
                        ANAL_T0 [Analysis T0], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T0' AND ANAD_CODE=ANAL_T0) [Analysis T0 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T0' AND ANAD_CODE=ANAL_T0) [Analysis T0 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T0' AND ANAD_CODE=ANAL_T0) [Analysis T0 Comments],
                        ANAL_T1 [Analysis T1], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T1' AND ANAD_CODE=ANAL_T0) [Analysis T1 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T1' AND ANAD_CODE=ANAL_T0) [Analysis T1 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T1' AND ANAD_CODE=ANAL_T0) [Analysis T1 Comments],
                        ANAL_T2 [Analysis T2], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T2' AND ANAD_CODE=ANAL_T0) [Analysis T2 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T2' AND ANAD_CODE=ANAL_T0) [Analysis T2 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T2' AND ANAD_CODE=ANAL_T0) [Analysis T2 Comments],
                        ANAL_T3 [Analysis T3], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T3' AND ANAD_CODE=ANAL_T0) [Analysis T3 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T3' AND ANAD_CODE=ANAL_T0) [Analysis T3 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T3' AND ANAD_CODE=ANAL_T0) [Analysis T3 Comments],
                        ANAL_T4 [Analysis T4], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T4' AND ANAD_CODE=ANAL_T0) [Analysis T4 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T4' AND ANAD_CODE=ANAL_T0) [Analysis T4 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T4' AND ANAD_CODE=ANAL_T0) [Analysis T4 Comments],
                        ANAL_T5 [Analysis T5], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T5' AND ANAD_CODE=ANAL_T0) [Analysis T5 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T5' AND ANAD_CODE=ANAL_T0) [Analysis T5 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T5' AND ANAD_CODE=ANAL_T0) [Analysis T5 Comments],
                        ANAL_T6 [Analysis T6], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T6' AND ANAD_CODE=ANAL_T0) [Analysis T6 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T6' AND ANAD_CODE=ANAL_T0) [Analysis T6 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T6' AND ANAD_CODE=ANAL_T0) [Analysis T6 Comments],
                        ANAL_T7 [Analysis T7], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T7' AND ANAD_CODE=ANAL_T0) [Analysis T7 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T7' AND ANAD_CODE=ANAL_T0) [Analysis T7 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T7' AND ANAD_CODE=ANAL_T0) [Analysis T7 Comments],
                        ANAL_T8 [Analysis T8], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T8' AND ANAD_CODE=ANAL_T0) [Analysis T8 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T8' AND ANAD_CODE=ANAL_T0) [Analysis T8 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T8' AND ANAD_CODE=ANAL_T0) [Analysis T8 Comments],
                        ANAL_T9 [Analysis T9], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T9' AND ANAD_CODE=ANAL_T0) [Analysis T9 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T9' AND ANAD_CODE=ANAL_T0) [Analysis T9 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T9' AND ANAD_CODE=ANAL_T0) [Analysis T9 Comments],
                        TRAN_DESC1 AS Description1, TRAN_DESC2 AS Description2, TRAN_DESC3 AS Description3,
                        TRAN_DESC4 AS Description4, TRAN_DESC5 AS Description5, TRAN_DESC6 AS Description6, HOLD_REF AS [Held Reference], ALLOCATION AS [Allocation Status]
                    FROM dbo.{dbCode.Key}SILEDG
                    WHERE  ALLOCATION<>'A' AND ACC_CODE IN (SELECT ACC_CODE
                        FROM SIACCNT
                        WHERE DB_CODE=@DB_CODE AND ACC_TYPE=@ACC_TYPE) AND (CASE WHEN @T='T0' THEN ANAL_T0 WHEN @T='T1' THEN ANAL_T1 WHEN @T='T2' THEN ANAL_T2 WHEN @T='T3' THEN ANAL_T3 WHEN @T='T4' THEN ANAL_T4 WHEN @T='T5' THEN ANAL_T5 WHEN @T='T6' THEN ANAL_T6 WHEN @T='T7' THEN ANAL_T7 WHEN @T='T8' THEN ANAL_T8 ELSE ANAL_T9 END) BETWEEN @FROM_ANAL AND @TO_ANAL
             ) TAB LEFT JOIN (SELECT UPPER(LEFT(USER_NAME,1))+LOWER(SUBSTRING(USER_NAME,2,LEN(USER_NAME))) username, USER_ID
                    FROM BCUSERS) U ON CONVERT(nvarchar,U.USER_ID)  =  TAB.[Analysis T0]
                    LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = TAB.[Account Code]
                WHERE [Amount_Total] >= 0 
            END";
            var param = new
            {
                DB_CODE = dbCode.Key,
                BY_DATE = "",
                ACC_TYPE = "D",
                T = "",
                FROM_ANAL = "",
                TO_ANAL = "",
                T0 = "%",
                T1 = "%",
                T2 = "%",
                T3 = "%",
                T4 = "%",
                T5 = "%",
                T6 = "%",
                T7 = "%",
                T8 = "%",
                T9 = "%"
            };
            var results = (await _sqlDataAccess.LoadData<SummaryAccountsReceivableModel, dynamic>(sql, param)).ToList();
            if (results.Count > 0) accountsReceivableModels.AddRange(results);
        }

        return accountsReceivableModels;
    }

    public async Task<List<OwedInvoiceDto>> GetAccountReceivableAmount(Dictionary<string, string> dbCodes)
    {
        var owedInvoice = new List<OwedInvoiceDto>();
        foreach (var dbCode in dbCodes)
        {
            var sql = @$"IF @T=''
            BEGIN
                SELECT SUM(TAB.Amount_Total) Amount
                FROM (
            SELECT ACC_PERIOD AS [Account Period], ACC_CODE AS [Account Code],
                        (SELECT ACC_NAME
                        FROM SIACCNT
                        WHERE DB_CODE = @DB_CODE AND ACC_CODE = {dbCode.Key}SILEDG.ACC_CODE) AS [Account Name], (SELECT ACC_COM1
                        FROM SIACCNT
                        WHERE DB_CODE = @DB_CODE AND ACC_CODE = {dbCode.Key}SILEDG.ACC_CODE) AS [ACC_KHMER],
                        (SELECT ACC_TYPE
                        FROM SIACCNT
                        WHERE DB_CODE = @DB_CODE AND ACC_CODE = {dbCode.Key}SILEDG.ACC_CODE) AS [Account Type],
                        TRANS_DATE AS [Transaction Date], JRNAL_NO AS [Journal Number], JRNAL_LINE AS [Journal Line], --@BY_DATE [By Date],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'C', 0,0,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Current], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 0,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_0-30],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 1,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_1-30], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 31,60,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_31-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 61,90,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_61-90], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 91,120,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_91-120],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,30)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-30], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,60)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,90)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-90], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,120)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-120],
                        AMOUNT*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Total], JRNAL_TYPE AS [Journal Type], REFERENCE AS Reference, DESCRIPTN AS Description, ENTRY_DATE AS [Entry Date], ENTRY_PRD AS [Entry Period], DUE_DATE AS [Due Date],
                        (CASE WHEN DUE_DATE<>'' THEN DATEDIFF(DAY,DUE_DATE,@BY_DATE) ELSE 0 END) [Due Date Amount],
                        ASSET_CODE AS [Asset Code], ASSET_UPDT AS [Asset Update], CONV_CODE AS [Conversion Code], CONV_SIGN AS [Conversion Sign], CONV_RATE AS [Conversion Rate],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'C', 0,0,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Current], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 0,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_0-30],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 1,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_1-30], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 31,60,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_31-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 61,90,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_61-90], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 91,120,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_91-120],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,30)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-30], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,60)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,90)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-90], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,120)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-120],
                        OTHER_AMT*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Total],
                        ANAL_T0 [Analysis T0], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T0' AND ANAD_CODE=ANAL_T0) [Analysis T0 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T0' AND ANAD_CODE=ANAL_T0) [Analysis T0 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T0' AND ANAD_CODE=ANAL_T0) [Analysis T0 Comments],
                        ANAL_T1 [Analysis T1], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T1' AND ANAD_CODE=ANAL_T1) [Analysis T1 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T1' AND ANAD_CODE=ANAL_T1) [Analysis T1 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T1' AND ANAD_CODE=ANAL_T1) [Analysis T1 Comments],
                        ANAL_T2 [Analysis T2], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T2' AND ANAD_CODE=ANAL_T2) [Analysis T2 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T2' AND ANAD_CODE=ANAL_T2) [Analysis T2 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T2' AND ANAD_CODE=ANAL_T2) [Analysis T2 Comments],
                        ANAL_T3 [Analysis T3], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T3' AND ANAD_CODE=ANAL_T3) [Analysis T3 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T3' AND ANAD_CODE=ANAL_T3) [Analysis T3 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T3' AND ANAD_CODE=ANAL_T3) [Analysis T3 Comments],
                        ANAL_T4 [Analysis T4], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T4' AND ANAD_CODE=ANAL_T4) [Analysis T4 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T4' AND ANAD_CODE=ANAL_T4) [Analysis T4 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T4' AND ANAD_CODE=ANAL_T4) [Analysis T4 Comments],
                        ANAL_T5 [Analysis T5], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T5' AND ANAD_CODE=ANAL_T5) [Analysis T5 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T5' AND ANAD_CODE=ANAL_T5) [Analysis T5 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T5' AND ANAD_CODE=ANAL_T5) [Analysis T5 Comments],
                        ANAL_T6 [Analysis T6], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T6' AND ANAD_CODE=ANAL_T6) [Analysis T6 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T6' AND ANAD_CODE=ANAL_T6) [Analysis T6 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T6' AND ANAD_CODE=ANAL_T6) [Analysis T6 Comments],
                        ANAL_T7 [Analysis T7], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T7' AND ANAD_CODE=ANAL_T7) [Analysis T7 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T7' AND ANAD_CODE=ANAL_T7) [Analysis T7 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T7' AND ANAD_CODE=ANAL_T7) [Analysis T7 Comments],
                        ANAL_T8 [Analysis T8], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T8' AND ANAD_CODE=ANAL_T8) [Analysis T8 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T8' AND ANAD_CODE=ANAL_T8) [Analysis T8 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T8' AND ANAD_CODE=ANAL_T8) [Analysis T8 Comments],
                        ANAL_T9 [Analysis T9], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T9' AND ANAD_CODE=ANAL_T9) [Analysis T9 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T9' AND ANAD_CODE=ANAL_T9) [Analysis T9 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T9' AND ANAD_CODE=ANAL_T9) [Analysis T9 Comments],
                        TRAN_DESC1 AS Description1, TRAN_DESC2 AS Description2, TRAN_DESC3 AS Description3,
                        TRAN_DESC4 AS Description4, TRAN_DESC5 AS Description5, TRAN_DESC6 AS Description6, HOLD_REF AS [Held Reference], ALLOCATION AS [Allocation Status]
                    FROM dbo.{dbCode.Key}SILEDG
                    WHERE  ALLOCATION<>'A' AND ACC_CODE IN (SELECT ACC_CODE
                        FROM SIACCNT
                        WHERE DB_CODE=@DB_CODE AND ACC_TYPE=@ACC_TYPE) AND ANAL_T0 LIKE @T0 AND ANAL_T1 LIKE @T1 AND ANAL_T2 LIKE @T2 AND ANAL_T3 LIKE @T3 AND ANAL_T4 LIKE @T4 AND ANAL_T5 LIKE @T5 AND ANAL_T6 LIKE @T6 AND ANAL_T7 LIKE @T7 AND ANAL_T8 LIKE @T8 AND ANAL_T9 LIKE @T9
            ) TAB LEFT JOIN (SELECT UPPER(LEFT(USER_NAME,1))+LOWER(SUBSTRING(USER_NAME,2,LEN(USER_NAME))) username, USER_ID
                    FROM BCUSERS) U ON CONVERT(nvarchar,U.USER_ID)  =  TAB.[Analysis T0]
                    LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = TAB.[Account Code]
                WHERE [Amount_Total] >= 0
            END
            ELSE
            BEGIN
                SELECT TAB.Reference TransactionCode, TAB.[Account Code] CustomerCode, TAB.[Account Name] CustomerName, TAB.Amount_Total InvoiceValue, [Analysis T0] AnalysisT0, U.username
                FROM (
             SELECT ACC_PERIOD AS [Account Period], ACC_CODE AS [Account Code],
                        (SELECT ACC_NAME
                        FROM SIACCNT
                        WHERE DB_CODE = @DB_CODE AND ACC_CODE = {dbCode.Key}SILEDG.ACC_CODE) AS [Account Name],
                        (SELECT ACC_TYPE
                        FROM SIACCNT
                        WHERE DB_CODE = @DB_CODE AND ACC_CODE = {dbCode.Key}SILEDG.ACC_CODE) AS [Account Type],
                        TRANS_DATE AS [Transaction Date], JRNAL_NO AS [Journal Number], JRNAL_LINE AS [Journal Line], --@BY_DATE [By Date],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'C', 0,0,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Current], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 0,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_0-30],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 1,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_1-30], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 31,60,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_31-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 61,90,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_61-90], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 91,120,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_91-120],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,30)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-30], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,60)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,90)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-90], dbo.{dbCode.Key}_AGED_AMOUNT('B', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,120)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Over-120],
                        AMOUNT*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Amount_Total], JRNAL_TYPE AS [Journal Type], REFERENCE AS Reference, DESCRIPTN AS Description, ENTRY_DATE AS [Entry Date], ENTRY_PRD AS [Entry Period],
                        DUE_DATE AS [Due Date], (CASE WHEN DUE_DATE<>'' THEN DATEDIFF(DAY,DUE_DATE,@BY_DATE) ELSE 0 END) [Due Date Amount], ASSET_CODE AS [Asset Code], ASSET_UPDT AS [Asset Update], CONV_CODE AS [Conversion Code], CONV_SIGN AS [Conversion Sign], CONV_RATE AS [Conversion Rate],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'C', 0,0,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Current], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 0,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_0-30],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 1,30,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_1-30], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 31,60,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_31-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 61,90,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_61-90], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'R', 91,120,0)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_91-120],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,30)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-30], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,60)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-60],
                        dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,90)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-90], dbo.{dbCode.Key}_AGED_AMOUNT('O', JRNAL_NO, JRNAL_LINE, @BY_DATE,'O', 0,0,120)*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Over-120],
                        OTHER_AMT*(CASE WHEN @ACC_TYPE='C' THEN -1 ELSE 1 END) AS [Other Amount_Total],
                        ANAL_T0 [Analysis T0], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T0' AND ANAD_CODE=ANAL_T0) [Analysis T0 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T0' AND ANAD_CODE=ANAL_T0) [Analysis T0 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T0' AND ANAD_CODE=ANAL_T0) [Analysis T0 Comments],
                        ANAL_T1 [Analysis T1], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T1' AND ANAD_CODE=ANAL_T0) [Analysis T1 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T1' AND ANAD_CODE=ANAL_T0) [Analysis T1 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T1' AND ANAD_CODE=ANAL_T0) [Analysis T1 Comments],
                        ANAL_T2 [Analysis T2], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T2' AND ANAD_CODE=ANAL_T0) [Analysis T2 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T2' AND ANAD_CODE=ANAL_T0) [Analysis T2 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T2' AND ANAD_CODE=ANAL_T0) [Analysis T2 Comments],
                        ANAL_T3 [Analysis T3], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T3' AND ANAD_CODE=ANAL_T0) [Analysis T3 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T3' AND ANAD_CODE=ANAL_T0) [Analysis T3 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T3' AND ANAD_CODE=ANAL_T0) [Analysis T3 Comments],
                        ANAL_T4 [Analysis T4], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T4' AND ANAD_CODE=ANAL_T0) [Analysis T4 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T4' AND ANAD_CODE=ANAL_T0) [Analysis T4 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T4' AND ANAD_CODE=ANAL_T0) [Analysis T4 Comments],
                        ANAL_T5 [Analysis T5], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T5' AND ANAD_CODE=ANAL_T0) [Analysis T5 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T5' AND ANAD_CODE=ANAL_T0) [Analysis T5 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T5' AND ANAD_CODE=ANAL_T0) [Analysis T5 Comments],
                        ANAL_T6 [Analysis T6], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T6' AND ANAD_CODE=ANAL_T0) [Analysis T6 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T6' AND ANAD_CODE=ANAL_T0) [Analysis T6 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T6' AND ANAD_CODE=ANAL_T0) [Analysis T6 Comments],
                        ANAL_T7 [Analysis T7], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T7' AND ANAD_CODE=ANAL_T0) [Analysis T7 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T7' AND ANAD_CODE=ANAL_T0) [Analysis T7 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T7' AND ANAD_CODE=ANAL_T0) [Analysis T7 Comments],
                        ANAL_T8 [Analysis T8], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T8' AND ANAD_CODE=ANAL_T0) [Analysis T8 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T8' AND ANAD_CODE=ANAL_T0) [Analysis T8 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T8' AND ANAD_CODE=ANAL_T0) [Analysis T8 Comments],
                        ANAL_T9 [Analysis T9], (SELECT ANAD_DESC
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T9' AND ANAD_CODE=ANAL_T0) [Analysis T9 Description], (SELECT ANAD_LOOKUP
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T9' AND ANAD_CODE=ANAL_T0) [Analysis T9 Lookup], (SELECT ANAD_COM
                        FROM SIANALD
                        WHERE DB_CODE=@DB_CODE AND ANAM_CODE='T9' AND ANAD_CODE=ANAL_T0) [Analysis T9 Comments],
                        TRAN_DESC1 AS Description1, TRAN_DESC2 AS Description2, TRAN_DESC3 AS Description3,
                        TRAN_DESC4 AS Description4, TRAN_DESC5 AS Description5, TRAN_DESC6 AS Description6, HOLD_REF AS [Held Reference], ALLOCATION AS [Allocation Status]
                    FROM dbo.{dbCode.Key}SILEDG
                    WHERE  ALLOCATION<>'A' AND ACC_CODE IN (SELECT ACC_CODE
                        FROM SIACCNT
                        WHERE DB_CODE=@DB_CODE AND ACC_TYPE=@ACC_TYPE) AND (CASE WHEN @T='T0' THEN ANAL_T0 WHEN @T='T1' THEN ANAL_T1 WHEN @T='T2' THEN ANAL_T2 WHEN @T='T3' THEN ANAL_T3 WHEN @T='T4' THEN ANAL_T4 WHEN @T='T5' THEN ANAL_T5 WHEN @T='T6' THEN ANAL_T6 WHEN @T='T7' THEN ANAL_T7 WHEN @T='T8' THEN ANAL_T8 ELSE ANAL_T9 END) BETWEEN @FROM_ANAL AND @TO_ANAL
             ) TAB LEFT JOIN (SELECT UPPER(LEFT(USER_NAME,1))+LOWER(SUBSTRING(USER_NAME,2,LEN(USER_NAME))) username, USER_ID
                    FROM BCUSERS) U ON CONVERT(nvarchar,U.USER_ID)  =  TAB.[Analysis T0]
                    LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = TAB.[Account Code]
                WHERE [Amount_Total] >= 0
            END";
            var param = new
            {
                DB_CODE = dbCode.Key,
                BY_DATE = "",
                ACC_TYPE = "D",
                T = "",
                FROM_ANAL = "",
                TO_ANAL = "",
                T0 = "%",
                T1 = "%",
                T2 = "%",
                T3 = "%",
                T4 = "%",
                T5 = "%",
                T6 = "%",
                T7 = "%",
                T8 = "%",
                T9 = "%"
            };
            var execute = await _sqlDataAccess.LoadSingleData<OwedInvoiceDto, dynamic>(sql, param);
            execute.BranchCode = dbCode.Key;
            execute.BranchName = dbCode.Value;
            owedInvoice.Add(execute);
        }

        return owedInvoice;
    }
}