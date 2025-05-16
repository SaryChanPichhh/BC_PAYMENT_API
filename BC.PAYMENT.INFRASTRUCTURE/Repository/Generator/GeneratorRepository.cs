

using BC.PAYMENT.APPLICATION.Interfaces.Generator;
using BC.PAYMENT.CORE.DTO.Generator;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Generator
{
    public class GeneratorRepository : IGeneratorRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public GeneratorRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<SaleAnalysisDto> GetSaleAnalysisByCustomerCodeAsync(string customerCode, string dbCode)
        { 
            var sql =
                @"SELECT ANAL_C0 AnalysisC0,ANAL_C1 AnalysisC1,ANAL_C2 AnalysisC2,ANAL_C3 AnalysisC3,ANAL_C4 AnalysisC4,ANAL_C5 AnalysisC5,ANAL_C6 AnalysisC6,ANAL_C7 AnalysisC7,ANAL_C8 AnalysisC8,ANAL_C9 AnalysisC9
                FROM SIADDANAL WHERE ADD_CODE = @CustomerCode AND DB_CODE = @DbCode";
            var param = new
            {
                CustomerCode = customerCode,
                DbCode = dbCode
            };
            var results = await _sqlDataAccess.LoadSingleData<SaleAnalysisDto,dynamic>(sql, param);
            return results;
        }
    }
}
