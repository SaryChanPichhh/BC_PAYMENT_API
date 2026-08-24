namespace BC.PAYMENT.APPLICATION.Interfaces.General
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetCustomer(int offset, int pageSize);

        Task<List<Customer>> GetCustomer();

        Task<List<Customer>> GetCustomerByMarketCodeAsync(string dbCode,List<string> marketCode ,List<string> saleTypes );

    }
}
