using BC.PAYMENT.CORE.Contracts.Request.Invoice;
using BC.PAYMENT.CORE.Contracts.Response.Invoice;
using BC.PAYMENT.CORE.Entities;
using NewInvoiceModel = BC.PAYMENT.CORE.Entities.Invoice.NewInvoiceModel;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Invoice
{
    public class NewInvoiceRepository(ISqlDataAccess sqlDataAccess, ICustomerRepository customerRepository,IDbConnection connection) : INewInvoiceRepository
    {
        public async Task<List<NewInvoiceResponse>> GetInvoices(InvoiceStatus type, string dbCode, DateTime createDate)
        {
            var param = new
            {
                DB_CODE = dbCode,
                CREATED_DATE = createDate.Date,
                STATUS = type switch
                {
                    InvoiceStatus.NewInvoice => "N",
                    InvoiceStatus.OldInvoice => "O",
                    InvoiceStatus.ChangeInvoice => "C",
                    _ => "N"
                }
            };
            var invoicesModels = await sqlDataAccess.LoadData<NewInvoiceResponse, dynamic>(NewInvoiceQueries.GetInvoices, param);
            return invoicesModels.ToList();
        }
        public async Task<List<NewInvoiceResponse>> GetInvoicesByInvoiceCode(string startInvoiceCode, string endInvoiceCode, DateTime fromDate, DateTime endDate,
            string dbCode)
        {
            var param = new
            {
                CODE_1 = startInvoiceCode,
                CODE_2 = endInvoiceCode,
                FROM_DATE = fromDate.ToString("MM/dd/yyyy"),
                TO_DATE = endDate.ToString("MM/dd/yyyy"),
                DB_CODE = dbCode,
            };
            var execute = await connection.QueryAsync(NewInvoiceQueries.SelectNewInvoiceDate(dbCode), param, commandType : CommandType.StoredProcedure);
            var rows = execute
                .Cast<IDictionary<string, object>>()
                .ToList().Select(x=> new NewInvoiceResponse
                {
                    InvoiceCode = x["CODE"].ToString(),
                    CustomerCode = x["Customer Code"].ToString(),
                    CustomerName = x["Customer Name"].ToString(),
                    InvoiceAmount = (x["Header Transaction Value"]).ToDouble(),
                    CreatedDate = x["Header Sale Order Date"].ToDateTime()
                }).ToList();
            Console.WriteLine(rows);
            var customers = await customerRepository.GetCustomer();
            var invoice = customers.Join(rows,
                invoice => invoice.CustomerCode,
                customer => customer.CustomerCode,
                (customer, invoice) => new NewInvoiceResponse
                {
                    InvoiceCode = invoice.InvoiceCode,
                    CustomerName = customer.CustomerName,
                    CustomerCode = customer.CustomerCode,
                    InvoiceAmount = invoice.InvoiceAmount,
                    Market = customer.Market,
                    Area = customer.Area,
                    Store = customer.Store,
                    CreatedDate = invoice.CreatedDate
                }).ToList();
            return invoice;
        }
        
        public async Task<int> SaveInvoices(List<NewInvoiceModel> invoices)
        {
            var affectedRow = 0;
            foreach (var param in from invoicesModel in invoices
                                  let invoiceStatus = invoicesModel.InvoiceType switch
                                  {
                                      InvoiceStatus.NewInvoice => "N",
                                      InvoiceStatus.OldInvoice => "O",
                                      InvoiceStatus.ChangeInvoice => "C",
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
                affectedRow += await sqlDataAccess.ExecuteAsync(NewInvoiceQueries.SaveInvoices, param);
            }
            return affectedRow;
        }

        public async Task<int> UpdateHeaderValueAsync(PcEditDividedInvoice request)
        {

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                var insertParam = new
                {
                    DB_CODE = request.DbCode,
                    DIVIDED_ID = request.DividedId,
                    OLD_VALUE = request.OldAmount,
                    NEW_VALUE = request.NewAmount,
                    DESCRIPTION = request.Description,
                    CREATED_BY = request.CreatedBy
                };

                var executeAsync = await connection.ExecuteAsync(NewInvoiceQueries.InsertEditValueDividedInvoice, insertParam, transaction);
                if (executeAsync > 0)
                {
                    var updateParam = new
                    {
                        InvoiceValue = request.NewAmount,
                        ID = request.DividedId
                    };
                    await connection.ExecuteAsync(NewInvoiceQueries.UpdateNewInvoiceHeaderValue, updateParam, transaction);
                    transaction.Commit();
                    return executeAsync;
                }
                else
                {
                    transaction.Rollback();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Debug.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<int> DeleteInvoiceAsync(int invoiceId)
        {
            return await sqlDataAccess.ExecuteAsync(NewInvoiceQueries.DeleteInvoice, new {INVOICE_ID = invoiceId}); 
        }
    }
}
