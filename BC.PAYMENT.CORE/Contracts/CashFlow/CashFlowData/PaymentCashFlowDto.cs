namespace BC.PAYMENT.CORE.DTO.CashFlow.CashFlowData;

public class PaymentCashFlowDto
{
    public DateTime Date { get; set; }
    public string Name { get; set; }
    public double Amount { get; set; }
    public CurrencyFormat CurrencyFormat { get; set; }
    public double ExchangeRate { get; set; }
}

public class PaymentCashFlowUpdateDto : PaymentCashFlowDto
{
    public int Id { get; set; }
}

public class PaymentCashFlowPostDto : PaymentCashFlowDto
{
    public string HeaderId { get; set; } = string.Empty;
}

public class SubmittedCashFlowPostDto : PaymentCashFlowPostDto
{
    public int Id { get; set; }
    public string HeaderId { get; set; } = string.Empty;
}