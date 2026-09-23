namespace BC.PAYMENT.API.Models.ExpenseTypes
{
    public class UpdateExpenseTypeRequest
    {
        public string? DbCode { get; set; }
        public string? ExpenseName { get; set; }
        public bool? Status { get; set; }
    }
}
