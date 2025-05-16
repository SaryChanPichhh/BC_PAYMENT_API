using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General
{
    public class CustomerRepository:ICustomerRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public CustomerRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<Customer>> GetCustomer(int offset, int pageSize)
        {
            const string sql = @"SELECT * FROM PM_GET_CUSTOMERS_BY_DB_CODE(@OFF_SET, @PAGESIZE)";
            var param = new
            {
                OFF_SET = offset,
                PAGESIZE = pageSize
            };
            var results = await _sqlDataAccess.LoadData<Customer, dynamic>(sql, param);
            return results.ToList();
        }
        public async Task<List<Customer>> GetCustomer()
        {
            const string sql = @"SELECT * FROM GET_CUSTOMERS_BY_DB_CODE()";
            
            var results = await _sqlDataAccess.LoadData<Customer, dynamic>(sql, new {});
            return results.ToList();
        }
    }
}
