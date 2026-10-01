namespace BC.PAYMENT.CORE.Contracts.Response.StockCar;

public class StockCarExpenseResponse
{
    public int Id { get; set; }
    public string Employee { get; set; } = string.Empty;
    public string ExpenseType { get; set; } = string.Empty;
    public int ExpenseTypeId { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double AmountRiel { get; set; }
    public double AmountDollar { get; set; }
    public string ProvinceName { get; set; } = string.Empty;
    public int ProvinceId { get; set; }
    public double ExchangeRate { get; set; }
    public DateTime ExpenseDate { get; set; }

    public double RielFromEmployee
    {
        get => AmountRiel;
        set => AmountRiel = value;
    }

    public double DollarFromEmployee
    {
        get => AmountDollar;
        set => AmountDollar = value;
    }

    public double ExchangeRateEmployee
    {
        get => ExchangeRate;
        set => ExchangeRate = value;
    }
}