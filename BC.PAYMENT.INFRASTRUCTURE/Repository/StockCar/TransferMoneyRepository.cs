using BC.PAYMENT.APPLICATION.Interfaces.StockCar;
using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Entities.StockCar;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.StockCar;

public class TransferMoneyRepository(ISqlDataAccess sqlDataAccess) : ITransferMoneyRepository
{
    public async Task<List<TransferMoneyResponse>> GetTransferMoneyAsync(string dbCode, int templateId)
    {
        const string sql =
            @"SELECT S.ID Id, S.TRANSACTION_DATE TransactionDate, S.AMOUNT Amount, S.DESCRIPTION Description,
                     S.DEPOSIT_DOLLAR DepositDollar, S.DEPOSIT_RIEL DepositRiel, S.DEPOSIT_EXCHANGE DepositExchange,
                     S.DOLLAR DollarFromEmployee, S.RIEL RielFromEmployee, S.EXCHANGE ExchangeRateEmployee,
                     UPPER(Emp.username) Employee, Emp.userid EmployeeId
              FROM BCSTOCK_CAR_SENT_MONEY_DETAILS S
              INNER JOIN TEMPLATE T ON T.Id = S.TEMPLATE_ID
              INNER JOIN (SELECT userid, username FROM USERS WHERE dbcode = @DbCode) Emp ON CONVERT(VARCHAR, Emp.userid) = T.Employee
              WHERE S.DB_CODE = @DbCode AND S.TEMPLATE_ID = @TemplateId";
        var parameter = new
        {
            DbCode = dbCode,
            TemplateId = templateId
        };
        var data = await sqlDataAccess.LoadData<TransferMoneyResponse, dynamic>(sql, parameter);
        return data.ToList();
    }

    public async Task<TransferMoneyResponse?> GetTransferMoneyByIdAsync(string dbCode, int templateId, int id)
    {
        const string sql =
            @"SELECT S.ID Id, S.TRANSACTION_DATE TransactionDate, S.AMOUNT Amount, S.DESCRIPTION Description,
                     S.DEPOSIT_DOLLAR DepositDollar, S.DEPOSIT_RIEL DepositRiel, S.DEPOSIT_EXCHANGE DepositExchange,
                     S.DOLLAR DollarFromEmployee, S.RIEL RielFromEmployee, S.EXCHANGE ExchangeRateEmployee,
                     UPPER(Emp.username) Employee, Emp.userid EmployeeId
              FROM BCSTOCK_CAR_SENT_MONEY_DETAILS S
              INNER JOIN TEMPLATE T ON T.Id = S.TEMPLATE_ID
              INNER JOIN (SELECT userid, username FROM USERS WHERE dbcode = @DbCode) Emp ON CONVERT(VARCHAR, Emp.userid) = T.Employee
              WHERE S.DB_CODE = @DbCode AND S.TEMPLATE_ID = @TemplateId AND S.ID = @Id";
        var parameter = new
        {
            DbCode = dbCode,
            TemplateId = templateId,
            Id = id
        };
        return await sqlDataAccess.LoadSingleData<TransferMoneyResponse?, dynamic>(sql, parameter);
    }

    public async Task<int> AddNewTransferMoneyAsync(TransferMoney transferMoney)
    {
        const string sql =
            @"INSERT INTO BCSTOCK_CAR_SENT_MONEY_DETAILS(TRANSACTION_DATE,DESCRIPTION,AMOUNT,DOLLAR,RIEL,EXCHANGE,DEPOSIT_DOLLAR,DEPOSIT_RIEL,DEPOSIT_EXCHANGE,CREATED_DATE,CREATED_BY,DB_CODE,EMPLOYEE,TEMPLATE_ID)
              VALUES(@TransactionDate,@Description,@Amount,@Dollar,@Riel,@Exchange,@DepositDollar,@DopositRiel,@DepositExchange,@CreatedDate,@CreatedBy,@DbCode,@Employee,@TemplateId)";
        var parameter = new
        {
            transferMoney.TransactionDate,
            transferMoney.Description,
            transferMoney.Amount,
            Dollar = transferMoney.DollarFromEmployee,
            Riel = transferMoney.RielFromEmployee,
            Exchange = transferMoney.ExchangeRateEmployee,
            transferMoney.DepositDollar,
            DopositRiel = transferMoney.DepositRiel,
            transferMoney.DepositExchange,
            transferMoney.CreatedDate,
            transferMoney.CreatedBy,
            transferMoney.DbCode,
            Employee = transferMoney.EmployeeId,
            transferMoney.TemplateId
        };
        return await sqlDataAccess.ExecuteAsync(sql, parameter);
    }

    public async Task<int> UpdateTransferMoneyAsync(TransferMoney transferMoney)
    {
        const string sql =
            @"UPDATE BCSTOCK_CAR_SENT_MONEY_DETAILS
              SET TRANSACTION_DATE = @TransactionDate,
                  DESCRIPTION = @Description,
                  AMOUNT = @Amount,
                  DOLLAR = @Dollar,
                  RIEL = @Riel,
                  EXCHANGE = @ExchangeFromSale,
                  DEPOSIT_DOLLAR = @DepositDollar,
                  DEPOSIT_RIEL = @DepositRiel,
                  DEPOSIT_EXCHANGE = @DepositExchange
              WHERE ID = @Id";
        var parameter = new
        {
            transferMoney.TransactionDate,
            transferMoney.Description,
            transferMoney.Amount,
            Dollar = transferMoney.DollarFromEmployee,
            Riel = transferMoney.RielFromEmployee,
            ExchangeFromSale = transferMoney.ExchangeRateEmployee,
            transferMoney.DepositDollar,
            transferMoney.DepositRiel,
            transferMoney.DepositExchange,
            transferMoney.Id
        };
        return await sqlDataAccess.ExecuteAsync(sql, parameter);
    }

    public async Task<int> DeleteTransferMoneyAsync(int transferId)
    {
        const string sql = @"DELETE FROM BCSTOCK_CAR_SENT_MONEY_DETAILS WHERE ID = @ID";
        var parameter = new
        {
            ID = transferId
        };
        return await sqlDataAccess.ExecuteAsync(sql, parameter);
    }

    public async Task<TotalTransferMoneyResponse?> GetTotalTransferMoneyByTemplateIdAsync(string dbCode, int templateId)
    {
        const string sql =
            @"SELECT ISNULL(SUM(DOLLAR),0) DollarFromEmployee, ISNULL(SUM(RIEL),0) RielFromEmployee
              FROM BCSTOCK_CAR_SENT_MONEY_DETAILS
              WHERE DB_CODE = @DbCode AND TEMPLATE_ID = @TemplateId";
        var parameter = new
        {
            DbCode = dbCode,
            TemplateId = templateId
        };
        return await sqlDataAccess.LoadSingleData<TotalTransferMoneyResponse?, dynamic>(sql, parameter);
    }
}