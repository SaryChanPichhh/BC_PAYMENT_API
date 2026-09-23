using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Entities.StockCar;

namespace BC.PAYMENT.APPLICATION.Interfaces.StockCar;

public interface IStockCarExpenseRepository
{
    Task<List<StockCarExpenseResponse>> GetAllByTemplateIdAsync(string dbCode, int templateId);
    Task<StockCarExpenseResponse?> GetByTemplateIdAndExpenseIdAsync(string dbCode, int templateId, int expenseId);
    Task<int> SaveAsync(StockCarExpense expenseModel);
    Task<int> UpdateAsync(StockCarExpense model);
    Task<int> DeleteAsync(int expenseId);
    Task<TotalStockCarExpenseResponse?> GetTotalExpenseByTemplateIdAsync(string dbCode, int templateId);
}
