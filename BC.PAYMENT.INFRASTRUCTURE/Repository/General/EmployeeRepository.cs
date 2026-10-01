namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public EmployeeRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<EmployeeModel>> GetAllHasCompletedPayment()
    {
        const string sql = @"
                SELECT DISTINCT U.USER_ID EmployeeId,UPPER(LEFT(USER_NAME,1))+LOWER(SUBSTRING(USER_NAME,2,LEN(USER_NAME))) EmployeeName
                FROM BCUSERS U INNER JOIN BCMSAPP MS ON MS.USER_ID = U.USER_ID
                WHERE U.USER_TYPE = 'S' AND U.USER_ID in (SELECT Employee FROM TEMPLATE WHERE IsEnable = 0)";
        var execute = await _sqlDataAccess.LoadData<EmployeeModel, dynamic>(sql, new { });
        return execute.ToList();
    }

    public async Task<List<TemplateModel>> GetAllTemplateByEmployeId(int employeeId)
    {
        var sql = $@"SELECT Id,Description,ClosingDate FROM TEMPLATE WHERE Employee = @Employee";
        var param = new
        {
            Employee = employeeId
        };
        var execute = await _sqlDataAccess.LoadData<TemplateModel, dynamic>(sql, param);
        return execute.ToList();
    }
}