namespace BC.PAYMENT.APPLICATION.Interfaces.Preset.AnnualPurchase;

public interface IAnnualPurchaseRepository
{
    Task<List<AnnualPurchaseModel>> GetSalesReportListAsync(string dbCode, List<int> year, int page, int pageSize);

    Task<List<AnnualPurchaseModel>> GetAllReportSalesPerYearsAsync(string dbCode,
        List<string> marketCode, List<string> customerCode,
        SaleTypeDto reportSalesPerYearRequestDto, params List<int>[] year);

    Task<List<string>> SaleCodesAsync(string dbCode);


    Task<List<DailySaleReportValueDetailsModel>> GetDailySaleReportDetailsValueAsync(DateTime date,
        string branchCode);

    Task<List<DailySaleReportValueModel>> GetDailySaleReportValueAsync(DateTime date,
        Dictionary<string, string> branches);
}