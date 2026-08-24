namespace BC.PAYMENT.SQL.Queries
{
    public static class BranchQueries
    {
        public static string AllBranches => "SELECT DB_CODE DbCode,DB_NAME DbName FROM SIDBINFO WHERE DB_STAT = 'A'";
        public static string LoginBranches =>
            @" SELECT 
                 si.DB_CODE AS DbCode,
                 si.DB_NAME AS DbName
                 FROM 
                 SIDB.dbo.TDSTINFO TD 
                 INNER JOIN SIDB.dbo.TDDBDET si ON si.COMPANYCODE = TD.COMPANY_CODE
                 INNER JOIN 
                 dbo.BCMSAPP ba ON si.DB_CODE = ba.DB_CODE
                 INNER JOIN 
                 dbo.BCUSERS bu ON ba.USER_ID = bu.USER_ID
                 WHERE 
                 TD.DB_STAT = 'A'
                 AND ba.APP_CODE = @APP_CODE
                 AND bu.USER_NAME = @USER_NAME;";
    }
}
