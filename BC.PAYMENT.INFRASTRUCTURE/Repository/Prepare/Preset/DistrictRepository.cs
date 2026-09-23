namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Prepare.Preset
{
    public class DistrictRepository(ISqlDataAccess sqlDataAccess) : IDistrictRepository
    {
        public async Task<int> AddNewAsync(DistrictModel model)
        {
            var sql = DistrictQueries.AddNew;

            var param = new
            {
                D_NAME = model.District,
                P_ID = model.ProvinceId
            };
            
            var affectedRow = await sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<int> UpdateAsync(DistrictModel model)
        {
            var sql = DistrictQueries.Update;

            var param = new
            {
                D_ID = model.DistrictId,
                D_NAME = model.District,
                P_ID = model.ProvinceId
            };
            var affectedRow = await sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<List<DistrictResponseDTO>> GetAsync(string dbCode)
        {
            var sql = DistrictQueries.Get;
            var param = new
            {
            };
            var execute = await sqlDataAccess.LoadData<DistrictResponseDTO, dynamic>(sql,param);
            return execute.ToList();
        }

        public async Task<int> DeleteAsync(string code)
        {
            var sql = DistrictQueries.Delete;

            var param = new
            {
                D_ID = code,
            };
            var affectedRow = await sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<List<DistrictResponseDTO>> GetDistrictsByProvinceAsync(string province)
        {
            var sql = DistrictQueries.GetDistrictsByProvince;
            var param = new
            {
                PRO_ID = province,
            };
            var execute = await sqlDataAccess.LoadData<DistrictResponseDTO, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<DistrictResponseDTO>> GetDistrictsByDistrictAsync(string district)
        {
            var sql = DistrictQueries.GetDistrictsByDistrict;
            var param = new
            {
                DISTRICT = district,
            };
            var execute = await sqlDataAccess.LoadData<DistrictResponseDTO, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
