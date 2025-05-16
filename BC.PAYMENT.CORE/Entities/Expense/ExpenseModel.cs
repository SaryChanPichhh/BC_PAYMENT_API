

namespace BC.PAYMENT.CORE.Entities.Expense
{
    public class ExpenseModel
    {
        public int Id { get; set; }
        public string? PaymentId { get; set; }
        public string? DbCode { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int PaymentInvoiceHeaderId { get; set; }
        public double UnitPrice { get; set; }
        public double UnitPriceCalculated => CurrencyType == "Riel" ? UnitPrice / ExchangeRate : UnitPrice;
        public string? CurrencyType { get; set; }
        public double ExchangeRate { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; }
        public double TotalCalculated => UnitPriceCalculated * Quantity;
        public string? CreatedBy { get; set; }
        public string? DeliveryName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
