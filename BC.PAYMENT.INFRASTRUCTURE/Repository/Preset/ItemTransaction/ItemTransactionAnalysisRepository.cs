namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Preset.ItemTransaction;

public class ItemTransactionAnalysisRepository : IItemTransactionAnalysisRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public ItemTransactionAnalysisRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<ItemTransactionAnalysisModel>> GetItemTransactionAsync(int fromMovPrd, int toMovPrd,
        string itemCode)
    {
        var procedure =
            $@"SI_PO_EACH_BRANCH ";
        var condition =
            @$" WHERE INV_PRD BETWEEN '{fromMovPrd}' AND '{toMovPrd}' AND ITEM_CODE = '{itemCode}'";

        var param = new
        {
            @Condition = condition
        };
        var result = await
            _sqlDataAccess.LoadData<ItemTransactionAnalysisModel, dynamic>(procedure, param,
                CommandType.StoredProcedure);
        return result.ToList();
    }

    public async Task<List<string>> GetAllItemByPeriodAsync(string DbCode, int fromMovPrd, int toMovPrd)
    {
        var sql =
            @$"SELECT DISTINCT D.ITEM_CODE ItemCode,D.ITEM_DESC DESCRIPTN FROM {DbCode}SIPOINV H INNER JOIN {DbCode}SIPODET D ON H.INV_REF = D.INV_REF WHERE H.INV_PRD BETWEEN @FROM_MOV AND @TO_MOV
                        --AND D.ITEM_CODE = '1HC772'";
        var param = new
        {
            FROM_MOV = fromMovPrd,
            TO_MOV = toMovPrd
        };
        var result = await _sqlDataAccess.LoadData<string, dynamic>(sql, param);
        return result.ToList();
    }

    public async Task<List<string>> GetItemByPeriodAndItemsAsync(string DbCode, int fromMovPrd, int toMovPrd,
        List<string> lsItem)
    {
        var itemCodes = string.Join(", ", lsItem.Select(item => $"'{item}'"));
        var condition = string.Empty;
        if (lsItem.Count > 0) condition = @$"AND D.ITEM_CODE IN ({itemCodes})";
        var sql =
            @$"SELECT DISTINCT D.ITEM_CODE ItemCode FROM {DbCode}SIPOINV H INNER JOIN {DbCode}SIPODET D ON H.INV_REF = D.INV_REF WHERE H.INV_PRD BETWEEN @FROM_MOV AND @TO_MOV
                         {condition}";
        var param = new
        {
            FROM_MOV = fromMovPrd,
            TO_MOV = toMovPrd
        };
        var result = await _sqlDataAccess.LoadData<string, dynamic>(sql, param);
        return result.ToList();
    }
}