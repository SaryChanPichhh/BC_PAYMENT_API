using BC.PAYMENT.APPLICATION.Interfaces.StockCar;
using BC.PAYMENT.CORE.Contracts.Response.StockCar;
using BC.PAYMENT.CORE.Entities.StockCar;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.StockCar;

public class StockCarPaymentInvoiceRepository(ISqlDataAccess sqlDataAccess) : IStockCarPaymentInvoiceRepository
{
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

    public async Task<int> PaymentInvoiceAsync(int invoiceId, double amount, string createdBy = "")
    {
        const string sql = @"
            INSERT INTO BCSTOCK_CAR_INVOICE_PAYMENT (INVOICE_ID, AMOUNT, CREATED_DATE, CREATED_BY)
            VALUES (@InvoiceId, @Amount, @CreatedDate, @CreatedBy);
            UPDATE BCSTOCK_CAR SET STATUS = 0 WHERE ID = @InvoiceId;";

        var parameter = new
        {
            InvoiceId = invoiceId,
            Amount = amount,
            CreatedDate = DateTime.Now,
            CreatedBy = createdBy
        };

        var affectedRows = await sqlDataAccess.ExecuteAsync(sql, parameter);
        return affectedRows > 0 ? 1 : 0;
    }

    public int PaymentInvoice(int invoiceId, double amount, string createdBy = "")
    {
        try
        {
            return PaymentInvoiceAsync(invoiceId, amount, createdBy).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return 0;
        }
    }

    public Task<int> InsertPaymentInvoiceAsync(int invoiceId, double amountPaid, string createdBy)
    {
        return PaymentInvoiceAsync(invoiceId, amountPaid, createdBy);
    }

    public async Task<int> DeletePaymentInvoiceByInvoiceIdAsync(int invoiceId)
    {
        const string sql = "DELETE FROM BCSTOCK_CAR_INVOICE_PAYMENT WHERE INVOICE_ID = @INVOICE_ID";
        var param = new
        {
            INVOICE_ID = invoiceId
        };
        return await sqlDataAccess.ExecuteAsync(sql, param);
    }

    public Task<int> DeletePaymentInvoiceByInvoiceId(int invoiceId)
    {
        return DeletePaymentInvoiceByInvoiceIdAsync(invoiceId);
    }

    public async Task<int> UpdatePaymentInvoiceByIdAsync(int id, double amount)
    {
        const string sql = "UPDATE BCSTOCK_CAR_INVOICE_PAYMENT SET AMOUNT = @Amount WHERE ID = @Id";
        var param = new
        {
            Amount = amount,
            Id = id
        };
        return await sqlDataAccess.ExecuteAsync(sql, param);
    }

    public Task<int> UpdatePaymentInvoiceById(int id, double amount)
    {
        return UpdatePaymentInvoiceByIdAsync(id, amount);
    }

    public async Task<List<InvoicesPayment>> GetPaymentHistoryByInvoiceIdAsync(int invoiceId)
    {
        const string sql =
            "SELECT ID Id,AMOUNT Amount,CREATED_DATE CreatedDate FROM BCSTOCK_CAR_INVOICE_PAYMENT WHERE INVOICE_ID = @InvoiceId";
        var param = new
        {
            InvoiceId = invoiceId
        };
        var data = await sqlDataAccess.LoadData<InvoicesPayment, dynamic>(sql, param);
        return data.ToList();
    }

    public Task<List<InvoicesPayment>> GetPaymentHistoryByInvoiceId(int invoiceId)
    {
        return GetPaymentHistoryByInvoiceIdAsync(invoiceId);
    }
}