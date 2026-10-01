namespace BC.PAYMENT.SQL.Queries;

public static class BranchQueries
{
    public static string AllBranches => "SELECT DB_CODE DbCode,DB_NAME DbName FROM SIDBINFO WHERE DB_STAT = 'A'";

    public static string LoginBranches =>
        @" IF(@USER_NAME='BCSA')
            BEGIN 
              SELECT DB_CODE DbCode,DB_NAME DbName FROM SIDBINFO WHERE DB_STAT = 'A';
            END
            ELSE
              BEGIN
                  SELECT 
                       S.DB_CODE AS DbCode,S.DB_NAME AS DbName
                      FROM SIDB.dbo.SIDBINFO S  
                     INNER JOIN  dbo.BCMSAPP ba ON S.DB_CODE = ba.DB_CODE
                      INNER JOIN   dbo.BCUSERS bu ON ba.USER_ID = bu.USER_ID
                     WHERE  S.DB_STAT = 'A' AND ba.APP_CODE = @APP_CODE AND bu.USER_NAME = @USER_NAME
              END";
}