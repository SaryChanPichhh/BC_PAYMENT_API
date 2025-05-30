using BC.PAYMENT.CORE.DTO.Generator;

namespace BC.PAYMENT.APPLICATION.Interfaces.Generator
{
    public interface IGeneratorRepository
    {
        Task<SaleAnalysisDto> GetSaleAnalysisByCustomerCodeAsync(string customerCode, string dbCode);
        Task<string> GenerateAdjRefCode(string dbCode, string movType, string recType);
        Task<string> GenerateFixInvoice(string dbCode);
        Task<string> PostSaleOrderAutoNumberAsync(string saleType, string dbCode);
        Task<List<string>> GetSaleCodeAsync(string dbCode);
    }
}
