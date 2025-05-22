using System.ComponentModel.DataAnnotations;

namespace BC.PAYMENT.CORE.Entities.Transaction.ProvincialPayment.StockCarPayment
{
    public class TransferMoneyModel
    {
        public int Id { get; set; }
        [Required]
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
        public int Amount { get; set; }
        public double DollarFromEmployee { get; set; }
        public double RielFromEmployee { get; set; }
        public double ExchangeRateEmployee { get; set; }
        public double TotalFromEmployee => RielFromEmployee / ExchangeRateEmployee + DollarFromEmployee;
        public DateTime ClosingDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? DbCode { get; set; }
        public string? Employee { get; set; }
        public int EmployeeId { get; set; }
        public int TemplateId { get; set; }
        public double DepositDollar { get; set; }
        public double DepositRiel { get; set; }
        public double DepositExchange { get; set; }
        public double TotalDeposit => DepositRiel / DepositExchange + DepositDollar;
        public double BiasedAmount => TotalFromEmployee - TotalDeposit;
    }
}
