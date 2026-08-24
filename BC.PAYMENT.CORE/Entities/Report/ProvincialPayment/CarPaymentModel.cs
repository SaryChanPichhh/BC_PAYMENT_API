namespace BC.PAYMENT.CORE.Entities.Report.ProvincialPayment
{
    public class CarPaymentModel
    {
        public class CarPaymentCreditInvoiceModel
        {
            public DateTime ClosingDate { get; set; }
            public string CustomerCode { get; set; }
            public string CustomerName { get; set; }
            public string TransactionRef { get; set; }
            public double Value { get; set; }
            public string Invoice { get; set; }
            public string Credit { get; set; }
            public string Return { get; set; }
            public string Description { get; set; }
        }

        public class CarPaymentInvoiceModel : Customer
        {
            public DateTime CreateDate { get; set; }
            public string TransactionRef { get; set; }
            public double Value { get; set; }
            public string Invoice { get; set; }
            public string Credit { get; set; }
            public string Return { get; set; }
            public string Description { get; set; }
        }

        public class CarPaymentExpenseModel
        {
            public DateTime Date { get; set; }
            public string ExpenseName { get; set; }
            public int Quantity { get; set; }
            public double UnitPrice { get; set; }
            public double AmountRiel { get; set; }
            public double AmountDollar { get; set; }
            public string Province { get; set; }
            public double ExchangeRate { get; set; }
            public DateTime ClosingDate { get; set; }
        }
        public class TransferMoney
        {
            public int Id { get; set; }
            public DateTime TransactionDate { get; set; }
            public string Description { get; set; }
            public int Amount { get; set; }
            public double DollarFromEmployee { get; set; }
            public double RielFromEmployee { get; set; }
            public double ExchangeRateEmployee { get; set; }
            public double TotalFromEmployee => (RielFromEmployee / ExchangeRateEmployee) + DollarFromEmployee;
            public DateTime CreatedDate { get; set; } = DateTime.Now;
            public DateTime ClosingDate { get; set; }
            public string CreatedBy { get; set; } 
            public string DbCode { get; set; } 
            public string Employee { get; set; }
            public int EmployeeId { get; set; }
            public int TemplateId { get; set; }
            public double? DepositDollar { get; set; }
            public double? DepositRiel { get; set; }
            public double? DepositExchange { get; set; }
            public double? TotalDeposit => (DepositRiel / DepositExchange) + DepositDollar;
            public double? BiasedAmount => TotalFromEmployee - TotalDeposit;
        }
        public class AmountInvoiceReportModel
        {
            public double Transfer { get; set; }
            public double Biased { get; set; }
            public double Total { get; set; }

        }
        public class TotalInvoiceReportModel
        {
            public string InvoiceType { get; set; }
            public double CreditAmount { get; set; }
            public double PaymentAmount { get; set; }
            public double Total { get; set; }
        }

        public class TotalCollectionModel
        {
            public double AmountExpense { get; set; }
            public double AmountTransfer { get; set; }
            public double AmountBiased { get; set; }
            public double Total { get; set; }
            public DateTime ClosingDate { get; set; }
        }
        
        public class TotalPaymentModel
        {
            public string Description { get; set; }
            public double CreditAmount { get; set; }
            public double PaymentAmount { get; set; }
            public double Total { get; set; }
            public DateTime ClosingDate { get; set; }
        }
        

    }
}
