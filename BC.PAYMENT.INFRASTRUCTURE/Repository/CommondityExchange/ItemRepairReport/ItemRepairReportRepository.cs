namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CommondityExchange.ItemRepairReport;

public class ItemRepairReportRepository : IItemRepairReportRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public ItemRepairReportRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<ItemRepairReceivedDto>> GetItemRepairReceivedAsync(string dbCode, DateTime fromDate,
        DateTime toDate)
    {
        var sql =
            @"SELECT​ DISTINCT Date,CustomerCode,CustomerName,Market,Store,Area,ItemCode,Description,Quantity,Received,Seller,Status,(SELECT DB_NAME FROM SIDBINFO S WHERE S.DB_CODE = T.DB_CODE) BranchName FROM(
            SELECT HEADER.CREATED_DATE [Date],CUS.*,DETAIL.ITEM_CODE ItemCode,DETAIL.DESCRIPION [Description],DETAIL.QUANTITY [Quantity],
            RECEIVED.QUANTITY [Received],S.LAST_NAME + ' ' + S.FIRST_NAME [Seller],DETAIL.IS_RECEIVED [Status],HEADER.DB_CODE
            FROM TB_BC_CHANGEINVOICE HEADER INNER JOIN TB_BC_CHANGEINVOICE_DETAIL DETAIL ON DETAIL.CHANGE_INVOICE_ID = HEADER.ID
            LEFT JOIN TB_BC_CHANGEINVOICE_RECEIVED RECEIVED ON RECEIVED.CHANGE_INVOICE_DETAIL_ID = DETAIL.ID
            LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = HEADER.CUST_CODE
            INNER JOIN dbo.BCUSERS S ON S.USER_ID = HEADER.USER_CODE
            WHERE HEADER.DB_CODE = @DB_CODE AND CONVERT(DATE,HEADER.CREATED_DATE) BETWEEN @FROM_DATE AND @TO_DATE AND DETAIL.TYPE = 'REPAIR'
            UNION 
            SELECT [TRANSFER].CREATED_DATE,CUS.*,[TRANSFER].ITEM_CODE,[TRANSFER].DESCRIPTION,[TRANSFER].QUANTITY,RECEIVED.QUANTITY [Received],S.LAST_NAME + ' ' + S.FIRST_NAME [Seller],
            [TRANSFER].STATUS,[TRANSFER].DB_CODE
            FROM TB_BC_CHANGEINVOICE_REPAIR_TRANSFER [TRANSFER]
            LEFT JOIN TB_BC_CHANGEINVOICE_REPAIR_RECEIVED RECEIVED ON RECEIVED.TRAN_REF = [TRANSFER].TRAN_REF
            INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED R ON R.ID = [TRANSFER].RECEIVED_ID
            INNER JOIN TB_BC_CHANGEINVOICE_DETAIL DETAIL ON DETAIL.ID = R.CHANGE_INVOICE_DETAIL_ID
            INNER JOIN TB_BC_CHANGEINVOICE HEADER ON HEADER.ID = DETAIL.CHANGE_INVOICE_ID
            INNER JOIN BCUSERS S ON S.USER_ID = HEADER.USER_CODE
            LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = [TRANSFER].CUST_CODE
            WHERE [TRANSFER].TO_DB_CODE = @DB_CODE) T
            WHERE CONVERT(DATE,T.Date) BETWEEN @FROM_DATE AND @TO_DATE";
        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate,
            TO_DATE = toDate
        };
        var execute = await _sqlDataAccess.LoadData<ItemRepairReceivedDto, dynamic>(sql, param);
        return execute.ToList();
    }

    public async Task<List<ReportRepairItemInHandDto>> GetItemRepairSendToRepairerReportAsync(string dbCode)
    {
        var query =
            @"SELECT RECEIVED.CREATED_DATE [Date],CUS.*,RECEIVED.ITEM_CODE [ItemCode],RECEIVED.DESCRIPTION [Description],RECEIVED.QUANTITY [Quantity],S.LAST_NAME + ' ' +  S.FIRST_NAME [Seller],RECEIVED.STATUS [Status],(SELECT S.DB_NAME FROM SIDBINFO S WHERE S.DB_CODE = HEADER.DB_CODE) BranchName FROM TB_BC_CHANGEINVOICE HEADER INNER JOIN TB_BC_CHANGEINVOICE_DETAIL DETAIL ON DETAIL.CHANGE_INVOICE_ID = HEADER.ID
                INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED RECEIVED ON RECEIVED.CHANGE_INVOICE_DETAIL_ID = DETAIL.ID
                LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = HEADER.CUST_CODE
                INNER JOIN BCUSERS S ON S.USER_ID = HEADER.USER_CODE
                WHERE HEADER.DB_CODE = @DB_CODE AND RECEIVED.STATUS = 0 AND DETAIL.TYPE = 'REPAIR'
                UNION ALL
                -- Get from other branch
                SELECT RECEIVED.CREATED_DATE [Date],CUS.*,RECEIVED.ITEM_CODE [ItemCode],RECEIVED.DESCRIPTION [Description],RECEIVED.QUANTITY [Quantity],S.LAST_NAME + ' ' + S.FIRST_NAME [Seller],RECEIVED.STATUS [Status],(SELECT S.DB_NAME FROM SIDBINFO S WHERE S.DB_CODE = HEADER.DB_CODE) BranchName FROM TB_BC_CHANGEINVOICE HEADER INNER JOIN TB_BC_CHANGEINVOICE_DETAIL DETAIL ON DETAIL.CHANGE_INVOICE_ID = HEADER.ID
                INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED RECEIVED ON RECEIVED.CHANGE_INVOICE_DETAIL_ID = DETAIL.ID
                INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_TRANSFER [TRANSFER] ON [TRANSFER].RECEIVED_ID = RECEIVED.ID
                INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_RECEIVED REPAIR_REC ON REPAIR_REC.TRAN_REF = [TRANSFER].TRAN_REF
                INNER JOIN dbo.BCUSERS S ON S.USER_ID = HEADER.USER_CODE
                LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = HEADER.CUST_CODE
                WHERE TO_DB_CODE = @DB_CODE AND REPAIR_REC.STATUS = 'Pending' AND DETAIL.TYPE = 'REPAIR'";
        var param = new
        {
            DB_CODE = dbCode
        };
        var execute = await _sqlDataAccess.LoadData<ReportRepairItemInHandDto, dynamic>(query, param);
        return execute.ToList();
    }

    public async Task<List<ReportItemInRepairDto>> GetItemRepairingReportAsync(string dbCode)
    {
        var sql = @"SELECT T.*,(SELECT S.DB_NAME FROM SIDBINFO S WHERE S.DB_CODE = T.DbCode) BranchName FROM(
                SELECT CASE WHEN REPAIR.IS_RECEIVED = 'Yes' THEN REPAIR.RECEIVED_DATE ELSE REPAIR.CREATED_DATE END [Date],CUS.*,'B16' DbCode, 
                D.ITEM_CODE ItemCode, REPAIR.TRAN_REF ItemTransaction, D.DESCRIPION Description, REPAIR.QUANTITY Quantity, REPAIR.IS_RECEIVED Status, Sale
                                FROM TB_BC_CHANGEINVOICE M INNER JOIN TB_BC_CHANGEINVOICE_DETAIL D ON D.CHANGE_INVOICE_ID = M.ID
                                    INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED R ON R.CHANGE_INVOICE_DETAIL_ID = D.ID
                                    INNER JOIN TB_BC_CHANGEINVOICE_REPAIR REPAIR ON REPAIR.RECEIVED_ID = R.ID                    
					                LEFT JOIN (SELECT S.ADD_CODE CustomerCode, STORE Store, S.ADD_LINE_1KH CustomerName, M.MARKET_KHMER_NAME Market, A.AREA_NAME_KHMER Area
                                    FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                                    WHERE S.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE
                                            ) CUS ON CUS.CustomerCode = M.CUST_CODE
                                    INNER JOIN (SELECT LAST_NAME +' '+ FIRST_NAME Sale, USER_ID Id
                                    FROM dbo.BCUSERS) USERS ON USERS.Id = M.USER_CODE
                                WHERE M.DB_CODE = @DB_CODE AND REPAIR.IS_RECEIVED <> 'Completed'
                UNION ALL
		                 SELECT CASE WHEN REPAIR.IS_RECEIVED = 'Yes' THEN REPAIR.RECEIVED_DATE ELSE REPAIR.CREATED_DATE END [Date], 
		                 CUS.*,SUBSTRING(TRAN_TYPE,1,3) DbCode, D.ITEM_CODE ItemCode, REPAIR.TRAN_REF, D.DESCRIPION Description,
                                    REPAIR.QUANTITY Quantity, REPAIR.IS_RECEIVED StatusRepair, Sale
                                FROM TB_BC_CHANGEINVOICE M INNER JOIN TB_BC_CHANGEINVOICE_DETAIL D ON D.CHANGE_INVOICE_ID = M.ID
                                    INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED R ON R.CHANGE_INVOICE_DETAIL_ID = D.ID
                                    INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_TRANSFER [TRANSFER] ON [TRANSFER].RECEIVED_ID = R.ID
                                    INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_RECEIVED [RECEIVED] ON [RECEIVED].TRAN_REF = [TRANSFER].TRAN_REF
                                    INNER JOIN TB_BC_CHANGEINVOICE_REPAIR REPAIR ON REPAIR.TRAN_REF = [RECEIVED].TRAN_REF
                                    LEFT JOIN (SELECT S.ADD_CODE CustomerCode, STORE Store, S.ADD_LINE_1KH CustomerName, M.MARKET_KHMER_NAME Market, A.AREA_NAME_KHMER Area
                                    FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                                            ) CUS ON CUS.CustomerCode = M.CUST_CODE
                                    INNER JOIN (SELECT LAST_NAME +' '+ FIRST_NAME Sale, USER_ID Id
                                    FROM dbo.BCUSERS) USERS ON USERS.Id = M.USER_CODE WHERE REPAIR.IS_RECEIVED <> 'Completed' AND REPAIR.DB_CODE = @DB_CODE) T WHERE T.ItemTransaction IS NOT NULL";
        var param = new
        {
            DB_CODE = dbCode
        };
        var execute = await _sqlDataAccess.LoadData<ReportItemInRepairDto, dynamic>(sql, param);
        return execute.ToList();
    }

    public async Task<List<ItemRepairAnalysis>> GetItemRepairReportAsync(string dbCode)
    {
        var sql =
            @"SELECT CONVERT(DATE,I.CREATED_DATE) Date,ITEM_CODE ItemCode,DESCRIPION Description,SUM(D.QUANTITY) Quantity 
                FROM TB_BC_CHANGEINVOICE_DETAIL D INNER JOIN TB_BC_CHANGEINVOICE I ON I.ID = D.CHANGE_INVOICE_ID WHERE TYPE= 'REPAIR'
                AND I.DB_CODE = @DB_CODE
                GROUP BY CREATED_DATE,DESCRIPION,ITEM_CODE
                ORDER BY DESCRIPION,Date";
        var param = new
        {
            DB_CODE = dbCode
        };
        var execute = await _sqlDataAccess.LoadData<ItemRepairAnalysis, dynamic>(sql, param);
        return execute.ToList();
    }
}