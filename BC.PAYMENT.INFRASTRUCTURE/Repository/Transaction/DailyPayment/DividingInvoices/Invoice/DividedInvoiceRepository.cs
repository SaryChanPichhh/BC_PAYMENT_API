using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.CORE.DTO.Filter;
using BC.PAYMENT.CORE.DTO.Invoice;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Entities.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class DividedInvoiceRepository : IDividedInvoiceRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public DividedInvoiceRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<DividedInvoiceModel>> GetDividedInvoicesByDeliveryIdAndDateAsync(string dbCode, string deliveryId, DateTime date)
        {
            var sql = $@"SELECT N.ID,DE.DELIVERIES_KHMER,
                     N.CUSTOMER_CODE,
                    N.ACC_NAME_KH,
                     Customer.STORE,
                     N.TRANSACTION_REF, 
                     '1.00' AS 'AMOUNT',
                     N.HEADER_TRANSACTION_VALUES,
                     CASE WHEN N.STATUS = 'N' THEN N'ប៉ុងថ្មី' WHEN N.STATUS = 'C' THEN N'ប៉ុងដូរ' else N'ប៉ុងចាស់' End 'Type'
                     FROM PC_DIVIDED_INVOICE D
                     INNER JOIN NEW_INVOICE N ON N.ID = D.INVOICE_ID
                     INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
                     LEFT JOIN(SELECT ADD_CODE, STORE FROM SIADD where DB_CODE =
                     @DB_CODE) Customer on Customer.ADD_CODE = N.CUSTOMER_CODE
                     WHERE N.IS_DIVIDED = '0'
                     AND DE.DELIVERIES_ID = @DELIVERY_ID
                     AND D.CREATE_DATE = @DATE
                     AND D.DB_CODE = @DB_CODE";
            var param = new
            {
                DB_CODE = dbCode,
                DELIVERY_ID = deliveryId,
                DATE = date,
            };
            var execute = await _sqlDataAccess.LoadData<DividedInvoiceModel, dynamic>(sql, param);

            return execute.ToList();
        }

        public async Task<int> DeleteDividedInvoiceAsync(string invoiceId, string transactionCode, string deliveryId, string note)
        {
            var procedure = $@"PC_REVOKE_DIVIDED_INVOICE";
            var param = new
            {
                INVOICE_ID = invoiceId,
                TRANS_REF = transactionCode,
                DELIVER_ID = deliveryId,
                NOTE = note,
            };

            var affectedRow = await _sqlDataAccess.ExecuteAsync(procedure, param);
            return affectedRow;
        }
    }
}
