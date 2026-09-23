using System.ComponentModel.DataAnnotations;

namespace BC.PAYMENT.CORE.Entities.StockCar;

public class StockCarExpense
{
    public int Id { get; set; }
    public string Employee { get; set; } = string.Empty;
    public int EmployeeId { get; set; }
    public string ExpenseType { get; set; } = string.Empty;
    public int ExpenseTypeId { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double AmountRiel { get; set; }
    public double AmountDollar { get; set; }
    public string ProvinceName { get; set; } = string.Empty;
    public int ProvinceId { get; set; }
    public double ExchangeRate { get; set; }
    [Required]
    public DateTime ExpenseDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string CreatedBy { get; set; } = string.Empty;
    public string DbCode { get; set; } = string.Empty;
    public int TemplateId { get; set; }
}
