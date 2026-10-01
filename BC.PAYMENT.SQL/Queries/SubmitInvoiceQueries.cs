namespace BC.PAYMENT.SQL.Queries;

public static class SubmitInvoiceQueries
{
    public static string GetApprovedSubmittedInvoice(string criteria, string addReference = "", string addOnField = "")
    {
        return $@"SELECT S.ID Id,DE.DELIVERIES_KHMER DeliveryName,
                N.CUSTOMER_CODE CustomerCode,
                CUSTOMER.CUSTOMER_NAME CustomerName,
                N.TRANSACTION_REF InvoiceCode,1 Amount,
                MARKET Market,AREA Area
                ,S.MONEY Money,S.PAID Paid,
                D.CREATE_DATE DividedDate,
                A.APPROVAL_STATUS StatusDesc,
                A.DESCRIPTION Description,
                A.APPROVAL_DATE ApprovedDate
                FROM BCINVOICE_SUMITTED S
                INNER JOIN NEW_INVOICE N ON N.ID = S.INVOICE_ID
                INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
                INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
                INNER JOIN BCAPPROVAL_INVOICE A ON A.SUBMITTED_ID = S.ID
                LEFT JOIN (SELECT ADD_CODE CUSTOMER,ADD_LINE_1KH CUSTOMER_NAME,
                AREA_NAME_KHMER AREA,MARKET_KHMER_NAME MARKET 
                FROM SIADD INNER JOIN TB_BCMARKET M ON M.MARKET_ID = SIADD.MARKET_ID
                INNER JOIN TB_AREAS A ON A.AREA_ID = M.AREA_ID 
                WHERE SIADD.DB_CODE = @DB_CODE
                AND M.DB_CODE  = @DB_CODE AND A.DB_CODE = @DB_CODE) 
                CUSTOMER ON CUSTOMER.CUSTOMER = N.CUSTOMER_CODE
                WHERE
                S.DB_CODE = @DB_CODE AND N.DB_CODE = @DB_CODE AND D.DB_CODE = @DB_CODE
                AND DE.DB_CODE = @DB_CODE
                {criteria}";
    }

    public static string GetPendingSubmittedInvoice =>
        $@"SELECT S.ID Id,
            DE.DELIVERIES_KHMER DeliveryName,
            CUS.*,
            N.TRANSACTION_REF InvoiceCode,1 Amount,
            S.MONEY Money,S.PAID Paid,
            S.MONEY - S.PAID Total,
            D.CREATE_DATE SubmittedDate,
            S.STATUS Status
            FROM BCINVOICE_SUMITTED S
            INNER JOIN NEW_INVOICE N
            ON N.ID = S.INVOICE_ID
            INNER JOIN PC_DIVIDED_INVOICE D ON D.INVOICE_ID = N.ID
            INNER JOIN TB_BCDELIVERIES DE ON DE.DELIVERIES_ID = D.DELIVERY_ID
            LEFT JOIN GET_CUSTOMERS_BY_DB_CODE() CUS ON CUS.CustomerCode = N.CUSTOMER_CODE
            WHERE S.DB_CODE = @DB_CODE
            AND N.DB_CODE = @DB_CODE
            AND D.DB_CODE = @DB_CODE
            AND S.SUBMISSION_STATUS != 'Cancel'
            AND D.CREATE_DATE BETWEEN @FROM_DATE
            AND @TO_DATE
            AND S.ID NOT IN(SELECT SUBMITTED_ID FROM BCAPPROVAL_INVOICE WHERE DB_CODE = @DB_CODE)";

    public static string GetPaymentHistoryInvoice =>
        $@"SELECT TRANSACTION_REF, 
                        HEADER_TRANSACTION_VALUES, AMOUNT, HEADER_TRANSACTION_VALUES - AMOUNT TOTAL,  PPI.CREATED_DATE,
                        CASE WHEN A.SUBMITTED_ID IS NULL THEN 'Submitted' ELSE 'Approved' END [STATUS] 
                        FROM NEW_INVOICE NI
                        INNER JOIN PC_DIVIDED_INVOICE PDI on NI.ID = PDI.INVOICE_ID and PDI.DB_CODE = @DB_CODE
                        INNER JOIN PC_PAYMENT_INVOICE PPI ON PPI.DIVDIE_INVOICE_ID = PDI.DIVIDED_INVOICE_ID and PPI.DB_CODE = @DB_CODE
			            INNER JOIN BCINVOICE_SUMITTED S ON S.INVOICE_ID = NI.ID and S.DB_CODE = @DB_CODE
			            LEFT JOIN BCAPPROVAL_INVOICE A ON A.SUBMITTED_ID = S.ID
                        WHERE  NI.DB_CODE = @DB_CODE AND PDI.CREATE_DATE <=@DATE AND NI.TRANSACTION_REF = @TRANSACTION
                        ORDER BY TOTAL ;";
}