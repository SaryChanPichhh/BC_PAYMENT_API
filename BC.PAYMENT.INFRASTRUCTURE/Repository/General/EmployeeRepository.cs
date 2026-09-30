namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class EmployeeRepository(ISqlDataAccess sqlDataAccess) : IEmployeeRepository
    {
        public async Task<List<EmployeeModel>> GetAllHasCompletedPayment()
        {
            const string sql = @"
                SELECT DISTINCT U.USER_ID EmployeeId,UPPER(LEFT(USER_NAME,1))+LOWER(SUBSTRING(USER_NAME,2,LEN(USER_NAME))) EmployeeName
                FROM BCUSERS U INNER JOIN BCMSAPP MS ON MS.USER_ID = U.USER_ID
                WHERE U.USER_TYPE = 'S' AND U.USER_ID in (SELECT Employee FROM TEMPLATE WHERE IsEnable = 0)";
            var execute = await sqlDataAccess.LoadData<EmployeeModel, dynamic>(sql, new{});
            return execute.ToList();
        }
    }
}
