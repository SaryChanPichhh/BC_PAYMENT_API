namespace BC.PAYMENT.INFRASTRUCTURE.Repository.CommondityExchange.ReportDividedInvoice
{
    public class ReportDividedInvoiceRepository : IReportDividedInvoiceRepository
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ReportDividedInvoiceRepository(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<List<ReportDividedInvoiceDto>> GetReportDividedInvoiceAsync(string dbCode, DateTime fromDate, DateTime toDate)
        {
            var sql =
                $@"SELECT S.LAST_NAME + ' ' + S.FIRST_NAME Seller,INVOICE.InvNum InvoiceNumber,D.CREATED_DATE [Date],DL.DELIVERIES_NAME Delivery,TRANSACTION_REF [Transaction],CustomerCode,CustomerName,Market,Area,Store,SHIPPED_STATUS ShippedStatus,TOTAL Total
                FROM TB_BC_CHANGEINVOICE_DIVIDED_INVOICE D
            LEFT JOIN (SELECT S.ADD_CODE CustomerCode,STORE Store,S.ADD_LINE_1KH CustomerName,M.MARKET_KHMER_NAME Market,A.AREA_NAME_KHMER Area 
                       FROM SIADD S INNER JOIN TB_BCMARKET M ON M.MARKET_ID = S.MARKET_ID INNER JOIN TB_AREAS A ON A.AREA_ID = S.AREA_ID
                       WHERE S.DB_CODE = @DB_CODE AND M.DB_CODE = @DB_CODE AND A.DB_CODE = @DB_CODE
                       )  TAB1 ON TAB1.CustomerCode = D.CUSTOMER_CODE
			INNER JOIN TB_BCDELIVERIES DL ON DL.DELIVERIES_ID = D.DELIVERY_ID
            INNER JOIN 
		            (SELECT CONVERT(DATE,INVOICE_DATE) [Date],TRANSACTION_CODE TransactionCode,HEADER.USER_CODE UserCode, HEADER.INVOICE_NUMBER InvNum, HEADER.ID TbcId  FROM TB_BC_CHANGEINVOICE_EXCHANGE_INVOICE E
	  					INNER JOIN TB_BC_CHANGEINVOICE HEADER ON HEADER.ID = E.REQUEST_EXCHANGE_ID WHERE HEADER.DB_CODE = @DB_CODE​​ AND E.DB_CODE = @DB_CODE
					  UNION 
					  SELECT CONVERT(DATE,INVOICE_DATE),E.INVOICE_NUMBER,HEADER.USER_CODE UserCode, HEADER.INVOICE_NUMBER InvNum, HEADER.ID TbcId FROM 
					  TB_BC_CHANGEINVOICE HEADER INNER JOIN TB_BC_CHANGEINVOICE_DETAIL DETAIL ON DETAIL.CHANGE_INVOICE_ID = HEADER.ID
						INNER JOIN TB_BC_CHANGEINVOICE_RECEIVED RECEIVED ON RECEIVED.CHANGE_INVOICE_DETAIL_ID = DETAIL.ID
						INNER JOIN TB_BC_CHANGEINVOICE_REPAIR REPAIR ON REPAIR.RECEIVED_ID = RECEIVED.ID
						INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_COMPLETED COMPLETED ON COMPLETED.TRAN_REF = REPAIR.TRAN_REF
						INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE_DETAILS INVOICE_DETAIL ON INVOICE_DETAIL.TRANS_REF = COMPLETED.TRAN_REF
						INNER JOIN TB_BC_CHANGEINVOICE_REPAIR_INVOICE E ON E.INVOICE_NUMBER = INVOICE_DETAIL.INVOICE_ID WHERE HEADER.DB_CODE = @DB_CODE AND E.DB_CODE = @DB_CODE) INVOICE ON INVOICE.TransactionCode = D.TRANSACTION_REF
				INNER JOIN BCUSERS S ON INVOICE.UserCode = S.USER_ID
			WHERE D.CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE";
            var param = new
            {
                FROM_DATE = fromDate,
                TO_DATE = toDate,
                DB_CODE = dbCode,
            };
            var execute = await _sqlDataAccess.LoadData<ReportDividedInvoiceDto, dynamic>(sql, param);
            return execute.ToList();
        }
    }
}
