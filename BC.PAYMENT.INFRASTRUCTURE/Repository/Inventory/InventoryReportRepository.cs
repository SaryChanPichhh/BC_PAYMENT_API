namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Inventory
{
    public class InventoryReportRepository : IInventoryReportRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public InventoryReportRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<InventoryReportModel>> GetAllStatusProductsSaleByDateAsync(InventoryReportRequestDto.RequestByDateDto model)
        {
            var sql = $@"SELECT ITEM_CODE ItemCode, SUM(POST) POST, SUM(HOLD) HOLD, SUM(RELEASE) RELEASE, SUM(TOTAL) TOTAL,TAB.TRANS_CODE
                 FROM(SELECT ITEM_CODE, POST, HOLD, RELEASE, (ISNULL(POST, 0) + ISNULL(HOLD, 0) + ISNULL(RELEASE, 0)) TOTAL,TAB1.TRANS_CODE
                FROM(SELECT DETAIL.ITEM_CODE,ISNULL(CASE WHEN HEADER.STATUS IN('80', '85') THEN SUM(VALUE_1) END, 0) 'POST',HEADER.TRANS_CODE,
                ISNULL(CASE WHEN HEADER.STATUS = '00' THEN SUM(VALUE_1) END, 0)'HOLD',
                ISNULL(CASE WHEN HEADER.STATUS = '10' THEN SUM(VALUE_1) END, 0)'RELEASE'
                FROM {model.DbCode}SISOHDR HEADER
                INNER JOIN {model.DbCode}SISODET DETAIL ON HEADER.TRANS_REF = DETAIL.TRANS_REF
                WHERE HEADER.ORDER_DATE BETWEEN @FROMDATE AND @TODATE
                AND DETAIL.TRANS_TYPE IN @TRANS_TYPE
                AND DETAIL.CREDIT_STATUS = ''
                GROUP BY ITEM_CODE, HEADER.STATUS,HEADER.TRANS_CODE) TAB1
                GROUP BY ITEM_CODE, POST, HOLD, RELEASE,TAB1.TRANS_CODE
                UNION ALL
                SELECT ITEM_CODE, '0', '0', '0', SUM(QUANTITY) , '' TRANS_CODE
                FROM {model.DbCode}SIINVMOV
                WHERE MOV_DATE BETWEEN @FROMDATE AND @TODATE
                AND LOCATION = @LOCATION
                AND IR_STAT = 'I'
                AND REC_TYPE = 'T'
                GROUP BY ITEM_CODE, MOV_DATE) TAB
                GROUP BY ITEM_CODE,TAB.TRANS_CODE
                ORDER BY ITEM_CODE";
            var param = new
            {
                LOCATION = model.Location,
                FROMDATE =Convert.ToDateTime(model.FromDate),
                TODATE = Convert.ToDateTime( model.ToDate),
                TRANS_TYPE = model.TranTypes
            };
            var execute = await _sqlDataAccess.LoadData<InventoryReportModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<InventoryReportModel>> GetAllStatusProductsSaleByPeriodAsync(InventoryReportRequestDto.RequestByPeriodDto model)
        {
            var sql = $@"SELECT ITEM_CODE ItemCode,POST,HOLD,RELEASE,(ISNULL(POST,0)+ISNULL(HOLD,0)+ISNULL(RELEASE,0)) TOTAL FROM (
                SELECT DETAIL.ITEM_CODE,
                CASE WHEN HEADER.STATUS IN('80', '85') THEN SUM(VALUE_1) END 'POST',
                CASE WHEN HEADER.STATUS = '00' THEN SUM(VALUE_1) END          'HOLD',
                CASE WHEN HEADER.STATUS = '10' THEN SUM(VALUE_1) END          'RELEASE'
                FROM B16SISOHDR HEADER
                INNER JOIN B16SISODET DETAIL ON HEADER.TRANS_REF = DETAIL.TRANS_REF
                WHERE HEADER.INV_PRD BETWEEN @FROM_PERIOD AND @TO_PERIOD
                AND DETAIL.TRANS_TYPE IN @TRANS_TYPE
                AND DETAIL.CREDIT_STATUS = ''
                GROUP BY ITEM_CODE, HEADER.STATUS) TAB1 GROUP BY ITEM_CODE, POST, HOLD, RELEASE
                UNION ALL
                SELECT ITEM_CODE, '0', '0', '0', SUM(QUANTITY)
                FROM B16SIINVMOV
                WHERE MOV_PRD BETWEEN @FROM_PERIOD AND @TO_PERIOD
                AND LOCATION = @LOCATION
                AND IR_STAT = 'I'
                AND REC_TYPE = 'T'
                GROUP BY ITEM_CODE, MOV_DATE";
            var param = new
            {
                LOCATION = model.Location,
                FROM_PERIOD = model.FromPeriod,
                TO_PERIOD = model.ToPeriod,
                TRANS_TYPE = model.TranTypes
            };
            var execute = await _sqlDataAccess.LoadData<InventoryReportModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<InventoryReportModel>> GetAllStatusProductsSaleByDateWithOutSaleFixAsync(InventoryReportRequestDto.RequestByDateDto model)
        {
            var sql = $@"SELECT ITEM_CODE ItemCode, SUM(POST) POST, SUM(HOLD) HOLD, SUM(RELEASE) RELEASE, SUM(TOTAL) TOTAL,TAB.TRANS_CODE
                 FROM(SELECT ITEM_CODE, POST, HOLD, RELEASE, (ISNULL(POST, 0) + ISNULL(HOLD, 0) + ISNULL(RELEASE, 0)) TOTAL,TAB1.TRANS_CODE
                FROM(SELECT DETAIL.ITEM_CODE,ISNULL(CASE WHEN HEADER.STATUS IN('80', '85') THEN SUM(VALUE_1) END, 0) 'POST',HEADER.TRANS_CODE,
                ISNULL(CASE WHEN HEADER.STATUS = '00' THEN SUM(VALUE_1) END, 0)'HOLD',
                ISNULL(CASE WHEN HEADER.STATUS = '10' THEN SUM(VALUE_1) END, 0)'RELEASE'
                FROM {model.DbCode}SISOHDR HEADER
                INNER JOIN {model.DbCode}SISODET DETAIL ON HEADER.TRANS_REF = DETAIL.TRANS_REF
                WHERE HEADER.ORDER_DATE BETWEEN @FROMDATE AND @TODATE
                AND DETAIL.TRANS_TYPE IN @TRANS_TYPE
                AND DETAIL.CREDIT_STATUS = '' AND DETAIL.TRANS_TYPE != 'SALE-FIX'
                GROUP BY ITEM_CODE, HEADER.STATUS,HEADER.TRANS_CODE) TAB1
                GROUP BY ITEM_CODE, POST, HOLD, RELEASE,TAB1.TRANS_CODE
                UNION ALL
                SELECT ITEM_CODE, '0', '0', '0', SUM(QUANTITY) , '' TRANS_CODE
                FROM {model.DbCode}SIINVMOV
                WHERE MOV_DATE BETWEEN @FROMDATE AND @TODATE
                AND LOCATION = @LOCATION
                AND IR_STAT = 'I'
                AND REC_TYPE = 'T'
                GROUP BY ITEM_CODE, MOV_DATE) TAB
                GROUP BY ITEM_CODE,TAB.TRANS_CODE
                ORDER BY ITEM_CODE";
            var param = new
            {
                LOCATION = model.Location,
                FROMDATE = Convert.ToDateTime(model.FromDate),
                TODATE = Convert.ToDateTime(model.ToDate),
                TRANS_TYPE = model.TranTypes
            };
            var execute = await _sqlDataAccess.LoadData<InventoryReportModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<List<InventoryReportModel>> GetAllStatusProductsSaleByPeriodWithOutSaleFixAsync(InventoryReportRequestDto.RequestByPeriodDto model)
        {
            var sql = $@"SELECT ITEM_CODE,POST,HOLD,RELEASE,(ISNULL(POST,0)+ISNULL(HOLD,0)+ISNULL(RELEASE,0)) TOTAL FROM (
                SELECT DETAIL.ITEM_CODE,
                CASE WHEN HEADER.STATUS IN('80', '85') THEN SUM(VALUE_1) END 'POST',
                CASE WHEN HEADER.STATUS = '00' THEN SUM(VALUE_1) END          'HOLD',
                CASE WHEN HEADER.STATUS = '10' THEN SUM(VALUE_1) END          'RELEASE'
                FROM B16SISOHDR HEADER
                INNER JOIN B16SISODET DETAIL ON HEADER.TRANS_REF = DETAIL.TRANS_REF
                WHERE HEADER.INV_PRD BETWEEN @FROM_PERIOD AND @TO_PERIOD
                AND DETAIL.TRANS_TYPE IN @TRANS_TYPE
                AND DETAIL.CREDIT_STATUS = '' AND DETAIL.TRANS_TYPE != 'SALE-FIX'
                GROUP BY ITEM_CODE, HEADER.STATUS) TAB1 GROUP BY ITEM_CODE, POST, HOLD, RELEASE
                UNION ALL
                SELECT ITEM_CODE, '0', '0', '0', SUM(QUANTITY)
                FROM B16SIINVMOV
                WHERE MOV_PRD BETWEEN @FROM_PERIOD AND @TO_PERIOD
                AND LOCATION = @LOCATION
                AND IR_STAT = 'I'
                AND REC_TYPE = 'T'
                GROUP BY ITEM_CODE, MOV_DATE";
            var param = new
            {
                LOCATION = model.Location,
                FROM_PERIOD = model.FromPeriod,
                TO_PERIOD = model.ToPeriod,
                TRANS_TYPE = model.TranTypes
            };
            var execute = await _sqlDataAccess.LoadData<InventoryReportModel, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> AddWarehouseData(List<InventoryTrackingWarehouseDataModel> model)
        {
            const string sql =
                @"INSERT INTO DT_INVENTORY_TRACKING_WAREHOUSE_DATA(DB_CODE,TRACKING_TYPE,WAREHOUSE,ITEM_CODE,ITEM_DESC,QUANTITY,CREATED_DATE,CREATED_BY,TRANS_DATE,REFERENCE,STATUS)
            VALUES(@DB_CODE,@TRACKING_TYPE,@WAREHOUSE,@ITEM_CODE,@ITEM_DESC,@QUANTITY,@CREATED_DATE,@CREATED_BY,@TRANSACTION_DATE,@REFERENCE,@STATUS)";
            var rowAffected = 0;
            foreach (var dataModel in model)
            {
                var stockType = dataModel.InventoryTrackingTypes == InventoryTrackingTypes.Plus ? "Plus" : "Subtract";
                var quantity = dataModel.InventoryTrackingTypes == InventoryTrackingTypes.Subtract
                    ? dataModel.Quantity *= -1
                    : dataModel.Quantity;
                var paramDetails = new
                {
                    DB_CODE = dataModel.DbCode,
                    TRACKING_TYPE = stockType,
                    WAREHOUSE = dataModel.Warehouse,
                    ITEM_CODE = dataModel.ItemCode,
                    ITEM_DESC = dataModel.ItemDescription,
                    QUANTITY = quantity,
                    CREATED_DATE = dataModel.CreatedDate,
                    CREATED_BY = dataModel.CreatedBy,
                    TRANSACTION_DATE = dataModel.TransactionDate,
                    REFERENCE = dataModel.Warehouse,
                    STATUS = "E"
                };
                rowAffected += await _sqlDataAccess.ExecuteAsync(sql, paramDetails);
            }
            return rowAffected;
        }

        public async Task<List<InventoryReceiveModel>> GetInventoryReceiveAsync(DateTime fromDate, DateTime toDate)
        {
            const string sql = @"SELECT 
                                    Id,RowNumber, DbCode, WareDesc, SupplierCode, WareCode, OrderedBy, OrderedDate, Completed, ItemCode, 
                                    OrderedQty, ReceivedQty, ReceivedDate, ReceivedBy 
                                FROM 
                                (
                                    SELECT 
                                        ROW_NUMBER() OVER (PARTITION BY R.COMPANY_PO_DETAIL_ID ORDER BY R.CREATED_DATE) AS RowNumber, 
                                        H.ID Id,
                                        H.DB_CODE AS DbCode, 
                                        H.DESCRIPTION AS WareDesc, 
                                        H.SUPPLIER_CODE AS SupplierCode, 
                                        H.WAREHOUSE AS WareCode, 
                                        H.ORDER_BY AS OrderedBy, 
                                        CAST(H.CREATED_DATE AS DATE ) AS OrderedDate, 
                                        H.COMPLETED AS Completed,
                                        D.ITEM_CODE AS ItemCode, 
                                        D.ORDER_QTY AS OrderedQty, 
                                        R.RECEIVED_QTY AS ReceivedQty, 
                                        R.CREATED_DATE AS ReceivedDate,
                                        R.CREATED_BY AS ReceivedBy 
                                    FROM 
                                        dbo.MB_COMPANY_PO H 
                                    INNER JOIN 
                                        dbo.MB_COMPANY_PO_DETAIL D ON D.ID = H.ID
                                    INNER JOIN 
                                        dbo.MB_COMPANY_PO_DETAIL_RECEIVED R ON R.COMPANY_PO_DETAIL_ID = D.DID
                                    WHERE 
                                        H.DB_CODE = 'B16' 
                                        AND H.STATUS = 1 
                                        AND H.CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE
                                ) AS MainQuery
                                ";
            var param = new { FROM_DATE = fromDate, TO_DATE = toDate };
            var result = await _sqlDataAccess.LoadData<InventoryReceiveModel, dynamic>(sql, param);
            return result.ToList();
        }
    }
}
