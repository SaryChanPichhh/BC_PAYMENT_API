
using BC.PAYMENT.APPLICATION.Interfaces.Prepare.Preset;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Entities.Prepare.Preset;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using System.Reflection;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Prepare.Preset
{
    public class DistrictRepository : IDistrictRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public DistrictRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<int> AddNewAsync(DistrictModel model)
        {
            var sql = $@"INSERT INTO DISTRICT VALUES (@D_ID,@D_NAME,@P_ID);";

            var param = new
            {
                D_ID = model.DistrictId,
                D_NAME = model.District,
                P_ID = model.ProvinceId
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<int> UpdateAsync(DistrictModel model)
        {
            var sql = $@"UPDATE DISTRICT SET D_NAME = @D_NAME,P_ID = @P_ID WHERE D_ID = @D_ID";

            var param = new
            {
                D_ID = model.DistrictId,
                D_NAME = model.District,
                P_ID = model.ProvinceId
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<List<DistrictModel>> GetAsync(string dbCode)
        {
            var sql = $@"SELECT D_ID DistrictId,D_NAME District,P.PROID ProvinceId,P.PRO_NAME Province FROM DISTRICT INNER JOIN PROVINCE P ON P.PROID = DISTRICT.P_ID ";
            var param = new
            {
            };
            var execute = await _sqlDataAccess.LoadData<DistrictModel, dynamic>(sql,param);
            return execute.ToList();
        }

        public async Task<int> DeleteAsync(string code)
        {
            var sql = $@"DELETE FROM DISTRICT WHERE D_ID = @D_ID";

            var param = new
            {
                D_ID = code,
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<List<DistrictModel>> GetDistrictsByProvinceAsync(string province)
        {
            var sql = $@"SELECT D_ID DistrictId,D_NAME District,P.PROID ProvinceId,P.PRO_NAME Province FROM DISTRICT INNER JOIN PROVINCE P ON P.PROID = DISTRICT.P_ID WHERE P.PRO_NAME LIKE '%@PROVINCE%'";
            var param = new
            {
                PROVINCE = province,
            };
            var execute = await _sqlDataAccess.LoadData<DistrictModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<DistrictModel>> GetDistrictsByDistrictAsync(string district)
        {
            var sql = $@"SELECT D_ID DistrictId,D_NAME District,P.PROID ProvinceId,P.PRO_NAME Province FROM DISTRICT INNER JOIN PROVINCE P ON P.PROID = DISTRICT.P_ID WHERE P.PRO_NAME LIKE '%@DISTRICT%'";
            var param = new
            {
                DISTRICT = district,
            };
            var execute = await _sqlDataAccess.LoadData<DistrictModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
