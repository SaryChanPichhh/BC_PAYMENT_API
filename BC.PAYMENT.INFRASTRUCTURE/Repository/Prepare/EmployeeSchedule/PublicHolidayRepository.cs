namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Prepare.EmployeeSchedule
{
    public  class PublicHolidayRepository : IPublicHolidayRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public PublicHolidayRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<int> AddNewAsync(PublicHolidayModel model)
        {
            const string sql =
                @"INSERT INTO EMPHOLIDAY(HOLIDAY_CODE,HOLIDAY_DES,[START_DATE],END_DATE,REMARK,USER_CREA,DATE_CREA,ACTIVE)
             VALUES(@HOLIDAY_CODE,@HOLIDAY_DES,@START_DATE,@END_DATE,@REMARK,@USER_CREA,@DATE_CREA,@ACTIVE)";
            var param = new
            {
                HOLIDAY_CODE = model.Code,
                HOLIDAY_DES = model.Description,
                START_DATE = model.StartDate,
                END_DATE = model.EndDate,
                REMARK = model.Remark,
                USER_CREA = model.UserName,
                DATE_CREA = DateTime.Now,
                ACTIVE = "A"
            };
            var rowAffected = await _sqlDataAccess.ExecuteAsync(sql, param);
            return rowAffected;
        }
        public async Task<List<PublicHolidayModel>> GetListHoliday()
        {
            string sql = @"SELECT HOLIDAY_CODE Code,HOLIDAY_DES [Description],START_DATE StartDate,END_DATE EndDate,REMARK Remark,ACTIVE [Status],LEFT(CAST(START_DATE AS DATE),4) [Year] FROM dbo.EMPHOLIDAY; ";
            var execute = await _sqlDataAccess.LoadData<PublicHolidayModel, dynamic>(sql, new { });
            return execute.ToList();
        }
        public async Task<int> UpdateAsync(PublicHolidayModel model)
        {
            const string sql =
                @"UPDATE EMPHOLIDAY SET HOLIDAY_DES=@HOLIDAY_DES,START_DATE = @START_DATE,END_DATE =@END_DATE,USER_UPDT = @USER_UPDT,DATE_UPDT=@DATE_UPDT,REMARK=@REMARK WHERE HOLIDAY_CODE = @HOLIDAY_CODE";
            var param = new
            {
                HOLIDAY_CODE = model.Code,
                HOLIDAY_DES = model.Description,
                START_DATE = model.StartDate,
                END_DATE = model.EndDate,
                REMARK = model.Remark,
                USER_UPDT = model.UserName,
                DATE_UPDT = DateTime.Now,
                ACTIVE = "A"
            };
            var rowAffected = await _sqlDataAccess.ExecuteAsync(sql, param);
            return rowAffected;
        }

        public async Task<List<PublicHolidayModel>> GetAsync(string dbCode)
        {
            string sql = @"SELECT HOLIDAY_CODE Code,HOLIDAY_DES [Description],START_DATE StartDate,END_DATE EndDate,REMARK Remark,ACTIVE [Status],LEFT(CAST(START_DATE AS DATE),4) [Year] FROM dbo.EMPHOLIDAY; ";
            var execute = await _sqlDataAccess.LoadData<PublicHolidayModel, dynamic>(sql, new { });
            return execute.ToList();
        }

        public async Task<int> DeleteAsync(string code)
        {
            string sql = @"DELETE FROM dbo.EMPHOLIDAY WHERE HOLIDAY_CODE = @HOLIDAY_CODE";
            var param = new { HOLIDAY_CODE = code };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<string> GetMaxCodePublicHolidayAsync()
        {
            string sql = @"SELECT MAX(HOLIDAY_CODE) FROM dbo.EMPHOLIDAY";
            var execute = await _sqlDataAccess.LoadSingleData<string, dynamic>(sql, new { });
            var maxCode = int.Parse(execute) + 1;
            return maxCode.ToString("D4");
        }
    }
}
