
using System.ComponentModel.DataAnnotations;

namespace BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.StockCarPayment
{
    public class StockCarExpenseDto
    {
        public string? Employee { get; set; }
        public int EmployeeId { get; set; }
        public string? ExpenseType { get; set; }
        public int ExpenseTypeId { get; set; }
        public double Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double AmountRiel { get; set; }
        public double AmountDollar { get; set; }
        public string? ProvinceName { get; set; }
        public int ProvinceId { get; set; }
        public double ExchangeRate { get; set; }
        public DateTime ExpenseDate { get; set; }
        public int TemplateId { get; set; }
    }

    public class StockCarExpenseUpdateDto : StockCarExpenseDto
    {
        [Required]
        public int Id { get; set; }
    }
}
