namespace BC.PAYMENT.CORE.Contracts.Response.StockCar;

public class TransferMoneyResponse
{
        public int Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Amount { get; set; }
        public double DollarFromEmployee { get; set; }
        public double RielFromEmployee { get; set; }
        public double ExchangeRateEmployee { get; set; }
        public double? DepositDollar { get; set; }
        public double? DepositRiel { get; set; }
        public double? DepositExchange { get; set; }
        public string Employee { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public double TotalFromEmployee => (ExchangeRateEmployee != 0 ? (RielFromEmployee / ExchangeRateEmployee) : 0) + DollarFromEmployee;
        public double? TotalDeposit => (DepositExchange.HasValue && DepositExchange.Value != 0 ? (DepositRiel / DepositExchange.Value) : 0) + DepositDollar;
        public double? BiasedAmount => TotalFromEmployee - (TotalDeposit ?? 0);
}
