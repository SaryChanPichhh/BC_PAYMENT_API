namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.DeliveryPaid;

public class DeliveryPaidRepository : IDeliveryPaidRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;
    private readonly IConfiguration _configuration;
    private readonly IDbConnection _dbConnection;

    public DeliveryPaidRepository(ISqlDataAccess sqlDataAccess, IConfiguration configuration,
        IDbConnection dbConnection)
    {
        _sqlDataAccess = sqlDataAccess;
        _configuration = configuration;
        _dbConnection = dbConnection;
    }

    public async Task<List<GeneralInvoicePaymentModel>> GetAllInvoiceByDeliveryIdAndDate(string dbCode,
        string deliveryId, DateTime date)
    {
        var sql =
            @"SELECT D.DIVIDED_INVOICE_ID dividedInvoiceId,D.CREATE_DATE Date,Store,DL.DELIVERIES_KHMER Delivery,D.DIVIDED_INVOICE_ID Id, N.CUSTOMER_CODE CustomerCode,CUST.CUSTOMER_NAME CustomerName,
                MARKET Market,AREA Area,N.TRANSACTION_REF TransactionCode,N.HEADER_TRANSACTION_VALUES InvoiceValue,CONVERT(BIT,CASE WHEN R.RETURN_ID IS NOT NULL THEN 1 ELSE 0 END) IsReturn,
                CONVERT(BIT,CASE WHEN P.PAYMENT_ID IS NOT NULL THEN 1 ELSE 0 END) IsPaid,R.[DESCRIPTION] Description,P.AMOUNT PaidAmount,N.[STATUS] Status
                FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
                    LEFT JOIN PC_PAYMENT_INVOICE P ON P.DIVDIE_INVOICE_ID = D.DIVIDED_INVOICE_ID
                    LEFT JOIN PC_RETURN_INVOICE R ON R.DIVIDED_INIOVICE_ID = D.DIVIDED_INVOICE_ID
                    LEFT JOIN (SELECT ADD_CODE CUSTOMER_CODE,STORE Store, ADD_LINE_1KH CUSTOMER_NAME, MARKET_KHMER_NAME MARKET, AREA_NAME_KHMER AREA
                    FROM SIADD S INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                        INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID
                    WHERE S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE) CUST ON CUST.CUSTOMER_CODE = N.CUSTOMER_CODE
                    INNER JOIN TB_BCDELIVERIES DL ON DL.DELIVERIES_ID = D.DELIVERY_ID
                    WHERE N.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE --AND DL.DB_CODE = @DB_CODE
                    AND D.CREATE_DATE = @DATE AND DL.DELIVERIES_ID = @DELIVERY_ID";

        var param = new
        {
            DB_CODE = dbCode,
            DELIVERY_ID = deliveryId,
            DATE = date.ToString("yyyy-MM-dd")
        };

        var execute = await _sqlDataAccess.LoadData<GeneralInvoicePaymentModel, dynamic>(sql, param);
        return execute.ToList();
    }

    public async Task<List<DeliveryDataObject>> GetAllInvoiceByDeliveryIdAndDateDataObjectAsync(string dbCode,
        string deliveryId, DateTime date)
    {
        var deliveryDict = new Dictionary<string, DeliveryDataObject>();
        var sql =
            @"SELECT DL.DELIVERIES_ID DeliveryId,DL.DELIVERIES_KHMER deliveryName,D.DELIVERY_ID DeliveryId,D.DIVIDED_INVOICE_ID dividedInvoiceId,D.CREATE_DATE Date,Store,D.DIVIDED_INVOICE_ID Id, N.CUSTOMER_CODE CustomerCode,CUST.CUSTOMER_NAME CustomerName,
                MARKET Market,AREA Area,N.TRANSACTION_REF TransactionCode,N.HEADER_TRANSACTION_VALUES InvoiceValue,CONVERT(BIT,CASE WHEN R.RETURN_ID IS NOT NULL THEN 1 ELSE 0 END) IsReturn,
                CONVERT(BIT,CASE WHEN P.PAYMENT_ID IS NOT NULL THEN 1 ELSE 0 END) IsPaid,R.[DESCRIPTION] Description,P.AMOUNT PaidAmount,N.[STATUS] Status
                FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
                    LEFT JOIN PC_PAYMENT_INVOICE P ON P.DIVDIE_INVOICE_ID = D.DIVIDED_INVOICE_ID
                    LEFT JOIN PC_RETURN_INVOICE R ON R.DIVIDED_INIOVICE_ID = D.DIVIDED_INVOICE_ID
                    LEFT JOIN (SELECT ADD_CODE CUSTOMER_CODE,STORE Store, ADD_LINE_1KH CUSTOMER_NAME, MARKET_KHMER_NAME MARKET, AREA_NAME_KHMER AREA
                    FROM SIADD S INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                        INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID
                    WHERE S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE) CUST ON CUST.CUSTOMER_CODE = N.CUSTOMER_CODE
                    INNER JOIN TB_BCDELIVERIES DL ON DL.DELIVERIES_ID = D.DELIVERY_ID
                    WHERE N.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE --AND DL.DB_CODE = @DB_CODE
                    AND D.CREATE_DATE = @DATE AND DL.DELIVERIES_ID = @DELIVERY_ID";

        var param = new
        {
            DB_CODE = dbCode,
            DELIVERY_ID = deliveryId,
            DATE = date.ToString("yyyy-MM-dd")
        };
        var query = _dbConnection.ConnectionString;
        var execute =
            await _dbConnection.QueryAsync<DeliveryDataObject, GeneralInvoicePaymentModel, DeliveryDataObject>(sql,
                (delivery, invoice) =>
                {
                    if (!deliveryDict.TryGetValue(delivery.DeliveryId!, out var deliveryData))
                    {
                        deliveryData = delivery;
                        deliveryDict.Add(delivery.DeliveryId!, deliveryData);
                    }

                    if (invoice != null) deliveryData.InvoicePayments.Add(invoice);
                    return deliveryData;
                }, param, splitOn: "DeliveryId");
        return execute.Distinct().ToList();
    }

    public async Task<List<GeneralInvoicePaymentModel>> LoadInvoicePaid(string dbCode, string deliveryId, DateTime date)
    {
        var sql = $@"SELECT
                              P.INVOICE_ID,P.DIVIDED_INVOICE_ID,N.TRANSACTION_REF, N.CUSTOMER_CODE, N.ACC_NAME_KH, CASE WHEN N.STATUS = 'O' THEN '1.00' END 'OLD', CASE WHEN N.STATUS = 'N' THEN '1.00' END 'NEW', CASE WHEN N.STATUS = 'C' THEN '1.00' END 'CHANGE',
                              N.HEADER_TRANSACTION_VALUES, CONVERT(BIT, 0)[RETURN], '' OTHER, PAID.AMOUNT, N.HEADER_TRANSACTION_VALUES - PAID.AMOUNT TOTAL, CONVERT(BIT, 1) PAID
                              FROM NEW_INVOICE N
                              INNER JOIN PC_DIVIDED_INVOICE P
                              ON P.INVOICE_ID = N.ID
                              INNER JOIN PC_PAYMENT_INVOICE PAID ON PAID.DIVDIE_INVOICE_ID = P.DIVIDED_INVOICE_ID
                              WHERE N.DB_CODE = @DB_CODE AND P.DB_CODE = @DB_CODE AND P.DELIVERY_ID = @DELIVERYID AND PAID.DB_CODE = @DB_CODE AND P.CREATE_DATE = @DIVIDED_DATE
                              UNION ALL
                              SELECT
                              P.INVOICE_ID,P.DIVIDED_INVOICE_ID,N.TRANSACTION_REF, N.CUSTOMER_CODE, N.ACC_NAME_KH, CASE WHEN N.STATUS = 'O' THEN '1.00' END 'OLD', CASE WHEN N.STATUS = 'N' THEN '1.00' END 'NEW', CASE WHEN N.STATUS = 'C' THEN '1.00' END 'CHANGE',
                              N.HEADER_TRANSACTION_VALUES, CONVERT(BIT, 1)[RETURN], R.DESCRIPTION, 0, 0, CONVERT(BIT, 0) PAID
                              FROM NEW_INVOICE N
                              INNER JOIN PC_DIVIDED_INVOICE P
                              ON P.INVOICE_ID = N.ID
                              INNER JOIN PC_RETURN_INVOICE R ON R.DIVIDED_INIOVICE_ID = P.DIVIDED_INVOICE_ID
                              WHERE N.DB_CODE = @DB_CODE AND P.DB_CODE = @DB_CODE AND R.DB_CODE = @DB_CODE AND P.DELIVERY_ID = @DELIVERYID AND P.CREATE_DATE = @DIVIDED_DATE
                              UNION ALL
                              SELECT
                              P.INVOICE_ID,P.DIVIDED_INVOICE_ID,N.TRANSACTION_REF, N.CUSTOMER_CODE, N.ACC_NAME_KH, CASE WHEN N.STATUS = 'O' THEN '1.00' END 'OLD', CASE WHEN N.STATUS = 'N' THEN '1.00' END 'NEW', CASE WHEN N.STATUS = 'C' THEN '1.00' END 'CHANGE',
                              N.HEADER_TRANSACTION_VALUES, CONVERT(BIT, 0)[RETURN], '' OTHER, 0, 0, CONVERT(BIT, 0) PAID
                              FROM NEW_INVOICE N
                              INNER JOIN PC_DIVIDED_INVOICE P
                              ON P.INVOICE_ID = N.ID
                              WHERE P.STATUS = '1' AND N.DB_CODE = @DB_CODE AND P.DB_CODE = @DB_CODE AND P.DELIVERY_ID = @DELIVERYID AND P.CREATE_DATE = @DIVIDED_DATE
                    ";
        var param = new
        {
            DB_CODE = dbCode,
            DELIVERYID = deliveryId,
            DIVIDED_DATE = date.ToString("yyyy-MM-dd")
        };
        var execute = await _sqlDataAccess.LoadData<GeneralInvoicePaymentModel, dynamic>(sql, param);
        return execute.ToList();
    }

    public async Task<int> UpdateDeliveryInvoicePaid(DeliveryGeneralInvoicePaidUpdateModel model)
    {
        var afftectedRow = 0;
        var transaction = _dbConnection.BeginTransaction();
        try
        {
            var insertQuery =
                $@"INSERT INTO PC_EDIT_VALUE_DIVIDED_INVOICE(DB_CODE,DIVIDED_INVOICE_ID,OLD_VALUE,NEW_VALUE,DESCRIPTION,CREATE_DATE,CREATE_BY)
                        VALUES(@DB_CODE,@DIVIDED_ID,@OLD_VALUE,@NEW_VALUE,@DESCRIPTION,GETDATE(),@CREATED_BY)";
            var insertParam = new
            {
                DB_CODE = model.DbCode,
                DIVIDED_ID = model.DividedInvoiceId,
                OLD_VALUE = model.OldAmount,
                NEW_VALUE = model.NewAmount,
                DESCRIPTION = model.Description,
                CREATED_BY = model.CreateBy
            };
            afftectedRow = await _dbConnection.ExecuteAsync(insertQuery, insertParam, transaction);


            if (afftectedRow > 0)
            {
                var updateQuery =
                    $@"UPDATE NEW_INVOICE SET HEADER_TRANSACTION_VALUES = @InvoiceValue WHERE ID IN (SELECT INVOICE_ID FROM PC_DIVIDED_INVOICE WHERE DIVIDED_INVOICE_ID = @ID)";
                var updateParam = new
                {
                    NEW_VALUE = model.NewAmount,
                    ID = model.DividedInvoiceId
                };
                await _dbConnection.ExecuteAsync(updateQuery, updateParam, transaction);
                transaction.Commit();
            }
            else
            {
                transaction.Rollback();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            transaction.Rollback();
        }

        return afftectedRow;
    }

    public async Task<bool> CheckExistsPaymentHeaderByInvoiceDividendDateAndDeliveryId(DateTime invoiceDividendDate,
        string deliveryId, string dbCode)
    {
        const string sql =
            @"SELECT CAST(COUNT(*) AS BIT) FROM PAYMENT_INVOICE_HEADER WHERE INVOICE_DIVIDEND_DATE = @INVOICE_DIVIDEND_DATE AND DELIVERY_ID = @DELIVERY_ID
            AND DB_CODE = @DB_CODE";
        var param = new
        {
            INVOICE_DIVIDEND_DATE = invoiceDividendDate,
            DELIVERY_ID = deliveryId,
            DB_CODE = dbCode
        };
        return await _sqlDataAccess.ExecuteScalarAsync<bool, dynamic>(sql, param);
    }

    public async Task<int> CreatePaymentHeader(PaymentInvoiceHeaderModel headerModel)
    {
        var sql =
            @"INSERT INTO PAYMENT_INVOICE_HEADER(DB_CODE,DELIVERY_ID,P_PERIOD,ENTRIES_CODE,CREATED_DATE,CREATED_BY,P_STATE,INVOICE_DIVIDEND_DATE)
            VALUES(@DB_CODE,@DELIVERY_ID,@PERIOD,@ENTRIES_CODE,@CREATED_DATE,@CREATED_BY,@STATUS,@INVOICE_DIVIDEND_DATE)";
        var param = new
        {
            DB_CODE = headerModel.DbCode,
            DELIVERY_ID = headerModel.DeliveryId,
            PERIOD = headerModel.Period,
            ENTRIES_CODE = headerModel.EntriesCode,
            CREATED_DATE = headerModel.CreatedDate,
            CREATED_BY = headerModel.CreatedBy,
            STATUS = "1",
            INVOICE_DIVIDEND_DATE = headerModel.InvoiceDividendDate
        };
        var rowAffected = await _sqlDataAccess.ExecuteAsync(sql, param);
        if (rowAffected <= 0) return 0;
        sql = "SELECT ISNULL(MAX(ID),0) FROM PAYMENT_INVOICE_HEADER";
        var results = await _sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, new { });
        return results;
    }

    public async Task<bool> CreatePaymentExpense(List<ExpenseModel> expenseModel)
    {
        const string sql =
            @"INSERT INTO PAYMENT_INVOICE_EXPENSE(DB_CODE,PAYMENT_HEADER_ID,NAME,DESCRIPTION,QUANTITY,UNIT_PRICE,TOTAL,CREATED_DATE,CREATED_BY,CURRENCY_TYPE,EXCHANGE_RATE)
            VALUES(@DB_CODE,@PAYMENT_HEADER_ID,@NAME,@DESCRIPTION,@QUANTITY,@UNIT_PRICE,@TOTAL,@CREATED_DATE,@CREATED_BY,@CURRENCY_TYPE,@EXCHANGE_RATE)";
        using var transaction = _dbConnection.BeginTransaction();
        var rowAffected = 0;
        foreach (var param in expenseModel.Select(model => new
                 {
                     DB_CODE = model.DbCode,
                     PAYMENT_HEADER_ID = model.PaymentInvoiceHeaderId,
                     NAME = model.Name,
                     DESCRIPTION = model.Description,
                     QUANTITY = model.Quantity,
                     UNIT_PRICE = model.UnitPrice,
                     TOTAL = model.Total,
                     CREATED_DATE = model.CreatedDate,
                     CREATED_BY = model.CreatedBy,
                     CURRENCY_TYPE = model.CurrencyType,
                     EXCHANGE_RATE = model.ExchangeRate
                 }))
        {
            var rowCount = await _dbConnection.ExecuteAsync(sql, param, transaction);
            if (rowCount > 0)
                rowAffected++;
        }

        if (rowAffected == expenseModel.Count)
        {
            transaction.Commit();
            return true;
        }

        transaction.Rollback();
        return false;
    }

    public async Task<int> GetPaymentHeaderId(DateTime invoiceDividendDate, string deliveryId, string dbCode)
    {
        const string sql =
            @"SELECT ID FROM PAYMENT_INVOICE_HEADER WHERE DB_CODE = @DB_CODE AND DELIVERY_ID = @DELIVERY_ID AND INVOICE_DIVIDEND_DATE = @INVOICE_DIVIDEND_DATE";
        var param = new
        {
            DB_CODE = dbCode,
            DELIVERY_ID = deliveryId,
            INVOICE_DIVIDEND_DATE = invoiceDividendDate
        };
        return await _sqlDataAccess.ExecuteScalarAsync<int, dynamic>(sql, param);
    }
}