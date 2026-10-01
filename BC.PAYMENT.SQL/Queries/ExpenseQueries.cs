namespace BC.PAYMENT.SQL.Queries;

public static class ExpenseQueries
{
    public const string CreatePaymentExpense =
        @"INSERT INTO PAYMENT_INVOICE_EXPENSE(DB_CODE,PAYMENT_HEADER_ID,NAME,DESCRIPTION,QUANTITY,UNIT_PRICE,TOTAL,CREATED_DATE,CREATED_BY,CURRENCY_TYPE,EXCHANGE_RATE)
        VALUES(@DB_CODE,@PAYMENT_HEADER_ID,@NAME,@DESCRIPTION,@QUANTITY,@UNIT_PRICE,@TOTAL,@CREATED_DATE,@CREATED_BY,@CURRENCY_TYPE,@EXCHANGE_RATE)";

    public static string GetBcPaymentDetail(string criteria, string addReference = "", string groupBy = "")
    {
        return $@"
        SELECT ID Id,
            EXP.DB_CODE AS DbCode,
            D.DELIVERIES_KHMER AS DeliveryName,
            D.DELIVERIES_ID AS DeliveryId,
            ISNULL(EXP.TOTAL, 0) AS Total,
            ISNULL(EXP.DOLLAR, 0) AS Dollar,
            ISNULL(EXP.RIEL, 0) AS Riel,
            ISNULL(EXP.EXCHANGE, 0) AS Exchange,
            ISNULL(EXP.DESC_EXP_1, '') AS DescExp1,
            ISNULL(EXP.EXP_AMOUNT_1, 0) AS ExpAmount1,
            ISNULL(EXP.DESC_EXP_2, '') AS DescExp2,
            ISNULL(EXP.EXP_AMOUNT_2, 0) AS ExpAmount2,
            ISNULL(EXP.DESC_EXP_3, '') AS DescExp3,
            ISNULL(EXP.EXP_AMOUNT_3, 0) AS ExpAmount3,
            ISNULL(EXP.MONEY_BIAS, 0) AS MoneyBias,
            EXP.CREATED_DATE AS CreatedDate,
            ISNULL(EXP.CREATED_BY, '') AS CreatedBy,
            ISNULL(EXP.ENTRIES_CODE, '') AS EntriesCode
        FROM dbo.BCPAYMENTDETAILA EXP INNER JOIN TB_BCDELIVERIES D ON D.DELIVERIES_ID = EXP.DELIVERYID
         {addReference}
        WHERE EXP.DB_CODE = @DB_CODE
          {criteria}
        ORDER BY CREATED_DATE DESC ;
";
    }

    public const string UpdateBcPaymentDetail = @"
        UPDATE BCPAYMENTDETAILA 
        SET TOTAL = @TT,
            DOLLAR = @D,
            RIEL = @R,
            EXCHANGE = @E,
            DESC_EXP_1 = @D1,
            DESC_EXP_2 = @D2,
            DESC_EXP_3 = @D3,
            EXP_AMOUNT_1 = @EA1,
            EXP_AMOUNT_2 = @EA2,
            EXP_AMOUNT_3 = @EA3,
            MONEY_BIAS = @B 
        WHERE ID = @ID;";

    public const string InsertSubmitExpense = @"
        INSERT INTO BCSUBMITTED_PAID_DETAIL (PAID_DETAIL_ID, DOLLAR, RIEL, EXCHANGE, TOTAL, EXPENSE_RIEL, MONEY_BIAS, EXPENSE_DOLLAR, STATUS, DB_CODE, SUBMITTED_DATE, SUBMITTED_BY)
        VALUES (@PAID_DETAIL_ID, @DOLLAR, @RIEL, @EXCHANGE, @TOTAL, @EXPENSE_RIEL, @MONEY_BIAS, @EXPENSE_DOLLAR, @STATUS, @DB_CODE, @SUBMITTED_DATE, @SUBMITTED_BY);";

    public static string GetSubmitExpenseDetail(string criteria, string addOnField = "", string addReference = "")
    {
        return
            $@"SELECT D.SUBMITTED_ID Id,DL.DELIVERIES_KHMER DeliveryName,P.CREATED_DATE Date,D.SUBMITTED_BY SubmittedBy,D.DOLLAR Dollar
        ,D.RIEL Riel,D.TOTAL SubTotal,D.MONEY_BIAS MoneyBias,D.EXPENSE_DOLLAR ExpenseDollar,D.EXPENSE_RIEL ExpenseRiel
        ,D.TOTAL - D.MONEY_BIAS/D.EXCHANGE-D.EXPENSE_DOLLAR Total,D.EXCHANGE Exchange,N'កំពុងដំណើរការ' Other,D.SUBMITTED_DATE SubmittedDate,
        EXPNSE_DESCRIPTION ExpenseDesc,P.DESC_EXP_1 ExpenseDesc1,P.DESC_EXP_2 ExpenseDesc2,P.DESC_EXP_3 ExpenseDesc3 {addOnField}
        FROM BCSUBMITTED_PAID_DETAIL D INNER JOIN BCPAYMENTDETAILA P ON P.ID = D.PAID_DETAIL_ID
         INNER JOIN TB_BCDELIVERIES DL ON DL.DELIVERIES_ID = P.DELIVERYID
         {addReference}
          {criteria}";
    }

    public const string UpdateSubmitExpenseDescription = @"
        UPDATE P SET DESC_EXP_1 = @DESC_EXP_1,DESC_EXP_2=@DESC_EXP_2,DESC_EXP_3=@DESC_EXP_3 FROM BCPAYMENTDETAILA P INNER JOIN BCSUBMITTED_PAID_DETAIL D
                       ON P.ID = D.PAID_DETAIL_ID WHERE D.SUBMITTED_ID = @SUBMITTED_ID;";

    public const string DeleteSubmitExpense =
        @"DELETE FROM BCSUBMITTED_PAID_DETAIL WHERE SUBMITTED_ID = @SUBMITTED_ID;";
}