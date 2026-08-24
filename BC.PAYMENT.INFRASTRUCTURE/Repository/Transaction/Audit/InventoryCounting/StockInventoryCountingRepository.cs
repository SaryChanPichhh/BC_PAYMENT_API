namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.Audit.InventoryCounting
{
    public class StockInventoryCountingRepository : IStockInventoryCountingRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IDbConnection _dbConnection;

        public StockInventoryCountingRepository(ISqlDataAccess sqlDataAccess, IDbConnection dbConnection)
        {
            _sqlDataAccess = sqlDataAccess;
            _dbConnection = dbConnection;
        }

        public async Task<List<StockInventoryCountingModel>> GetInventoryCountingAsync(string dbCode)
        {
            var sql = $@"SELECT ST.STOCK_ID StockId,ST.STOCK_NAME Stock,ST.WAREHOUSE Warehouse
                    ,ST.STOCK_CONTROLLER StockController,ST.PARTICIPATION Participation,ST.PERION Period,ST.COUNT_DATE CountingDate
                 FROM SC_STOCK ST WHERE ST.DB_CODE = @DB_CODE AND ST.STATUS = 1";
            var param = new
            {
                DB_CODE = dbCode
            };
            var execute = await _sqlDataAccess.LoadData<StockInventoryCountingModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> InsertInventoryCountingAsync(StockInventoryCountingModel model)
        {
            var sql = $@"INSERT INTO SC_STOCK (DB_CODE,STOCK_NAME,WAREHOUSE,STOCK_CONTROLLER,PARTICIPATION,PERION,COUNT_DATE,STATUS,CREATE_BY) 
                        VALUES(@DB_CODE, @STOCKNAME, @WARHOUSE, @STOCK_CONTROLLER, @PARTICIPATION, @PERIOD, @COUNT_DATE, 1, @CREATE_BY)";
            var param = new
            {
                DB_CODE = model.DbCode,
                STOCKNAME = model.Stock,
                WARHOUSE = model.Warehouse,
                STOCK_CONTROLLER = model.StockController,
                PARTICIPATION = model.Participation,
                PERIOD = model.Period,
                COUNT_DATE = model.CountingDate,
                CREATE_BY = model.CreateBy
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<int> UpdateInventoryCountingAsync(StockInventoryCountingModel model)
        {
            var sql =
                $@"UPDATE SC_STOCK SET STOCK_NAME = @STOCKNAME,WAREHOUSE = @WARHOUSE,STOCK_CONTROLLER = @STOCK_CONTROLLER,PARTICIPATION = @PARTICIPATION,
                        PERION = @PERIOD,UPDATE_BY = @UPDATE_BY,UPDATE_DATE = @UPDATE_DATE
                          WHERE STOCK_ID = @ID AND DB_CODE = @DB_CODE";
            var param = new
            {
                STOCKNAME = model.Stock,
                WARHOUSE = model.Warehouse,
                STOCK_CONTROLLER = model.StockController,
                PARTICIPATION = model.Participation,
                PERIOD = model.Period,
                UPDATE_BY = model.CreateBy,
                UPDATE_DATE = DateTime.Now,
                ID = model.StockId,
                DB_CODE = model.DbCode
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<int> DeleteInventoryCountingAsync(string code)
        {
            var deleteStock = $@"DELETE FROM SC_STOCK WHERE STOCK_ID = @STOCK_ID";
            var param = new
            {
                STOCK_ID = code
            };
            var deletedStockAffectedRow = await _sqlDataAccess.ExecuteAsync(deleteStock, param);
            return deletedStockAffectedRow;
        }
    }
}
