using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.APPLICATION.Interfaces.BaseInterface
{
    public interface IBaseRepository<T> where T : class
    {
        Task<int> AddNewAsync(T model);
        Task<int> UpdateAsync(T model);
        Task<List<T>> GetAsync(string dbCode);
        Task<int> DeleteAsync(string code);
    }
}
