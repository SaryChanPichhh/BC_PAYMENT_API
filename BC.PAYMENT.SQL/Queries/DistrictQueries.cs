namespace BC.PAYMENT.SQL.Queries
{
    public static class DistrictQueries
    {
        public static string AddNew => "INSERT INTO DISTRICT VALUES (@D_NAME,@P_ID);";
        
        public static string Update => "UPDATE DISTRICT SET D_NAME = @D_NAME,P_ID = @P_ID WHERE D_ID = @D_ID";
        
        public static string Get => "SELECT D_ID DistrictId,D_NAME District,P.PROID ProvinceId,P.PRO_NAME Province FROM DISTRICT INNER JOIN PROVINCE P ON P.PROID = DISTRICT.P_ID ";
        
        public static string Delete => "DELETE FROM DISTRICT WHERE D_ID = @D_ID";
        
        public static string GetDistrictsByProvince => $@"SELECT D_ID DistrictId,D_NAME District,P.PROID ProvinceId,P.PRO_NAME Province 
        FROM DISTRICT INNER JOIN PROVINCE P ON P.PROID = DISTRICT.P_ID WHERE P.PROID = @PRO_ID";
        
        public static string GetDistrictsByDistrict => "SELECT D_ID DistrictId,D_NAME District,P.PROID ProvinceId,P.PRO_NAME Province FROM DISTRICT INNER JOIN PROVINCE P ON P.PROID = DISTRICT.P_ID WHERE P.D_ID = @D_ID";
    }
}
