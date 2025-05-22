
using BC.PAYMENT.APPLICATION.Interfaces.Transaction.ProvincialPayment.ReviewReport;
using BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.ReviewReport;
using BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.StockCarPayment;
using BC.PAYMENT.CORE.Entities.Transaction.ProvincialPayment.StockCarPayment;
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
        #region Credit Invoice
        public async Task<List<ReviewReportCreditInvoice>> GetAllCreditInvoiceByRequestIdAndCheckingStatus(int requestId,bool checkingStatus)
        {
            const string sql =
                $@"SELECT BCRIC.ID InvoiceId,CUSTOMER_CODE CustomerCode,CUSTOMER_NAME CustomerName,CODE TransactionCode,INVOICE_VALUE InvoiceValue,TRANSACTION_DATE TransactionDate,IS_CHECKED IsCheck
                FROM BCSTOCK_CAR BC INNER JOIN BC_STOCK_CAR_REQUESTS_INVOICE_CREDIT BCRIC ON BCRIC.INVOICE_ID = BC.ID WHERE REQUEST_ID = @RequestId AND BCRIC.IS_CHECKED = @IS_CHECK";
            var satus = checkingStatus ? "Yes" : "No";
            var param = new
            {
                RequestId = requestId,
                IS_CHECK = satus
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
                Status = status ,
                Id = submitId
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }
        #endregion


        #region Paid Invoice

        public async Task<List<PaidInvoiceRequestDto>> GetAllPaidInvoiceByRequestIdAndCheckingStatus(int requestId, bool status)
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
                CheckingStatus = status ? "Yes" : "No"
            };
            var execute = await _sqlDataAccess.LoadData<PaidInvoiceRequestDto, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> UpdateStatusCheckingPaymentInvoiceByInvoiceId(int invoiceId, bool status)
        {
            var sql =
                $@"UPDATE BC_STOCK_CAR_REQUESTS_INVOICE_PAID SET IS_CHECKED = @CheckingStatus WHERE INVOICE_ID = @InvoiceId";
            var param = new
            {
                CheckingStatus = status ? "Yes" : "No",
                InvoiceId = invoiceId
            };
            var affectedRow = await _sqlDataAccess.ExecuteAsync(sql, param);
            return affectedRow;
        }

        public async Task<List<HistoryPaymentInvoiceRespondDto>> GetAllHistoryPaymentInvoiceByTransactionCode(string transactionCode)
        {
            const string sql = $@"SELECT  ROW_NUMBER() OVER (ORDER BY BCIP.AMOUNT) RowNumber,BCIP.AMOUNT Amount,BCIP.CREATED_DATE CreateDate FROM BCSTOCK_CAR BC 
                                INNER JOIN BCSTOCK_CAR_INVOICE_PAYMENT BCIP ON BCIP.INVOICE_ID = BC.ID WHERE BC.CODE = @TRANSACTION_CODE";
            var param = new
            {
                TRANSACTION_CODE = transactionCode.ToUpper(),
            };
            var execute = await _sqlDataAccess.LoadData<HistoryPaymentInvoiceRespondDto, dynamic>(sql, param);
            return execute.ToList();
        }


        #endregion

        #region Transfer Money
        public async Task<List<TransferMoneyModel>> GetAllTransferByRequestIdAndCheckingStatus(int requestId, bool status)
        {
            const string sql =
                $@"SELECT ID,TransactionDate,Amount,Description,ISNULL(DepositDollar,0)DepositDollar,ISNULL(DepositRiel,0)DepositRiel,ISNULL(DepositExchange,0)DepositExchange,TotalDeposit,
                 ISNULL(DollarFromEmployee,0)DollarFromEmployee,ISNULL(RielFromEmployee,0)RielFromEmployee,ISNULL(ExchangeRateEmployee,0)ExchangeRateEmployee,ISNULL(TotalFromEmployee,0)TotalFromEmployee,ISNULL(TotalFromEmployee - TotalDeposit,0) BiasedAmount
                 FROM( SELECT BCRT.ID, TRANSFER_DATE TransactionDate, AMOUNT Amount, DESCRIPTION Description, DEPOSIT_DOLLAR DepositDollar, DEPOSIT_RIEL DepositRiel, DEPOSIT_EXCHANGE DepositExchange,
                 ISNULL((DEPOSIT_RIEL / DEPOSIT_EXCHANGE), 0) + DEPOSIT_DOLLAR TotalDeposit,DOLLAR_FROM_EMPLOYEE DollarFromEmployee, RIEL_FROM_EMPLOYEE RielFromEmployee,
                 EXCHANGE_FROM_EMPLOYEE ExchangeRateEmployee,ISNULL((RIEL_FROM_EMPLOYEE / EXCHANGE_FROM_EMPLOYEE), 0) + DOLLAR_FROM_EMPLOYEE TotalFromEmployee
                 FROM BC_STOCK_CAR_REQUESTS BCR INNER JOIN BC_STOCK_CAR_REQUESTS_TRANSFER BCRT ON BCRT.REQUEST_ID = BCR.REQUEST_ID WHERE BCR.REQUEST_ID = @RequestId AND IS_CHECKED = @CheckingStatus) TAB";
            var param = new
            {
                RequestId = requestId,
                CheckingStatus = status ? "Yes" : "No"
            };
            var excute = await _sqlDataAccess.LoadData<TransferMoneyModel, dynamic>(sql, param);
            return excute.ToList();
        }

        public async Task<int> UpdateStatusCheckingTransferBySubmitTransferId(int transferId, bool checkingStatus)
        {
            const string sql = "UPDATE BC_STOCK_CAR_REQUESTS_TRANSFER SET IS_CHECKED = @Status Where ID = @Id";
            string status = checkingStatus ? "Yes" : "No";
            var parameter = new
            {
                Status = status,
                Id = transferId
            };
            var affectedRow =await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }

        public async Task<List<ReviewReportExpenseDto>> GetAllExpenseByRequestIdAndCheckingStatus(int requestId, bool status)
        {
            const string sql =
                @"SELECT BCRE.ID Id,BCET.EXPENSE_NAME Description,QTY Quantity,UNIT_PRICE UnitPrice,AMOUNT_RIEL AmountRiel,AMOUNT_DOLLAR AmountDollar,P.PRO_NAME Province,EXCHANGE_RATE ExchangeRate,EXPENSE_DATE ExpenseDate
                 FROM BC_STOCK_CAR_REQUESTS_EXPENSE BCRE INNER JOIN BC_STOCK_CAR_REQUESTS BCR ON
                 BCR.REQUEST_ID = BCRE.REQUEST_ID INNER JOIN BCSTOCK_CAR_EXPENSE_TYPE BCET ON BCET.EXPENSE_ID = BCRE.EXPENSE_TYPE_ID
                 INNER JOIN PROVINCE P ON P.PROID = BCRE.PROVINCE_ID
                 WHERE BCRE.REQUEST_ID = @RequestId AND BCRE.IS_CHECKED = @CheckingStatus";
            string invoiceStatus = status ? "Yes" : "No";
            var param = new {
                RequestId = requestId,
                CheckingStatus = invoiceStatus
            };
            var execute = await _sqlDataAccess.LoadData<ReviewReportExpenseDto, dynamic>(sql, param);
            return execute.ToList();
        }

        public async Task<int> UpdateStatusCheckingExpenseByInvoiceId(int requestExpenseId, bool checkingStatus)
        {
            const string sql = "UPDATE BC_STOCK_CAR_REQUESTS_EXPENSE SET IS_CHECKED = @CheckingStatus WHERE ID = @requestSubmitId";
            string status = checkingStatus ? "Yes" : "No";
            var parameter = new
            {
                CheckingStatus = status,
                requestSubmitId = requestExpenseId
            };
            var affectedRow =await _sqlDataAccess.ExecuteAsync(sql, parameter);
            return affectedRow;
        }

        public async Task<List<ApprovalInvoiceHistoryModel>> GetAllApprovalHistoryByRequestIdAsync(int requestId,bool status)
        {
            const string sql = @"SELECT ID InvoiceId,TRANSACTION_DATE​ TransactionDate,CODE TransactionCode,CUSTOMER_CODE CustomerCode,CUSTOMER_NAME CustomerName,VALUE InvoiceValue,ISNULL(CASE WHEN VALUE > Payment.AmountPayment THEN Payment.AmountPayment END,0)  as HalfPayment,               
                 ISNULL(CASE WHEN VALUE = Payment.AmountPayment THEN Payment.AmountPayment END,0) as FullPayment,VALUE - Payment.AmountPayment TotalPayment,
                 CASE WHEN TYPE = 'N' THEN N'ថ្មី' WHEN TYPE = 'C' THEN N'ដូរ' WHEN TYPE = 'O' THEN N'ចាស់' END InvoiceType 
                 FROM BCSTOCK_CAR BC INNER JOIN (SELECT REQUEST_ID, INVOICE_ID, SUM(PAID_AMOUNT) AmountPayment FROM BC_STOCK_CAR_REQUESTS_INVOICE_PAID 
                 GROUP BY INVOICE_ID, REQUEST_ID) Payment On Payment.INVOICE_ID = BC.ID 
                 WHERE REQUEST_ID = @RequestId";
            string invoiceStatus = status ? "Yes" : "No";
            var param = new
            {
                RequestId = requestId,
                CheckingStatus = status
            };

            var execute = await _sqlDataAccess.LoadData<ApprovalInvoiceHistoryModel, dynamic>(sql, param);
            return execute.ToList();
        }

        #endregion
    }
}
