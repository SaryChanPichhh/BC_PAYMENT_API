namespace BC.PAYMENT.SQL.Queries
{
    public static class ExpenseTypeQueries
    {
        public static string GetExpenseTypes => 
            "SELECT EXPENSE_ID AS ExpenseId, DB_CODE AS DbCode, EXPENSE_NAME AS ExpenseName, STATUS AS Status, USER_CREATED AS UserCreated, CREATED_DATE AS CreatedDate FROM BCSTOCK_CAR_EXPENSE_TYPE WHERE DB_CODE = @DB_CODE";

        public static string CreateExpenseType => @"
            INSERT INTO BCSTOCK_CAR_EXPENSE_TYPE ( DB_CODE, EXPENSE_NAME, STATUS, USER_CREATED, CREATED_DATE)
            VALUES ( @DbCode, @ExpenseName, @Status, @UserCreated, @CreatedDate)";

        public static string UpdateExpenseType => @"
            UPDATE BCSTOCK_CAR_EXPENSE_TYPE 
            SET DB_CODE = @DbCode, 
                EXPENSE_NAME = @ExpenseName, 
                STATUS = @Status
            WHERE EXPENSE_ID = @ExpenseId";

        public static string DeleteExpenseType => 
            "DELETE FROM BCSTOCK_CAR_EXPENSE_TYPE WHERE EXPENSE_ID = @ExpenseId";
    }
}
