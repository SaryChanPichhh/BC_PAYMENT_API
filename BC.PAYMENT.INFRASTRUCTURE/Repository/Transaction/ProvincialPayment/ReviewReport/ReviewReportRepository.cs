

using Azure.Core;
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.ProvincialPayment.ReviewReport;
using BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.ReviewReport;
using BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.StockCarPayment;
using BC.PAYMENT.INFRASTRUCTURE.DBAccess;

namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.ProvincialPayment.ReviewReport
{
    public class ReviewReportRepository : IReviewReportRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ReviewReportRepository(ISqlDataAccess sqlDataAccess) 
        {
            _sqlDataAccess = sqlDataAccess;
        }
        public async Task<List<RequestionActionDto>> GetAllActionAsync()
        {
            const string sql = "SELECT ID,NAME FROM BC_STOCK_CAR_REQUESTS_ACTION";
            var execute = await _sqlDataAccess.LoadData<RequestionActionDto,dynamic>(sql,new {});
            return execute.ToList();
        }

        public async Task<List<StockCarRequestDto>> GetAllRequestByActionIdAsync(string dbCode,int actionId)
        {
            const string sql = @"SELECT BCR.REQUEST_ID RequestId,UPPER(USER_NAME) Employee,TITLE Title,ACTION_ID Action,T.CreatedDate Date FROM BC_STOCK_CAR_REQUESTS BCR INNER JOIN TEMPLATE T ON T.Id = BCR.TEMPLATE_ID
                INNER JOIN(SELECT USER_NAME, USER_ID FROM BCUSERS ) [USER] ON [USER].USER_ID = T.Employee
               WHERE ACTION_ID = @ActionId AND CURRENT_STATE_ID = (select ROLE_ID from BCSTOCK_CAR_ROLES where UPPER(ROLE_NAME) = 'ADMIN' OR UPPER(ROLE_NAME) = 'ADMINISTRATOR')"
                ;
            var parameter = new
            {
                DbCode = dbCode,
                Actionid = actionId,
            };
            var execute = await _sqlDataAccess.LoadData<StockCarRequestDto, dynamic>(sql, parameter);
            return execute.ToList();
        }
        public async Task<List<StockCarRequestDto>> GetAllRequestByActionIdAndRoleIdAsync(string dbCode,int actionId, List<int> roleId)
        {
            const string sql = @" SELECT BCR.REQUEST_ID RequestId, USER_NAME Employee,TITLE Title, ACTION_ID Action FROM BC_STOCK_CAR_REQUESTS BCR INNER JOIN
            TEMPLATE T ON T.Id = BCR.TEMPLATE_ID
            INNER JOIN (SELECT USER_NAME, USER_ID FROM BCUSERS ) [USER] ON [USER].USER_ID = T.Employee WHERE ACTION_ID = @ActionId And CURRENT_STATE_ID IN @Stated";
            var newRoleId = roleId.Select(x => x.ToString());
            var parameter = new
            {
                DbCode = dbCode,
                Actionid = actionId,
                Stated = roleId
            };
            var execute = await _sqlDataAccess.LoadData<StockCarRequestDto, dynamic>(sql, parameter);
            return execute.ToList();
        }

        public async Task<List<ReviewReportCreditInvoice>> GetAllCreditInvoiceByRequestIdAndCheckingStatus(int requestId)
        {
            const string sql =
                $@"SELECT BCRIC.ID,CUSTOMER_CODE,CUSTOMER_NAME,CODE,INVOICE_VALUE,TRANSACTION_DATE,IS_CHECKED
                FROM BCSTOCK_CAR BC INNER JOIN BC_STOCK_CAR_REQUESTS_INVOICE_CREDIT BCRIC ON BCRIC.INVOICE_ID = BC.ID
                WHERE REQUEST_ID = @RequestId";
            var param = new
            {
                RequestId = requestId
            };
            var execute = await _sqlDataAccess.LoadData<ReviewReportCreditInvoice, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> UpdateStatusCheckingCreditInvoiceBySubmitId(int submitId, bool checkingStatus)
        {
            const string sql = "UPDATE BC_STOCK_CAR_REQUESTS_INVOICE_CREDIT SET IS_CHECKED = @Status WHERE ID = @Id ";
            string status = checkingStatus ? "Yes" : "No";
            var parameter = new
            {
                Status = status,
                Id = submitId
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }

        public async Task<List<PaidInvoiceRequestDto>> GetAllPaymentInvoiceByRequestIdAndCheckingStatus(int requestId, bool status)
        {
            var sql =
                $@" SELECT ID InvoiceId,TRANSACTION_DATE TransactionDate,CODE TransactionCode,CUSTOMER_CODE CustomerCode,CUSTOMER_NAME CustomerName,
			   VALUE InvoiceValue,ISNULL(CASE WHEN VALUE > Payment.AmountPayment THEN Payment.AmountPayment END,0) HalfPayment,
                 ISNULL(CASE WHEN VALUE = Payment.AmountPayment THEN Payment.AmountPayment END,0) FullPayment,VALUE - Payment.AmountPayment Total,
                 CASE WHEN TYPE = 'N' THEN N'ថ្មី' WHEN TYPE = 'C' THEN N'ដូរ' WHEN TYPE = 'O' THEN N'ចាស់' END InvoiceType
               FROM BCSTOCK_CAR BC INNER JOIN
                 (SELECT REQUEST_ID, INVOICE_ID, SUM(PAID_AMOUNT) AmountPayment FROM BC_STOCK_CAR_REQUESTS_INVOICE_PAID WHERE IS_CHECKED = @CheckingStatus
                 GROUP BY INVOICE_ID, REQUEST_ID) Payment On Payment.INVOICE_ID = BC.ID
                 WHERE REQUEST_ID = @RequestId";
            var param = new
            {
                RequestId = requestId,
                CheckingStatus = status
            };
            var execute = await _sqlDataAccess.LoadData<PaidInvoiceRequestDto, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
