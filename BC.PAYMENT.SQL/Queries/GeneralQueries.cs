namespace BC.PAYMENT.SQL.Queries;

public static class GeneralQueries
{
    public static string GetSaleTypes => @"SELECT CODE FROM SIDATA WHERE DB_CODE = @DB_CODE AND SI_TYPE  = 'SALES'";

    public static string LoadAccountCode =>
        @"SELECT ACC_CODE, IIF(LEN(TRIM(ACC_COM1))=0, ACC_NAME, ACC_COM1) ACC_COM1 FROM SIACCNT WHERE DB_CODE = @DB_CODE AND ACC_TYPE = 'D' AND ACC_BF = 'B'";

    public static string LoadAnalysisByRangeDetails =>
        @"SELECT ANAD_CODE, ANAD_COM FROM SIANALD WHERE ANAM_CODE = @ANAM_CODE AND DB_CODE = @DB_CODE";

    public static string LoadAnalysisByAllDetail =>
        @"SELECT ANAD_CODE, ANAD_DESC, ANAD_COM FROM SIANALD WHERE ANAM_CODE = @ANAM_CODE AND DB_CODE = @DB_CODE";

    public static string LoadAnalysisType =>
        @"SELECT ANAM_CODE, ANAM_DESC FROM SIANALM WHERE DB_CODE = @DB_CODE AND ANAM_CODE LIKE 'T%' GROUP BY ANAM_CODE, ANAM_DESC";

    public static string GetPeriod =>
        @$"SELECT CODE FROM dbo.SIDATA WHERE DB_CODE = @DB_CODE AND SI_LOOKUP = 'Current'";
}