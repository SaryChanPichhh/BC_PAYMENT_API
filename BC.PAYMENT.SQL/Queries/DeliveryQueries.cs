namespace BC.PAYMENT.SQL.Queries;

public static class DeliveryQueries
{
    public static string GetDelivery => "PM_SELECT_DELIVERIES";

    public static string GetDeliveryByPermission =>
        @"SELECT DELIVERIES_ID DeliveryId,DELIVERIES_KHMER DeliveryNameKhmer,ISNULL(IMAGE,'') Image FROM  TB_BCDELIVERIES   D
                INNER JOIN (SELECT DISTINCT U.USER_NAME,U.USER_ID, M.DB_CODE, U.FIELD_0 FROM BCUSERS U 
                INNER JOIN dbo.BCMSAPP M
                ON U.USER_ID = M.USER_ID 
                WHERE M.APP_CODE = 'HSM' AND U.USER_STATUS = 1 AND M.DB_CODE = @DB_CODE) AS
                USERS ON D.DELIVERIES_ID = USERS.FIELD_0 
                WHERE STATUS = '1'
                UNION ALL
                SELECT DELIVERIES_ID,DELIVERIES_KHMER,ISNULL(IMAGE,'') IMAGE FROM  (SELECT DELIVERIES_ID,DELIVERIES_KHMER,IMAGE,STATUS
                FROM TB_BCDELIVERIES WHERE DB_CODE = @DB_CODE AND (LOWER(DELIVERIES_NAME)= N'office') ) D
                WHERE STATUS = '1' ORDER BY DeliveryId";

    public static string GetDeliveryByPermissionAsync => GetDeliveryByPermission;

    public static string GetDeliveryImage =>
        "SELECT IMAGE FROM TB_BCDELIVERIES WHERE DELIVERIES_ID = @DELIVERIES_ID";

    public static string CreateDelivery =>
        @"INSERT INTO TB_BCDELIVERIES (DELIVERIES_ID, DB_CODE, DELIVERIES_NAME, DELIVERIES_KHMER, OTHERS, IMAGE, STATUS, USER_CREATE, CREATE_DATE)
          VALUES (@DeliveryId, @DbCode, @DeliveryName, @DeliveryNameKhmer, @Others, @Image, @Status, @UserCreate, @CreateDate)";

    public static string UpdateDelivery =>
        @"UPDATE TB_BCDELIVERIES 
          SET DB_CODE = @DbCode, 
              DELIVERIES_NAME = @DeliveryName, 
              DELIVERIES_KHMER = @DeliveryNameKhmer, 
              OTHERS = @Others, 
              IMAGE = COALESCE(@Image, IMAGE), 
              STATUS = @Status, 
              USER_UPDATE = @UserUpdate, 
              UPDATE_DATE = @UpdateDate
          WHERE DELIVERIES_ID = @DeliveryId";

    public static string DeleteDelivery =>
        "DELETE FROM TB_BCDELIVERIES WHERE DELIVERIES_ID = @DeliveryId";
}