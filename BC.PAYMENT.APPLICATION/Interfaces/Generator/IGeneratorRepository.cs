using BC.PAYMENT.CORE.DTO.Generator;

namespace BC.PAYMENT.APPLICATION.Interfaces.Generator
{
    public interface IGeneratorRepository
    {
        Task<SaleAnalysisDto> GetSaleAnalysisByCustomerCodeAsync(string customerCode, string dbCode);
        Task<string> GenerateAdjRefCode(string movType, string recType);
    }
}
