namespace BC.PAYMENT.SQL.Queries;

public static class UserStoreProcedures
{
    public static string AllUser
        => @"SELECT 
                    U.USER_ID UserId, 
                    U.USER_NAME Username, 
                    M.DB_CODE DbCode, 
                    CONCAT(U.FIRST_NAME, ' ', U.LAST_NAME) AS [Name], 
                    U.USER_PASS UserPass,
                    GETDATE() CurrentDate
                FROM 
                    dbo.BCUSERS U
                INNER JOIN 
                    dbo.BCMSAPP M 
                    ON M.USER_ID = U.USER_ID
                WHERE 
                    M.DB_CODE = @DB_CODE 
                    AND M.APP_CODE = @APP_CODE
                    AND U.USER_STATUS = 1";

    public static string UserById
        => @"SELECT 
                    U.USER_ID UserId, 
                    U.USER_NAME Username, 
                    M.DB_CODE DbCode, 
                    CONCAT(U.FIRST_NAME, ' ', U.LAST_NAME) AS [Name], 
                    U.USER_PASS UserPass,
                    GETDATE() CurrentDate
                FROM 
                    dbo.BCUSERS U
                INNER JOIN 
                    dbo.BCMSAPP M 
                    ON M.USER_ID = U.USER_ID
                WHERE 
                    U.USER_ID = @USER_ID
                    AND M.DB_CODE = @DB_CODE 
                    AND M.APP_CODE = @APP_CODE
                    AND U.USER_STATUS = 1";

    public static string IsUserAuthorized => "dbo.GET_BCUSER_CREDENTIAL ";

    public static string GetCredential
        => @"SELECT 
                    U.USER_ID UserId, 
                    U.USER_NAME Username, 
                    M.DB_CODE DbCode, 
                    CONCAT(U.FIRST_NAME, ' ', U.LAST_NAME) AS [Name], 
                    U.USER_PASS UserPass,
                    GETDATE() CurrentDate
                FROM 
                    dbo.BCUSERS U
                INNER JOIN 
                    dbo.BCMSAPP M 
                    ON M.USER_ID = U.USER_ID
                WHERE 
                    U.USER_NAME = @USER_NAME
                    AND M.DB_CODE = @DB_CODE 
                    AND M.APP_CODE = @APP_CODE
                    AND U.USER_STATUS = 1;";

    public static string IsExistsUser =>
        $@"
    SELECT CAST(CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END AS BIT) IsExists
                  FROM 
                      dbo.BCUSERS U
                  INNER JOIN 
                      dbo.BCMSAPP M 
                      ON M.USER_ID = U.USER_ID
                  WHERE 
                      U.USER_NAME = @USER_NAME
                      AND M.APP_CODE = @APP_CODE
                      AND U.USER_STATUS = 1;
";

    public static string GetUserId => "";
    public static string GetAppCode => "";
}