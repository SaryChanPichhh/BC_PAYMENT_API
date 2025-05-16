namespace BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DailyPayment
{
    public class ExpenseDetailDto
    {
        public int Id { get; set; }
        public double Total { get; set; }
        public double Dollar { get; set; }
        public double Riel { get; set; }
        public double Exchange { get; set; }
        public double Description1 { get; set; }
        public double Description2 { get; set; }
        public double Description3 { get; set; }
        public double Expense1 { get; set; }
        public double Expense2 { get; set; }
        public double Expense3 { get; set; }
        public double Misaligned { get; set; }
    }
}
