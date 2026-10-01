namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Preset.DailySaleAnalysis;

public class DailyAnalysisRepository : IDailyAnalysisRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public DailyAnalysisRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<DailySaleAnalysisModel>> GetDailySaleAnalysisAsync(string fromPrd, string endPrd,
        string itemCode = null)
    {
        var procedure =
            $@"dbo.SALE_DAILY_EACH_BRANCH ";
        var condition = itemCode == null
            ? @$" WHERE INV_PRD BETWEEN '{fromPrd}' AND '{endPrd}' "
            : @$" WHERE ITEM_CODE LIKE '%{itemCode}%' AND INV_PRD BETWEEN '{fromPrd}' AND '{endPrd} ";
        var param = new
        {
            Condition = condition
        };
        var result = await
            _sqlDataAccess.LoadData<DailySaleAnalysisModel, dynamic>(procedure, param,
                CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<List<DailySaleAnalysisModel>> GetDailySaleAnalysisByItemCodeAndDateAsync(DateTime fromDate,
        DateTime endDate, List<string> itemCode)
    {
        var itemCodes = string.Join(", ", itemCode.Select(date => $"'{date}'"));

        var procedure =
            $@"dbo.SALE_DAILY_EACH_BRANCH ";
        var condition = itemCode == null || itemCode.Count == 0
            ? @$" WHERE TRY_CONVERT(DATETIME, INV_DATE, 101) BETWEEN '{fromDate:yyyy-MM-dd}' AND '{endDate:yyyy-MM-dd}'  "
            : @$" WHERE ITEM_CODE IN ({itemCodes}) AND TRY_CONVERT(DATETIME, INV_DATE, 101) BETWEEN '{fromDate:yyyy-MM-dd}' AND '{endDate:yyyy-MM-dd}'  ";
        var param = new
        {
            Condition = condition
        };
        //System.Windows.MessageBox.Show(param.ToString());
        var result = await
            _sqlDataAccess.LoadData<DailySaleAnalysisModel, dynamic>(procedure, param,
                CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<List<DailySaleAnalysisModel>> GetDailySaleAnalysisByItemCodeAndPeriodAsync(string fromPrd,
        string endPrd, List<string> itemCode)
    {
        var itemCodes = string.Join(", ", itemCode.Select(date => $"'{date}'"));
        var procedure =
            $@"dbo.SALE_DAILY_EACH_BRANCH ";
        var condition = itemCode == null || itemCode.Count == 0
            ? @$" WHERE INV_PRD BETWEEN '{fromPrd}' AND '{endPrd}'"
            : @$" WHERE ITEM_CODE IN ({itemCodes}) AND INV_PRD BETWEEN '{fromPrd}' AND '{endPrd}' ";
        var param = new
        {
            Condition = condition
        };
        var result = await
            _sqlDataAccess.LoadData<DailySaleAnalysisModel, dynamic>(procedure, param,
                CommandType.StoredProcedure);
        return result.ToList();
    }
}