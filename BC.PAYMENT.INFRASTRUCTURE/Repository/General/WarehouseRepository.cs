namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public WarehouseRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<WarehouseDto>> GetWarehouseAsync(string dbCode)
        {
            var sql = $@"SELECT WAR_CODE WarehouseCode , WAR_NAME WarehouseName FROM SIWAREH WHERE DB_CODE = @DB_CODE AND WAR_STAT = 'A';";
            var param = new
            {
                DB_CODE = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<WarehouseDto, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
