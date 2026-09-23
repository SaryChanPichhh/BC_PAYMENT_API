using BC.PAYMENT.APPLICATION.Interfaces.StockCar;
using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Entities.StockCar;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.StockCar;

public class StockCarExpenseRepository(ISqlDataAccess sqlDataAccess) : IStockCarExpenseRepository
{
    public async Task<List<StockCarExpenseResponse>> GetAllByTemplateIdAsync(string dbCode, int templateId)
    {
        const string sql =
            @"SELECT E.ID, UPPER(Employee.USER_NAME) [Employee], T.EXPENSE_NAME ExpenseType, E.EXPENS_TYPE_ID ExpenseTypeId, E.QTY Quantity,
                     E.UNIT_PRICE UnitPrice, E.AMOUNT_RIEL AmountRiel, E.AMOUNT_DOLLAR AmountDollar, P.PRO_NAME ProvinceName, P.PROID ProvinceId,
                     E.EXCHANGE_RATE ExchangeRate, E.DATE ExpenseDate
              FROM BCSTOCK_CAR_EXPENSE E
              INNER JOIN BCSTOCK_CAR_EXPENSE_TYPE T ON T.EXPENSE_ID = E.EXPENS_TYPE_ID
              INNER JOIN PROVINCE P ON P.PROID = E.PROVINCE_ID
              INNER JOIN TEMPLATE TEMP ON TEMP.Id = E.TEMPLATE_ID
              INNER JOIN (SELECT USER_ID, USER_NAME FROM BCUSERS) Employee ON CONVERT(varchar, Employee.USER_ID) = TEMP.Employee
              WHERE E.DB_CODE = @DB_CODE AND T.DB_CODE = @DB_CODE AND E.TEMPLATE_ID = @TEMPLATE_ID";
        var parameter = new
        {
            DB_CODE = dbCode,
            TEMPLATE_ID = templateId
        };
        var data = await sqlDataAccess.LoadData<StockCarExpenseResponse, dynamic>(sql, parameter);
        return data.ToList();
    }

    public async Task<StockCarExpenseResponse?> GetByTemplateIdAndExpenseIdAsync(string dbCode, int templateId, int expenseId)
    {
        const string sql =
            @"SELECT E.ID, UPPER(Employee.USER_NAME) [Employee], T.EXPENSE_NAME ExpenseType, E.EXPENS_TYPE_ID ExpenseTypeId, E.QTY Quantity,
                     E.UNIT_PRICE UnitPrice, E.AMOUNT_RIEL AmountRiel, E.AMOUNT_DOLLAR AmountDollar, P.PRO_NAME ProvinceName, P.PROID ProvinceId,
                     E.EXCHANGE_RATE ExchangeRate, E.DATE ExpenseDate
              FROM BCSTOCK_CAR_EXPENSE E
              INNER JOIN BCSTOCK_CAR_EXPENSE_TYPE T ON T.EXPENSE_ID = E.EXPENS_TYPE_ID
              INNER JOIN PROVINCE P ON P.PROID = E.PROVINCE_ID
              INNER JOIN TEMPLATE TEMP ON TEMP.Id = E.TEMPLATE_ID
              INNER JOIN (SELECT USER_ID, USER_NAME FROM BCUSERS) Employee ON CONVERT(varchar, Employee.USER_ID) = TEMP.Employee
              WHERE E.DB_CODE = @DB_CODE AND T.DB_CODE = @DB_CODE AND E.TEMPLATE_ID = @TEMPLATE_ID AND E.ID = @ID";
        var parameter = new
        {
            DB_CODE = dbCode,
            TEMPLATE_ID = templateId,
            ID = expenseId
        };
        return await sqlDataAccess.LoadSingleData<StockCarExpenseResponse?, dynamic>(sql, parameter);
    }

    public async Task<int> SaveAsync(StockCarExpense expenseModel)
    {
        const string sql =
            @"INSERT INTO BCSTOCK_CAR_EXPENSE (DB_CODE,EXCHANGE_RATE,EXPENS_TYPE_ID,PROVINCE_ID,QTY,UNIT_PRICE,AMOUNT_DOLLAR,AMOUNT_RIEL,DATE,CREATED_BY,CREAETD_DATE,TEMPLATE_ID)
              OUTPUT inserted.ID
              VALUES (@DB_CODE,@EXCHANGE_RATE,@EXPENSE_TYPE_ID,@PROVINCE_ID,@QTY,@UNIT_PRICE,@AMOUNT_DOLLAR,@AMOUNT_RIEL,@DATE,@CREATED_BY,@CREATED_DATE,@TEMPLATE_ID)";
        var parameter = new
        {
            DB_CODE = expenseModel.DbCode,
            EXCHANGE_RATE = expenseModel.ExchangeRate,
            EXPENSE_TYPE_ID = expenseModel.ExpenseTypeId,
            PROVINCE_ID = expenseModel.ProvinceId,
            QTY = expenseModel.Quantity,
            UNIT_PRICE = expenseModel.UnitPrice,
            AMOUNT_DOLLAR = expenseModel.AmountDollar,
            AMOUNT_RIEL = expenseModel.AmountRiel,
            DATE = expenseModel.ExpenseDate,
            CREATED_BY = expenseModel.CreatedBy,
            CREATED_DATE = expenseModel.CreatedDate,
            TEMPLATE_ID = expenseModel.TemplateId
        };
        return await sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, parameter);
    }

    public async Task<int> UpdateAsync(StockCarExpense model)
    {
        const string sql =
            @"UPDATE BCSTOCK_CAR_EXPENSE
              SET EXCHANGE_RATE = @EXCHANGE_RATE,
                  EXPENS_TYPE_ID = @EXPENSE_TYPE_ID,
                  PROVINCE_ID = @PROVINCE_ID,
                  QTY = @QTY,
                  UNIT_PRICE = @UNIT_PRICE,
                  AMOUNT_DOLLAR = @AMOUNT_DOLLAR,
                  AMOUNT_RIEL = @AMOUNT_RIEL,
                  DATE = @DATE
              WHERE ID = @ID";
        var parameter = new
        {
            EXCHANGE_RATE = model.ExchangeRate,
            EXPENSE_TYPE_ID = model.ExpenseTypeId,
            PROVINCE_ID = model.ProvinceId,
            QTY = model.Quantity,
            UNIT_PRICE = model.UnitPrice,
            AMOUNT_DOLLAR = model.AmountDollar,
            AMOUNT_RIEL = model.AmountRiel,
            DATE = model.ExpenseDate,
            ID = model.Id
        };
        return await sqlDataAccess.ExecuteAsync(sql, parameter);
    }

    public async Task<int> DeleteAsync(int expenseId)
    {
        const string sql = "DELETE FROM BCSTOCK_CAR_EXPENSE WHERE ID = @ID";
        var parameter = new
        {
            ID = expenseId
        };
        return await sqlDataAccess.ExecuteAsync(sql, parameter);
    }

    public async Task<TotalStockCarExpenseResponse?> GetTotalExpenseByTemplateIdAsync(string dbCode, int templateId)
    {
        const string sql =
            @"SELECT ISNULL(SUM(AMOUNT_DOLLAR),0) DollarFromEmployee, ISNULL(SUM(AMOUNT_RIEL),0) RielFromEmployee
              FROM BCSTOCK_CAR_EXPENSE
              WHERE DB_CODE = @DB_CODE AND TEMPLATE_ID = @TEMPLATE_ID";
        var parameter = new
        {
            DB_CODE = dbCode,
            TEMPLATE_ID = templateId
        };
        return await sqlDataAccess.LoadSingleData<TotalStockCarExpenseResponse?, dynamic>(sql, parameter);
    }
}
