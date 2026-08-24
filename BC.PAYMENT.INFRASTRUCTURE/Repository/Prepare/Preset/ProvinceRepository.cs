namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Prepare.Preset
{
    public  class ProvinceRepository : IProvinceRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ProvinceRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<int> AddNewAsync(ProvinceModel model)
        {
            var sql = $@"INSERT INTO PROVINCE VALUES(@PRO_NAME)";
            var param = new { PRO_NAME = model.Province };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }
        public async Task<int> UpdateAsync(ProvinceModel model)
        {
            var sql = $@"UPDATE dbo.PROVINCE SET PRO_NAME = @PRO_NAME WHERE PROID = @PROID";
            var param = new { PRO_NAME = model.Province };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<List<ProvinceModel>> GetAsync(string dbCode)
        {
            var sql = $@"SELECT PROID ProvinceId, PRO_NAME Province FROM dbo.PROVINCE ";
            var execute = await _sqlDataAccess.LoadData<ProvinceModel, dynamic>(sql, new { });
            return execute.ToList();
        }

        public async Task<int> DeleteAsync(string code)
        {
            var sql = $@"DELETE FROM dbo.PROVINCE WHERE PROID = @PROID";
            var param = new { PROID = code };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }
    }
}
