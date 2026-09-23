using IDividedInvoiceRepository = BC.PAYMENT.APPLICATION.Interfaces.Payment.IDividedInvoiceRepository;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Payment
{
    public class DividedInvoiceRepository(ISqlDataAccess sqlDataAccess) : IDividedInvoiceRepository
    {
        public async Task<List<CORE.Entities.Invoice.Invoice>> GetInvoices(IssueInvoiceExclusionFilterDTO dto)
        {
            var sql = "[PM_SELECT_TRANSACTIONS_WITH_EXCLUSION]";
            var param = new
            {
                DB_CODE = dto.DbCode,
                AREA_ID = dto.AreaId,
                EXCLUDED_TRANSACTION = dto.TransRef,
                OFF_SET = dto.Page,
                PAGESIZE = dto.PageSize,
            };
            var result = await sqlDataAccess.LoadData<CORE.Entities.Invoice.Invoice, dynamic>(sql, param, CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<CORE.Entities.Invoice.Invoice>> GetInvoices(IssueInvoiceFilterDTO dto)
        {
            var sql = "PM_SELECT_TRANSACTIONS";
            var param = new
            {
                DB_CODE = dto.DbCode,
                AREA_ID = dto.AreaId,
                OFF_SET = dto.Page,
                PAGESIZE = dto.PageSize,
            };
            var result = await sqlDataAccess.LoadData<CORE.Entities.Invoice.Invoice, dynamic>(sql, param, CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<int> SaveDividedInvoice(List<IssueInvoiceDTO> dto)
        {
            const string sql = "[PM_SAVE_DIVIDED_INVOICE]";

            var dataTable = AppExtension.ConvertToDataTable(dto);

            // Define the parameter for the stored procedure
            var param = new DynamicParameters();
            param.Add("@INVOICES", dataTable.AsTableValuedParameter("ISSUE_INVOICE")); // Specify table type name

            var rowAffected = await sqlDataAccess.ExecuteAsync(sql, param, CommandType.StoredProcedure);
            return rowAffected;
        }

        public async Task<List<Delivery>> GetDividedDeliveryInfo(string dbCode, DateTime date)
        {
            var sql = "PM_SELECT_DIVIDED_DELIVERY";
            var param = new
            {
                DB_CODE = dbCode,
                DATE = date
            };
            var result = await sqlDataAccess.LoadData<Delivery, dynamic>(sql, param, CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<CORE.Entities.Invoice.Invoice>> GetDividedInvoice(string dbCode,string deliveryId, DateTime date)
        {
            var sql = "PM_SELECT_DIVIDEDINVOICE";
            var param = new
            {
                DB_CODE = dbCode,
                DELIVERY_ID = deliveryId,
                DATE = date
            };
            var result = await sqlDataAccess.LoadData<CORE.Entities.Invoice.Invoice, dynamic>(sql, param, CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<DividedInvoiceSummary>> GetDividedInvoiceSummary(string dbCode,DateTime date)
        {
            var sql = "PM_SELECT_INVOICE_SUMMARY";
            var param = new
            {
                DB_CODE = dbCode,
                DATE = date
            };
            var result = await sqlDataAccess.LoadData<DividedInvoiceSummary, dynamic>(sql, param, CommandType.StoredProcedure);
            return result.ToList();
        }
    }
}
