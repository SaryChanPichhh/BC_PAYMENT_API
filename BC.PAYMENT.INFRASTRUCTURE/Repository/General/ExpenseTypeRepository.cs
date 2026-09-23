using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using BC.PAYMENT.SQL.Queries;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class ExpenseTypeRepository(ISqlDataAccess sqlDataAccess) : IExpenseTypeRepository
    {

        public async Task<List<ExpenseType>> GetExpenseTypes(string dbCode)
        {
            var param = new { DB_CODE = dbCode };
            var result = await sqlDataAccess.LoadData<ExpenseType, dynamic>(ExpenseTypeQueries.GetExpenseTypes, param, CommandType.Text);
            return result.ToList();
        }

        public async Task<bool> CreateExpenseType(ExpenseType expenseType)
        {
            var result = await sqlDataAccess.ExecuteAsync(ExpenseTypeQueries.CreateExpenseType, expenseType);
            return result > 0;
        }

        public async Task<bool> UpdateExpenseType(ExpenseType expenseType)
        {
            var result = await sqlDataAccess.ExecuteAsync(ExpenseTypeQueries.UpdateExpenseType, expenseType);
            return result > 0;
        }

        public async Task<bool> DeleteExpenseType(string expenseId)
        {
            var param = new { ExpenseId = expenseId };
            var result = await sqlDataAccess.ExecuteAsync(ExpenseTypeQueries.DeleteExpenseType, param);
            return result > 0;
        }
    }
}
