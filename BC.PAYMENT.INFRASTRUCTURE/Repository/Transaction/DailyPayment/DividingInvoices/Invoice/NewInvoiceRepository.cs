using System.Data;
using BC.PAYMENT.APPLICATION.Interfaces.General;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.DividingInvoices.Invoice;
using BC.PAYMENT.CORE.Enums;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class NewInvoiceRepository : INewInvoiceRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly ICustomerRepository _customerRepository;
        public NewInvoiceRepository(ISqlDataAccess sqlDataAccess, ICustomerRepository customerRepository)
        {
            _sqlDataAccess = sqlDataAccess;
            _customerRepository = customerRepository;
        }

        public async Task<List<NewInvoiceModel>> GetInvoices(InvoiceTypes type, string dbCode, DateTime createDate)
        {
            const string sql = @"SELECT ID InvoiceId,TRANSACTION_REF InvoiceCode,CUS.*,HEADER_TRANSACTION_VALUES InvoiceAmount,
                        CASE WHEN N.STATUS = 'C' THEN 'ChangeInvoice'
                        WHEN N.STATUS = 'N' THEN 'NewInvoice'
                        WHEN N.STATUS = 'O' THEN 'OldInvoice' END [InvoiceType],
                        CREATED_DATE CreatedDate,
                        CREATED_BY CreatedBy,
                        IS_DIVIDED IsDivided,
                        ENTRIES_CODE EntriesCode
                        FROM NEW_INVOICE N LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = N.CUSTOMER_CODE
                        WHERE N.DB_CODE = @DB_CODE AND STATUS = @STATUS AND N.CREATED_DATE = @CREATED_DATE";
            var param = new
            {
                DB_CODE = dbCode,
                CREATED_DATE = createDate.Date,
                STATUS = type switch
                {
                    InvoiceTypes.NewInvoice => "N",
                    InvoiceTypes.OldInvoice => "O",
                    InvoiceTypes.ChangeInvoice => "C",
                    _ => "N"
                }
            };
            var invoicesModels = await _sqlDataAccess.LoadData<NewInvoiceModel, dynamic>(sql, param);
            return invoicesModels.ToList();
        }
        public async Task<List<NewInvoiceModel>> GetInvoicesByInvoiceCode(string startInvoiceCode, string endInvoiceCode, string fromDate, string endDate,
            string dbCode)
        {
            var procedure = $@"${dbCode}SELECT_NEW_INVOICE_DATE";
            var param = new
            {
                CODE_1 = startInvoiceCode,
                CODE_2 = endInvoiceCode,
                FROM_DATE = fromDate,
                TO_DATE = endDate,
                DB_CODE = dbCode,
            };
            var execute = await _sqlDataAccess.LoadData<NewInvoiceModel, dynamic>(procedure, param, CommandType.StoredProcedure);

            var customers = await _customerRepository.GetCustomer();
            var invoice = customers.Join(execute.ToList(),
                invoice => invoice.CustomerCode,
                customer => customer.CustomerCode,
                (customer, invoice) => new NewInvoiceModel()
                {
                    InvoiceCode = invoice.InvoiceCode,
                    CustomerName = customer.CustomerName,
                    CustomerCode = customer.CustomerCode,
                    InvoiceAmount = invoice.InvoiceAmount,
                    Market = customer.Market,
                    Area = customer.Area,
                    Store = customer.Store
                }).ToList();

            return invoice;
        }

        public async Task<int> SaveInvoices(List<NewInvoiceModel> invoices)
        {
            var afftectedRow = 0;
            const string sql =
                @"
                IF NOT EXISTS (SELECT * FROM NEW_INVOICE WHERE TRANSACTION_REF = @INVOICE_CODE AND CREATED_DATE = CONVERT(DATE,GETDATE()) 
                AND STATUS = @STATUS AND DB_CODE = @DB_CODE AND IS_DIVIDED = 1)
                    INSERT INTO NEW_INVOICE(DB_CODE,TRANSACTION_REF,CUSTOMER_CODE,ACC_NAME_KH,HEADER_TRANSACTION_VALUES,STATUS,CREATED_DATE,CREATED_BY,IS_DIVIDED,ENTRIES_CODE)
                             VALUES(@DB_CODE,@INVOICE_CODE,@CUSTOMER_CODE,@CUSTOMER_NAME,@INVOICE_VALUE,@STATUS,@CREATED_DATE,@CREATED_BY,@IS_DIVIDED,@ENTRIES_CODE)";
            foreach (var param in from invoicesModel in invoices
                                  let invoiceStatus = invoicesModel.InvoiceTypes switch
                                  {
                                      InvoiceTypes.NewInvoice => "N",
                                      InvoiceTypes.OldInvoice => "O",
                                      InvoiceTypes.ChangeInvoice => "C",
                                      _ => "N"
                                  }
                                  select new
                                  {
                                      DB_CODE = invoicesModel.DbCode,
                                      INVOICE_CODE = invoicesModel.InvoiceCode,
                                      CUSTOMER_CODE = invoicesModel.CustomerCode,
                                      CUSTOMER_NAME = invoicesModel.CustomerName,
                                      INVOICE_VALUE = invoicesModel.InvoiceAmount,
                                      STATUS = invoiceStatus,
                                      CREATED_DATE = DateTime.Now,
                                      CREATED_BY = invoicesModel.CreatedBy,
                                      IS_DIVIDED = true,
                                      ENTRIES_CODE = invoicesModel.EntriesCode
                                  })
            {
                afftectedRow += await _sqlDataAccess.ExecuteAsync(sql, param);
            }
            return afftectedRow;
        }
        public async Task<List<string>> GetSaleTypes(string dbCode)
        {
            var sql = $@"SELECT CODE FROM SIDATA WHERE DB_CODE = @DB_CODE AND SI_TYPE  = 'SALES'";
            var param = new
            {
                DB_CODE = dbCode,
            };
            var execute = await _sqlDataAccess.LoadData<string, dynamic>(sql, param);
            return execute.ToList();
        }

    }
}
