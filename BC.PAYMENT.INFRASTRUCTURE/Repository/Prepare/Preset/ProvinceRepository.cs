using BC.PAYMENT.CORE.Contracts.Response.Province;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Prepare.Preset
{
    public class ProvinceRepository(ISqlDataAccess sqlDataAccess) : IProvinceRepository
    {
        public async Task<int> AddNewAsync(ProvinceModel model)
        {
            var sql = $@"INSERT INTO PROVINCE VALUES(@PRO_NAME)";
            var param = new { PRO_NAME = model.Province };
            var affectedRow = await sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }
        public async Task<int> UpdateAsync(ProvinceModel model)
        {
            var sql = $@"UPDATE dbo.PROVINCE SET PRO_NAME = @PRO_NAME WHERE PROID = @PRO_ID";
            var param = new { PRO_NAME = model.Province ,PRO_ID = model.ProvinceId};
            var affectedRow = await sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }
        

        public async Task<List<ProvinceModel>> GetAsync(string dbCode)
        {
            return [];
        }

        public async Task<int> DeleteAsync(string code)
        {
            var sql = $@"DELETE FROM dbo.PROVINCE WHERE PROID = @PROID";
            var param = new { PROID = code };
            var affectedRow = await sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<List<ProvinceResponse>> GetAllProvinces()
        {
            var sql = $@"SELECT PROID ProvinceId, PRO_NAME Province FROM dbo.PROVINCE ";
            var execute = await sqlDataAccess.LoadData<ProvinceResponse, dynamic>(sql, new { });
            return execute.ToList();
        }
    }
}
