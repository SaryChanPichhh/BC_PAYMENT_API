namespace BC.PAYMENT.INFRASTRUCTURE.Repository.General;

public class ProductRepository : IProductRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public ProductRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<ProductModel>> GetAllProductsAsync(string dbCode)
    {
        const string sql =
            @"SELECT ITEM_CODE ItemCode, ITEM_CUS10_KH ItemDesc, ITEM_PRICE1 Price FROM dbo.SIITEMS WHERE ITEM_STAT = @ITEM_STAT AND DB_CODE = @DB_CODE";
        var param = new
        {
            ITEM_STAT = "A",
            DB_CODE = dbCode
        };
        var execute = await _sqlDataAccess.LoadData<ProductModel, dynamic>(sql, param);
        return execute.ToList();
    }
}