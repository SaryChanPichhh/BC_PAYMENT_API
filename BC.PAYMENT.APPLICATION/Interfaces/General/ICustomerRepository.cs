using BC.PAYMENT.CORE.Contracts.Response;
using BC.PAYMENT.CORE.Contracts.Response.Customer;

namespace BC.PAYMENT.APPLICATION.Interfaces.General;

public interface ICustomerRepository
{
    Task<List<Customer>> GetCustomer(int offset, int pageSize);

    Task<List<Customer>> GetCustomer();

    Task<List<Customer>> GetCustomerByMarketCodeAsync(string dbCode, List<string> marketCode, List<string> saleTypes);
    Task<List<CustomerResponse>> GetCustomerInfoByMarketIdAsync(string marketId);
    Task<List<CustomerResponse>> GetAllCustomerInfoAsync(string dbCode, int page, int pageSize);
    Task<List<CustomerResponse>> GetCustomerWhoWrongAreaAndMarketAsync(string dbCode);
    Task<int> GetAllCustomerInfoCountAsync(string dbCode);
    Task<byte[]> GetCustomerImageAsync(string dbCode, string customerId);
}