using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Contracts.Response.User;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.APPLICATION.Interfaces.StockCar;

public interface ITemplateRepository
{
    Task<List<TemplateResponse>> GetTemplatesAsync(string dbCode);
    Task<List<TemplateResponse>> GetTemplatesByEmployeeIdAsync(int employeeId);
    Task<int> AddNewTemplateAsync(Template req);
    Task<int> UpdateTemplateAsync(Template model);
    Task<int> DeleteTemplateAsync(int id);
    Task<List<UserResponse>> GetAllUsersWhoCompletedPayment(string dbCode);
}