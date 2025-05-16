using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class InvoiceReportRepository : IInvoiceReportRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public InvoiceReportRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<InvoiceReportModel>> GetInvoiceReportAsync(string dbCode, DateTime date)
        {
            var sql =
                $@"SELECT DELIVERIES_NAME DeliveryName,COUNT(CASE WHEN N.STATUS = 'N' THEN 1 END) AS 'NEW',COUNT(CASE WHEN N.STATUS = 'C' THEN 1 END) AS 'CHANGE',COUNT(CASE WHEN N.STATUS = 'O' THEN 1 END) AS 'OLD',
                (COUNT(CASE WHEN N.STATUS = 'N' THEN 1 END) + COUNT(CASE WHEN N.STATUS = 'C' THEN 1 END) + COUNT(CASE WHEN N.STATUS = 'O' THEN 1 END)) AS 'TOTAL'
                 FROM NEW_INVOICE N INNER JOIN PC_DIVIDED_INVOICE P ON P.INVOICE_ID = N.ID INNER JOIN TB_BCDELIVERIES D ON D.DELIVERIES_ID = P.DELIVERY_ID
                 WHERE P.CREATE_DATE = @DATE AND P.DB_CODE = @DB_CODE
                 GROUP BY DELIVERIES_NAME";
            var param = new
            {
                DB_CODE = dbCode,
                DATE = date,
            };
            var execute = await _sqlDataAccess.LoadData<InvoiceReportModel, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
