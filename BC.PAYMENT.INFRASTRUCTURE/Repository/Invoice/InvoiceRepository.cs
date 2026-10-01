using BC.PAYMENT.CORE.Contracts.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using NewInvoiceModel = BC.PAYMENT.CORE.Entities.Invoice.NewInvoiceModel;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Invoice;

public class InvoiceRepository(
    ISqlDataAccess sqlDataAccess,
    ICustomerRepository customerRepository,
    IConfiguration configuration,
    IDbConnection dbConnection)
    : IInvoiceRepository
{
    public async Task<int> SavePaymentInvoiceAsync(List<PaymentInvoiceRequest> request)
    {
        var dataTable = AppExtension.ConvertToDataTable([request]);
        var parameter = new
        {
            PAYMENT_INVOICE = dataTable.AsTableValuedParameter("SAVE_PAYMENT_INVOICE")
        };
        return await sqlDataAccess.ExecuteAsync(PaymentInvoiceQueries.SavePaymentInvoice, parameter,
            CommandType.StoredProcedure);
    }

    public async Task<List<DividedInvoiceTransactionResponse>> GetInvoiceTransactionByDeliveryAndDateAsync(
        string dbCode, string deliveryId, DateTime date)
    {
        return (await sqlDataAccess.LoadData<DividedInvoiceTransactionResponse, dynamic>(
            DividedInvoiceQueries.GetDividedTransactionStatus,
            new
            {
                DB_CODE = dbCode,
                DELIVERY_ID = deliveryId,
                DIVIDED_DATE = date
            })).ToList();
    }

    public async Task<List<InvoiceResponse>> GetInvoiceByAreaAndTransCodeAsync(string dbCode, string areaId,
        string transCode)
    {
        var condition = string.Empty;
        if (!string.IsNullOrEmpty(transCode))
            condition = $@"AND [Transaction] NOT IN ('{transCode}')";
        var arguments = new
        {
            DB_CODE = dbCode,
            AREA_ID = areaId
        };
        var data = await sqlDataAccess.LoadData<InvoiceResponse, dynamic>
            (InvoiceQueries.GetInvoicesByAreaAndTransCode(condition), arguments);
        return data.ToList();
    }

    public async Task<bool> PostPrintInvoiceAsync(string transactionInvoice, RequestType type, string dbCode,
        string period, string userName)
    {
        var param = new
        {
            NEW_TRANS_REF = transactionInvoice,
            TRANS_REF = transactionInvoice,
            INV_DATE = DateTime.Today.ToString("MM/dd/yyyy"),
            INV_PRD = period
        };
        if (dbConnection.State == ConnectionState.Closed) dbConnection.Open();
        using var transaction = dbConnection.BeginTransaction();
        try
        {
            await dbConnection.ExecuteAsync(InvoiceQueries.ExecuteSiSoPrintInvoice(dbCode), new
            {
                DB_CODE = dbCode,
                TRANS_REF = transactionInvoice,
                INV_DATE = DateTime.Today.ToString("MM/dd/yyyy"),
                NEW_TRANS_REF = transactionInvoice,
                INV_PRD = period,
                USER_INVOICED = userName
            }, transaction, commandType: CommandType.StoredProcedure);
            await dbConnection.ExecuteAsync(InvoiceQueries.InsertSiSoHdrRecordTypeO(dbCode), param, transaction);
            await dbConnection.ExecuteAsync(
                InvoiceQueries.UpdateSiSoHdrStatus(dbCode),
                new
                {
                    TRANS_REF = transactionInvoice
                },
                transaction
            );
            await dbConnection.ExecuteAsync(
                InvoiceQueries.UpdateSiSoDetStatus(dbCode),
                new
                {
                    INV_DATE = DateTime.Today.ToString("MM/dd/yyyy"),
                    INV_PRD = period,
                    NEW_TRANS_REF = transactionInvoice,
                    USER_INVOICED = userName,
                    TRANS_REF = transactionInvoice
                }, transaction);
            if (type == RequestType.Exchange)
                await dbConnection.ExecuteAsync(
                    InvoiceQueries.UpdateChangeInvoiceExchangeStatus,
                    new { TRANSACTION_CODE = transactionInvoice }, transaction);
            else
                await dbConnection.ExecuteAsync(
                    InvoiceQueries.UpdateChangeInvoiceRepairStatus,
                    new { TRANSACTION_CODE = transactionInvoice }, transaction);
            transaction.Commit();
            return true;
        }
        catch (SqlException e)
        {
            transaction.Rollback();
            return false;
        }
    }

    public async Task<bool> IsAlreadyPosted(string dbCode, string transactionCode)
    {
        var param = new { TRANS_REF = transactionCode };
        var execute = await sqlDataAccess.LoadSingleData<bool, dynamic>(InvoiceQueries.IsAlreadyPost(dbCode), param);
        return execute;
    }

    public async Task<bool> SaveRecordPostInvoice(string dbCode, string userName, int invoiceId, string transactionCode,
        string transCode)
    {
        const string sql =
            @"INSERT INTO TB_BC_CHANGEINVOICE_POST_INVOICE (DB_CODE,INVOICE_ID,TRANSACTION_CODE,POST_TRANSACTION,CREATED_BY) 
                VALUES (@DB_CODE,@INVOICE_ID,@TRANSACTION_CODE,@POST_TRANSACTION,@CREATED_BY)";
        var param = new
        {
            DB_CODE = dbCode,
            INVOICE_ID = invoiceId,
            TRANSACTION_CODE = transactionCode,
            POST_TRANSACTION = transCode,
            CREATED_BY = userName
        };
        var affectedRow = await sqlDataAccess.ExecuteAsync(sql, param);
        return affectedRow > 0;
    }

    public async Task<List<SaleDetailsDto>> GetItemExpiredDates(string dbCode,
        Dictionary<string, List<string>> itemCodeAndExpireDate, string wareHouse)
    {
        List<SaleDetailsDto> result = new();
        foreach (var key in itemCodeAndExpireDate)
        {
            var expireDateParam = string.Join(",", key.Value.Select(date => $"{date}"));
            var param = new
            {
                DBCODE = dbCode,
                ITEM_CODE = key.Key,
                WAREHOUSE = wareHouse,
                EXPIREDDATE = key.Value
            };
            var execute =
                await sqlDataAccess.LoadData<SaleDetailsDto, dynamic>(InvoiceQueries.GetItemExpiredDates(dbCode),
                    param);
            result.AddRange(execute.ToList());
        }

        return result;
    }

    public async Task<int> CreateInvoiceSaleAsync(string dbCode, SaleHeaderDto saleHeader,
        List<SaleDetailsDto> detailsDtos)
    {
        var affectedRow = 0;
        if (dbConnection.State == ConnectionState.Closed)
            dbConnection.Open();
        var transaction = dbConnection.BeginTransaction();
        try
        {
            var param = new
            {
                REC_TYPE = saleHeader.RecType, //1
                TRANS_REF = saleHeader.Transaction, //2
                HEADER_ID = saleHeader.HeaderId, //3
                CUST_CODE = saleHeader.CustomerCode, //4
                DELIV_ADD = saleHeader.DeliveryAdd, //5
                TRANS_DATE = saleHeader.TransactionDate, //6
                STATUS = saleHeader.Status, //7
                TRANS_CD = saleHeader.TransactionCd, //8
                TRANS_CODE = saleHeader.TransactionCode, //9
                ORDER_NO = saleHeader.OrderNo, //10
                ORDER_DATE = saleHeader.OrderDate, //11
                PRN_DATE = saleHeader.PrnDate, //12
                DEL_DATE = saleHeader.DelDate, //13
                INV_DATE = saleHeader.InvoiceDate, //14
                INV_PRD = saleHeader.InvoicePeriod, //15
                CUST_REF = saleHeader.CustomerRef, //16
                DEL_REF = saleHeader.DeliveryRef, //17
                COMMENTS = saleHeader.Comments, //18
                TRANS_VAL = saleHeader.TransactionValue, //19
                PAY_DATE = saleHeader.PayDate, //20
                ANAL_M0 = saleHeader.AnalM0, //21
                ANAL_M1 = saleHeader.AnalM1, //22
                ANAL_M2 = saleHeader.AnalM2, //23
                ANAL_M3 = saleHeader.AnalM3, //24
                ANAL_M4 = saleHeader.AnalM4, //25
                ANAL_M5 = saleHeader.AnalM5, //26
                ANAL_M6 = saleHeader.AnalM6, //27
                ANAL_M7 = saleHeader.AnalM7, //28
                ANAL_M8 = saleHeader.AnalM8, //29
                ANAL_M9 = saleHeader.AnalM9, //30
                QUOTE_CONVERT = saleHeader.QuoteConvert, //31
                QUOTE_PRINT = saleHeader.QuotePrint, //32
                QUOTE_EXPIRY = saleHeader.QuoteExpiry, //33
                QUOTED_PRD = saleHeader.QuotePeriod, //34
                QUOTATION_REF = saleHeader.QuotationRef, //35
                DATE_QUOTED = saleHeader.DateQuoted, //36
                VOID_STATUS = saleHeader.VoidStatus, //37
                USER_CODE = saleHeader.UserCode //38
            };
            affectedRow = await dbConnection.ExecuteAsync(InvoiceQueries.InsertSiSoHdr(dbCode), param, transaction);
            if (affectedRow > 0)
            {
                foreach (var parameter in detailsDtos.Select(saleDetailsDto => new
                         {
                             REC_TYPE = saleDetailsDto.RefType,
                             DETAIL_ID = saleDetailsDto.DetailId,
                             TRANS_TYPE = saleDetailsDto.TransType,
                             TRANS_REF = saleDetailsDto.TransRef,
                             TRANS_LINE = saleDetailsDto.TransLine,
                             TRANS_CD = saleDetailsDto.TransCd,
                             LOCATION = saleDetailsDto.Location,
                             ITEM_CODE = saleDetailsDto.ItemCode,
                             DESCRIPTN = saleDetailsDto.Description,
                             DUE_DATE = saleDetailsDto.DueDate,
                             STATUS = "05",
                             VALUE_1 = saleDetailsDto.Value1,
                             VALUE_2 = saleDetailsDto.Value2,
                             VALUE_3 = saleDetailsDto.Value3,
                             VALUE_4 = saleDetailsDto.Value4,
                             VALUE_5 = saleDetailsDto.Value5,
                             VALUE_6 = saleDetailsDto.Value6,
                             VALUE_7 = saleDetailsDto.Value7,
                             VALUE_8 = saleDetailsDto.Value8,
                             VALUE_9 = saleDetailsDto.Value9,
                             VALUE_10 = saleDetailsDto.Value10,
                             VALUE_11 = saleDetailsDto.Value11,
                             VALUE_12 = saleDetailsDto.Value12,
                             VALUE_13 = saleDetailsDto.Value13,
                             VALUE_14 = saleDetailsDto.Value14,
                             VALUE_15 = saleDetailsDto.Value15,
                             VALUE_16 = saleDetailsDto.Value16,
                             VALUE_17 = saleDetailsDto.Value17,
                             VALUE_18 = saleDetailsDto.Value18,
                             VALUE_19 = saleDetailsDto.Value19,
                             VALUE_20 = saleDetailsDto.Value20,
                             UNIT_SALE = saleDetailsDto.UnitSale,
                             ORD_PRD = saleDetailsDto.OrdPeriod,
                             DEL_DATE = saleDetailsDto.DelDate,
                             INV_DATE = saleDetailsDto.InvoiceDate,
                             INV_NO = saleDetailsDto.InvoiceNo,
                             INV_PRD = saleDetailsDto.InvoicePeriod,
                             ACCNT_CODE = saleDetailsDto.AccountCode,
                             ANAL_M0 = saleDetailsDto.AnalM0,
                             ANAL_M1 = saleDetailsDto.AnalM1,
                             ANAL_M2 = saleDetailsDto.AnalM2,
                             ANAL_M3 = saleDetailsDto.AnalM3,
                             ANAL_M4 = saleDetailsDto.AnalM4,
                             ANAL_M5 = saleDetailsDto.AnalM5,
                             ANAL_M6 = saleDetailsDto.AnalM6,
                             ANAL_M7 = saleDetailsDto.AnalM7,
                             ANAL_M8 = saleDetailsDto.AnalM8,
                             ANAL_M9 = saleDetailsDto.AnalM9,
                             ASSEMBLY_IND = saleDetailsDto.AssemblyInd,
                             SPLIT_VAL = saleDetailsDto.SplitVal,
                             CREDIT_STATUS = saleDetailsDto.CreditStatus,
                             PRICE_BOOK = saleDetailsDto.PriceBook,
                             SALE_QTY_VALUE = saleDetailsDto.SaleQtyValue,
                             STK_QTY_VALUE = saleDetailsDto.StkQtyValue,
                             TOT_VALUE = saleDetailsDto.TopValue,
                             DISP_VAL_1 = saleDetailsDto.DisplayValue1,
                             DISP_VAL_2 = saleDetailsDto.DisplayValue2,
                             FIXED_VAL = saleDetailsDto.FixedValue,
                             LINE_REF = saleDetailsDto.LineRef,
                             USER_CODE = saleDetailsDto.UserCode,
                             USER_INVOICED = saleDetailsDto.UserInvoice,
                             ALLOC_REF = saleDetailsDto.AllowRef,
                             UPDATE_STOCK = saleDetailsDto.UpdateStock,
                             FIXED_VAL_2 = saleDetailsDto.FixedStockValue2,
                             FIXED_VAL_3 = saleDetailsDto.FixedStockValue3
                         }))

                    affectedRow +=
                        await dbConnection.ExecuteAsync(InvoiceQueries.InsertSiSoDet(dbCode), parameter, transaction);

                if (affectedRow > 1)
                {
                    transaction.Commit();
                    return affectedRow;
                }
            }

            transaction.Rollback();
            return 0;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            Debug.WriteLine($@"Error Exception : ${e.Message}");
        }

        return 0;
    }

    public async Task<int> InsertRecordInvoice(string dbCode, string userName, string transaction, string customerCode,
        string customerName, double value, DateTime date,
        string entryCode)
    {
        var param = new
        {
            DB_CODE = dbCode,
            TRANSACTION_CODE = transaction,
            CUSTOMER_CODE = customerCode,
            CUSTOMER_NAME = customerName,
            VALUE = value,
            STATUS = "C",
            CREATED_DATE = date,
            CREATED_BY = userName,
            IS_DIVIDED = "1",
            ENTRIES_CODE = entryCode
        };
        var rowAffected = await sqlDataAccess.ExecuteAsync(InvoiceQueries.InsertRecordInvoice, param);
        return rowAffected;
    }

    public async Task<int> CreateInvoice(CreateInvoiceDto createInvoiceDto,
        List<CreateInvoiceDetailDto> createInvoiceDetailDto)
    {
        var connection = new SqlConnection(configuration.GetConnectionString("DBConnection"));
        var affectedRow = 0;
        if (connection.State == ConnectionState.Closed)
            connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var paramMaster = new
            {
                DB_CODE = createInvoiceDto.DbCode,
                DB_CODE_1 = createInvoiceDto.DbCode,
                INVOICE_DATE = createInvoiceDto.InvoiceDate,
                INVOICE_DUE = createInvoiceDto.InvoiceDue,
                CUSTOMER_CODE = createInvoiceDto.CustomerCode,
                DISCOUNT = createInvoiceDto.Discount,
                TAXES = createInvoiceDto.Taxes,
                TOTAL_AMOUNT = createInvoiceDto.TotalAmount,
                CREATED_DATE = createInvoiceDto.CreatedDate,
                CREATED_BY = createInvoiceDto.CreatedBy,
                INVOICE_REFERENCE = createInvoiceDto.InvoiceReference,
                DESCRIPTION = createInvoiceDto.Description
            };

            affectedRow += await connection.ExecuteAsync(InvoiceQueries.InsertRepairInvoice, paramMaster, transaction);
            foreach (var invoiceDetailDto in createInvoiceDetailDto)
            {
                var paramDetail = new
                {
                    INVOICE_ID = createInvoiceDto.InvoiceReference,
                    REPAIR_COMPLETED_ID = invoiceDetailDto.RepairCompletedId,
                    QUANTITY = invoiceDetailDto.Quantity,
                    TRANS_REF = invoiceDetailDto.ItemTransaction,
                    UNIT_PRICE = invoiceDetailDto.UnitPrice,
                    TOTAL = invoiceDetailDto.Total,
                    REQUEST_REPAIR_ID = invoiceDetailDto.RequestRepairId
                };
                var requestDeetailId =
                    await connection.ExecuteScalarAsync<int>(InvoiceQueries.InsertRepairInvoiceDetails, paramDetail,
                        transaction);
                affectedRow += await connection.ExecuteAsync(
                    InvoiceQueries.UpdateChangeInvoiceDetail,
                    new { RequestDetailId = requestDeetailId }, transaction);

                // update to completed
                affectedRow += await connection.ExecuteAsync(InvoiceQueries.UpdateRepairCompleted,
                    new { RepairCompletedId = invoiceDetailDto.RepairCompletedId }, transaction);
                affectedRow += await connection.ExecuteAsync(InvoiceQueries.UpdateRepairMaster,
                    new { COMPLETED_DATE = DateTime.Today, TRANSACTION = invoiceDetailDto.ItemTransaction },
                    transaction);
            }

            if (affectedRow > 3)
            {
                transaction.Commit();
                return affectedRow;
            }

            transaction.Rollback();
            return 0;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            Debug.WriteLine($@"Error Exception : {e.Message}");
        }

        return 0;
    }

    public async Task<InvoiceDetailDto> GetInvoiceByCustomerCode(string dbCode, string customerCode)
    {
        var param = new
        {
            CustomerCode = customerCode,
            DbCode = dbCode
        };
        return await sqlDataAccess.LoadSingleData<InvoiceDetailDto, dynamic>(
            InvoiceQueries.GetInvoiceByCustomerCodeQuery, param);
    }

    public async Task<bool> CheckStockQuantityAsync(string dbCode, string location, string itemCode,
        int quantityRequest)
    {
        var param = new
        {
            DB_CODE = dbCode,
            LOCATION = location,
            ITEM_CODE = itemCode,
            QUANTITYREQUEST = quantityRequest
        };
        var results =
            await sqlDataAccess.LoadSingleData<bool, dynamic>(InvoiceQueries.CheckStockQuantity(dbCode), param);
        return results;
    }

    public async Task UpdateStatusExchangeReceivedToCredit(int id, int status)
    {
        var param = new
        {
            ID = id,
            STATUS = status
        };
        await sqlDataAccess.ExecuteAsync(InvoiceQueries.UpdateStatusExchangeReceivedToCredit, param);
    }

    public async Task UpdateStatusRequestExchangeDetails(int id, ExchangeStatus exchangeStatus)
    {
        var param = new
        {
            STATUS = Enum.GetName(typeof(ExchangeStatus), exchangeStatus),
            ID = id
        };
        await sqlDataAccess.ExecuteAsync(InvoiceQueries.UpdateStatusRequestExchangeDetails, param);
    }

    public async Task SaveRecordItemExchanged(string dbCode, string userName, string transaction, string itemCode,
        int quantity, double unitPrice)
    {
        var param = new
        {
            TRANSACTION = transaction,
            ITEM_CODE = itemCode,
            QUANTITY = quantity,
            CREATED_DATE = DateTime.Today,
            CREATED_BY = userName,
            UNIT_PRICE = unitPrice,
            DB_CODE = dbCode
        };
        await sqlDataAccess.ExecuteAsync(InvoiceQueries.SaveRecordItemExchanged, param);
    }

    public async Task<bool> IsAllItemRequestCompletedByRequestIdAsync(int requestId)
    {
        var param = new
        {
            REQUEST_ID = requestId
        };
        var result =
            await sqlDataAccess.LoadSingleData<bool, dynamic>(InvoiceQueries.IsAllItemRequestCompletedByRequestId,
                param);
        return result;
    }

    public async Task<bool> UpdateReceivedToCompletedByIdAsync(string dbCode, int requestId)
    {
        var param = new
        {
            REQUEST_ID = requestId,
            DB_CODE = dbCode
        };
        var result = await sqlDataAccess.ExecuteAsync(InvoiceQueries.UpdateReceivedToCompletedById, param);
        return result == 1;
    }

    public Task<int> UpdateValue6ToZero(
        List<(string newTransaction, string TransLine, string itemCode, string oldTransaction)> tupleValues)
    {
        throw new NotImplementedException();
    }
}