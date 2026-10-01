namespace BC.PAYMENT.INFRASTRUCTURE.Repository.Transaction.Submitting.SubmittingInvoice;

public class SubmittingPerDeliveryRepository(ISqlDataAccess sqlDataAccess) : ISubmittingPerDeliveryRepository
{
    public async Task<List<ExpenseDetailModel>> GetSubmittedInvoicesAsync(string dbCode, string fromDate, string toDate)
    {
        var sql =
            $@"SELECT ID,EXCHANGE ExchangeRate,D.DELIVERIES_NAME DeliveryName,TAB1.PAID SubTotal,SUM(DOLLAR) Dollar,SUM(RIEL) Riel,SUM(EXP_AMOUNT_1+EXP_AMOUNT_2+EXP_AMOUNT_3) ExpenseRiel,(SUM(EXP_AMOUNT_1+EXP_AMOUNT_2+EXP_AMOUNT_3)/EXCHANGE) ExpenseDollar,SUM(TOTAL) TOTAL,MONEY_BIAS Misaligned,EXPNSE_DESCRIPTION ExpenseDescription
                FROM BCPAYMENTDETAILA PD INNER JOIN TB_BCDELIVERIES D ON D.DELIVERIES_ID = PD.DELIVERYID INNER JOIN(SELECT D.DELIVERY_ID, SUM(P.AMOUNT) PAID FROM PC_DIVIDED_INVOICE D INNER JOIN PC_PAYMENT_INVOICE P ON P.DIVDIE_INVOICE_ID = D.DIVIDED_INVOICE_ID WHERE D.CREATE_DATE BETWEEN @FROM_DATE AND @TO_DATE AND P.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE GROUP BY D.DELIVERY_ID) TAB1
                ON TAB1.DELIVERY_ID = D.DELIVERIES_ID WHERE PD.CREATED_DATE BETWEEN @FROM_DATE AND @TO_DATE AND PD.DB_CODE = @DB_CODE AND ID NOT IN(SELECT PAID_DETAIL_ID FROM BCSUBMITTED_PAID_DETAIL WHERE DB_CODE = @DB_CODE)
                GROUP BY ID,EXCHANGE,D.DELIVERIES_NAME,MONEY_BIAS,EXPNSE_DESCRIPTION,TAB1.PAID";
        var param = new
        {
            DB_CODE = dbCode,
            FROM_DATE = fromDate,
            TO_DATE = toDate
        };
        var execute = await sqlDataAccess.LoadData<ExpenseDetailModel, dynamic>(sql, param);
        return execute.ToList();
    }

    public async Task<int> AddNewSubmittedInvoicesAsync(ExpenseDetailModel model)
    {
        var sql =
            $@"INSERT INTO BCSUBMITTED_PAID_DETAIL VALUES(@PAID_DETAIL_ID,@DOLLAR,@RIEL,@EXCHANGE,@TOTAL,@EXPENSE_RIEL,@MONEY_BIAS,@EXPENSE_DOLLAR
                       ,@STATUS,@DB_CODE,@SUBMITTED_DATE,@SUBMITTED_BY 
                       )";
        var param = new
        {
            PAID_DETAIL_ID = model.Id,
            DOLLAR = model.Dollar,
            RIEL = model.Riel,
            EXCHANGE = model.ExchangeRate,
            TOTAL = model.Total,
            EXPENSE_RIEL = model.ExpenseRiel,
            MONEY_BIAS = model.Misaligned,
            EXPENSE_DOLLAR = model.ExpenseDollar,
            STATUS = 1,
            DB_CODE = model.DbCode,
            SUBMITTED_DATE = model.CreateDate.ToString("yyyy-MM-dd"),
            SUBMITTED_BY = model.CreateBy
        };
        var affectedRow = await sqlDataAccess.ExecuteAsync(sql, param);
        return affectedRow;
    }
}