namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class AreaRepository :IAreaRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        public AreaRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<Area>> GetArea(string dbCode)
        {
            var sql = "[PM_SELECT_AREAS]";

            var param = new
            {
                DB_CODE = dbCode,
            };
            var result = await _sqlDataAccess.LoadData<Area, dynamic>(sql, param, CommandType.StoredProcedure);
            return result.ToList();
        }
    }
}
