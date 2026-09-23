namespace BC.PAYMENT.SQL.Queries;

public static class AreaQueries
{
    public static string GetAllArea =>
        $@"SELECT TA.AREA_ID AreaId,TA.AREA_NAME AreaName,TA.AREA_NAME_KHMER AreaNameKhmer,
TB.DELIVERIES_NAME DeliveryNamme,TB.DELIVERIES_KHMER DeliveryNameKhmer,TA.OTHER Other,TA.STATUS Status FROM TB_AREAS TA
LEFT JOIN TB_BCDELIVERIES TB
ON TA.DELIVERY_ID = TB.DELIVERIES_ID
 WHERE TA.DB_CODE = @DB_CODE AND TA.STATUS = 1";

    public static string CreateArea =>
        @"INSERT INTO TB_AREAS (AREA_ID, DB_CODE, AREA_NAME, AREA_NAME_KHMER, OTHER, STATUS, USER_CREATE, CREATE_DATE) 
          VALUES (@AreaId, @DbCode, @AreaName, @AreaNameKhmer, @Other, @Status, @CreatedBy, @CreatedAt)";

    public static string UpdateArea =>
        @"UPDATE TB_AREAS 
          SET AREA_NAME = @AreaName, AREA_NAME_KHMER = @AreaNameKhmer, OTHER = @Other, STATUS = @Status, USER_UPDATE = @UpdatedBy, UPDATE_DATE = @UpdatedAt 
          WHERE AREA_ID = @AreaId AND DB_CODE = @DbCode";

    public static string DeleteArea =>
        @"DELETE FROM TB_AREAS WHERE AREA_ID = @AreaId AND DB_CODE = @DbCode";

    public static string GenerateAreaId =>
        "SELECT dbo.Generate_ID_area()";
}