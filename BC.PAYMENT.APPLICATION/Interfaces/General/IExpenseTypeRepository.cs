using BC.PAYMENT.CORE.Entities.General;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BC.PAYMENT.APPLICATION.Interfaces.General;

public interface IExpenseTypeRepository
{
    Task<List<ExpenseType>> GetExpenseTypes(string dbCode);
    Task<bool> CreateExpenseType(ExpenseType expenseType);
    Task<bool> UpdateExpenseType(ExpenseType expenseType);
    Task<bool> DeleteExpenseType(string expenseId);
}