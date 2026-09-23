using BC.PAYMENT.APPLICATION.Interfaces.StockCar;
using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.StockCar;

public class TemplateRepository(ISqlDataAccess sqlDataAccess) : ITemplateRepository
{
    public async Task<List<TemplateResponse>> GetTemplatesAsync(string dbCode)
    {
        var sql = $@"SELECT Id,FORMAT(CreatedDate, 'MM-dd-yyyy') CreatedDate,CreatedBy,UPPER(EmployeeName) Employee,EmployeeId,Description FROM TEMPLATE INNER JOIN (SELECT USER_ID EmployeeId,USER_NAME EmployeeName FROM BCUSERS)
                   Employee on Employee.EmployeeId = TEMPLATE.Employee WHERE DbCode = @DB_CODE AND IsEnable = '1';";
        var param = new
        {
            DB_CODE = dbCode
        };
        return (await sqlDataAccess.LoadData<TemplateResponse, dynamic>(sql, param)).ToList();
    }

    public async Task<List<TemplateResponse>> GetTemplatesByEmployeeIdAsync(int employeeId)
    {
        const string sql = @"SELECT Id, Description, ClosingDate, CreatedDate, CreatedBy, Employee
                             FROM TEMPLATE WHERE Employee = @Employee";
        var param = new { Employee = employeeId };
        return (await sqlDataAccess.LoadData<TemplateResponse, dynamic>(sql, param)).ToList();
    }

    public async Task<int> AddNewTemplateAsync(Template req)
    {
        var sql = $@"INSERT INTO TEMPLATE(Employee,CreatedDate,CreatedBy,Description,DbCode,IsEnable) OUTPUT inserted.Id VALUES(@EMPLOYEE,@CREATED_DATE,@CREATED_BY,@DESCRIPTION,@DB_CODE,@IS_ENABLE)";
        var param = new
        {
            EMPLOYEE = req.Employee,
            CREATED_DATE = req.CreatedDate,
            CREATED_BY = req.CreatedBy,
            DESCRIPTION = req.Description,
            DB_CODE = req.DbCode,
            IS_ENABLE = true,
        };
        var affectedRow = await sqlDataAccess.ExecuteAsync(sql, param);
        return affectedRow;
    }
    public async Task<int> UpdateTemplateAsync(Template model)
    {
        const string sql = "UPDATE TEMPLATE SET Employee = @EMPLOYEE,Description = @DESCRIPTION WHERE Id = @ID";
        var parameter = new
        {
            ID = model.Id,
            EMPLOYEE = model.Employee,
            DESCRIPTION = model.Description
        };
        var affectedRow = await sqlDataAccess.ExecuteAsync(sql, parameter);
        return affectedRow;
    }

    public async Task<int> DeleteTemplateAsync(int id)
    {
         var sql = "DELETE FROM TEMPLATE WHERE Id = @ID";
        var parameter = new
        {
            ID = id
        };
        var affectedRow = await sqlDataAccess.ExecuteAsync(sql, parameter);
        return affectedRow;
    }
}