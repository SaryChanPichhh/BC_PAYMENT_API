using BC.PAYMENT.APPLICATION.Interfaces.StockCar;
using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Entities;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.StockCar;

public class StockCarInvoiceRepository(ISqlDataAccess sqlDataAccess) : IStockCarInvoiceRepository
{
    public async Task<List<StockCarInvoiceResponse>> GetStockCarInvoicesByInvoiceTypeAsync(string dbCode,
        int templateId, InvoiceStatus invoiceStatus)
    {
        var sql =
            $@"SELECT N.ID InvoiceId,CUSTOMER_CODE CustomerCode,CUSTOMER_NAME CustomerName,CODE TransactionCode,VALUE InvoiceValue,
             TRANSACTION_DATE TransactionDate,
             UPPER(Employee.USER_NAME) Employee,Market,Area
          FROM BCSTOCK_CAR N INNER JOIN TEMPLATE T ON T.Id = N.TEMPLATE_ID 
             INNER JOIN (SELECT USER_NAME,USER_ID FROM BCUSERS) Employee
          ON Convert(varchar,Employee.USER_ID) = T.Employee
             LEFT JOIN (SELECT S.ADD_CODE CustomerCode,M.MARKET_KHMER_NAME Market,A.AREA_NAME_KHMER Area
             FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID
             INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
             WHERE S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE) Cust
             on Cust.CustomerCode = CUSTOMER_CODE
          WHERE N.DB_CODE = @DB_CODE and T.DbCode = @DB_CODE and N.TYPE = @INVOICE_STATUS and T.Id = @TEMPLATE_ID";
        var param = new
        {
            INVOICE_STATUS = Enum.GetName(typeof(InvoiceStatus), invoiceStatus)?[..1],
            DB_CODE = dbCode,
            TEMPLATE_ID = templateId
        };
        return (await sqlDataAccess.LoadData<StockCarInvoiceResponse, dynamic>(sql, param)).ToList();
    }

    public async Task<List<PaymentInvoiceResponse>> GetPaymentInvoicesByTemplateIdAsync(string dbCode, int templateId)
    {
        const string sql = @"
            SELECT C.ID InvoiceId, CUSTOMER_CODE CustomerCode, CUSTOMER_NAME CustomerName, CODE TransactionCode,
                   CASE 
                       WHEN TYPE = 'N' THEN N'ថ្មី'
                       WHEN TYPE = 'C' THEN N'ដូរ'
                       WHEN TYPE = 'O' THEN N'ចាស់' 
                       ELSE N'មិនស្គាល់'
                   END 'InvoiceType',
                   VALUE InvoiceValue, SUM(P.AMOUNT) AmountPaid, C.STATUS Status, Cust.Market, Area
            FROM BCSTOCK_CAR C 
            INNER JOIN TEMPLATE T ON T.Id = C.TEMPLATE_ID 
            LEFT JOIN BCSTOCK_CAR_INVOICE_PAYMENT P ON P.INVOICE_ID = C.ID
            LEFT JOIN (
                SELECT S.ADD_CODE CustomerCode, M.MARKET_KHMER_NAME Market, A.AREA_NAME_KHMER Area
                FROM SIADD S 
                INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID
                INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                WHERE S.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE
            ) Cust ON Cust.CustomerCode = CUSTOMER_CODE
            WHERE C.DB_CODE = @DB_CODE AND T.DbCode = @DB_CODE AND T.Id = @TEMPLATE_ID
              AND C.ID NOT IN (SELECT INVOICE_ID FROM BCSTOCK_CAR_INVOICE_RETURN)
            GROUP BY C.ID, CUSTOMER_CODE, CUSTOMER_NAME, CODE, Cust.Market, Area,
                     VALUE, C.STATUS, TYPE 
            ORDER BY STATUS";

        var parameter = new
        {
            DB_CODE = dbCode,
            TEMPLATE_ID = templateId
        };
        var data = await sqlDataAccess.LoadData<PaymentInvoiceResponse, dynamic>(sql, parameter);
        return data.ToList();
    }

    public async Task<int> DeleteStockCarInvoiceByIdAsync(int id)
    {
        var sql = $@"DELETE FROM BCSTOCK_CAR WHERE ID = @ID";
        return await sqlDataAccess.ExecuteAsync(sql, new { ID = id });
    }

    public async Task<int> AddNewStockCarInvoiceAsync(BcStockCar model)
    {
        const string sql =
            $@"IF NOT EXISTS (SELECT * FROM BCSTOCK_CAR WHERE TEMPLATE_ID = @TEMPLATE_ID and CODE = @INVOICE_CODE) 
            INSERT INTO BCSTOCK_CAR Output inserted.ID Values(@DB_CODE, @CUSTOMER_CODE, @CUSTOMER_NAME, @INVOICE_CODE, @INVOICE_VALUE, @INVOICE_TYPE, @PERIOD,
             @TRANS_DATE, @STATUS, @CREATED_DATE,@CREATED_BY, @TEMPLATE_ID)";

        var parameter = new
        {
            DB_CODE = model.DbCode,
            CUSTOMER_CODE = model.CustomerCode,
            CUSTOMER_NAME = model.CustomerName,
            INVOICE_CODE = model.Code,
            INVOICE_VALUE = model.Value,
            INVOICE_TYPE = model.Type,
            PERIOD = model.Period,
            TRANS_DATE = model.TransactionDate,
            STATUS = model.Status,
            CREATED_DATE = model.CreatedDate,
            CREATED_BY = model.CreatedBy,
            TEMPLATE_ID = model.TemplateId
        };

        var insertedId = await sqlDataAccess.ExecuteScalarAsync<int?, dynamic>(sql, parameter);
        model.Id = insertedId ?? 0;
        return model.Id;
    }
}