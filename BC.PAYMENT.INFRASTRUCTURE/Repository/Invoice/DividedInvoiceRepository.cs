
using BC.PAYMENT.CORE.Contracts.Invoice;
using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using IDividedInvoiceRepository = BC.PAYMENT.APPLICATION.Interfaces.Invoice.IDividedInvoiceRepository;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Invoice
{
    public class DividedInvoiceRepository(ISqlDataAccess sqlDataAccess) : IDividedInvoiceRepository
    {
        public async Task<List<DividedInvoiceResponse>> GetDividedInvoicesByDeliveryIdAndDateAsync(string dbCode, string deliveryId, DateTime date)
        {
            var param = new
            {
                DB_CODE = dbCode,
                DELIVERY_ID = deliveryId,
                DATE = date,
            };
            var execute = await sqlDataAccess.LoadData<DividedInvoiceResponse, dynamic>(DividedInvoiceQueries.GetDividedInvoicesByDeliveryIdAndDate, param);

            return execute.ToList();
        }

        

        public async Task<List<DividedInvoiceDetailResponse>> GetDividedInvoiceDetailByDateAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var argument = new
            {
                DB_CODE = dbCode,
                FROM_DATE = fromDate,
                TO_DATE = toDate,
            };
            return (await sqlDataAccess.LoadData<DividedInvoiceDetailResponse, dynamic>(DividedInvoiceQueries.GetDividedInvoiceDetail, argument)).ToList();
        }

        public async Task<int> DeleteDividedInvoiceAsync(int invoiceId, string transactionCode, string deliveryId, string note)
        {
            var param = new
            {
                INVOICE_ID = invoiceId,
                TRANS_REF = transactionCode,
                DELIVER_ID = deliveryId,
                NOTE = note,
            };
            var affectedRow = await sqlDataAccess.ExecuteAsync(DividedInvoiceQueries.RevokeDividedInvoice, param,commandType:CommandType.StoredProcedure);
            return affectedRow;
        }

        public async Task<bool> CheckExistsDividedInvoice(int invoiceId)
        {
            var param = new { InvoiceId = invoiceId };
            return await sqlDataAccess.LoadSingleData<bool, dynamic>(DividedInvoiceQueries.CheckExistsDividedInvoice, param);
        }
        public async Task<int> SaveDividedInvoiceAsync(List<CreateDividedInvoiceRequest> requests, string dbCode, string userName)
        {
            var rowAffected = 0;
            foreach (var invoiceRequestDto in requests)
            {
                if (await CheckExistsDividedInvoice(invoiceRequestDto.InvoiceId)) continue;
                var insertParam = new
                {
                    DbCode = dbCode,
                    InvoiceId = invoiceRequestDto.InvoiceId,
                    DeliveryId = invoiceRequestDto.DeliveryId,
                    CreatedDate = invoiceRequestDto.CreatedDate,
                    CreatedBy = userName
                };
                var affected = await sqlDataAccess.ExecuteAsync(DividedInvoiceQueries.InsertDividedInvoice, insertParam);
                if (affected <= 0) continue;
                var updateParam = new { InvoiceId = invoiceRequestDto.InvoiceId };
                await sqlDataAccess.ExecuteAsync(DividedInvoiceQueries.UpdateNewInvoiceDividedStatus, updateParam);
                rowAffected += affected;
            }
            return rowAffected;
        }
        
        public async Task<List<DividedInvoiceSummaryResponse>> GetDividedInvoiceReportAsync(string dbCode, DateTime date)
        {
            var param = new
            {
                DB_CODE = dbCode,
                DATE = date,
            };
            var execute = await sqlDataAccess.LoadData<DividedInvoiceSummaryResponse, dynamic>(DividedInvoiceQueries.GetDividedInvoice, param);
            return execute.ToList();
        }
        public async Task<List<DividedInvoiceStatusResponse>> GetDividedInvoiceStatusByDateAsync(string dbCode, DateTime dividedDate,string deliveryId)
        {
            return (await sqlDataAccess.LoadData<DividedInvoiceStatusResponse, dynamic>
                (DividedInvoiceQueries.GetDividedInvoiceStatus, new { DB_CODE = dbCode, DIVIDED_DATE = dividedDate,DELIVERY_ID = deliveryId })).ToList();
        }
    }
}
