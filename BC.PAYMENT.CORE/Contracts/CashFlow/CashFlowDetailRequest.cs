using System.ComponentModel.DataAnnotations;

namespace BC.PAYMENT.CORE.Contracts.CashFlow;

public class CashFlowDetailRequest
{
    /// <summary>0 = use (or create) the header of <see cref="Date"/>.</summary>
    [Range(0, int.MaxValue)] public int HeaderId { get; set; }
    public DateTime Date { get; set; }
    [Required] public string Name { get; set; } = string.Empty;
    [Range(0.0001, double.MaxValue)] public double Amount { get; set; }
    public CurrencyFormat CurrencyFormat { get; set; }
    [Range(0.0001, double.MaxValue)] public double ExchangeRate { get; set; }
}
