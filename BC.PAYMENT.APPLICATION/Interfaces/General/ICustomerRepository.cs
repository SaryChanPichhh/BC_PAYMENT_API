using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC.PAYMENT.CORE.Entities.General;

namespace BC.PAYMENT.APPLICATION.Interfaces.General
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetCustomer(int offset, int pageSize);

        Task<List<Customer>> GetCustomer();

        Task<List<Customer>> GetCustomerByMarketCodeAsync(string dbCode,List<string> marketCode ,List<string> saleTypes );

    }
}
