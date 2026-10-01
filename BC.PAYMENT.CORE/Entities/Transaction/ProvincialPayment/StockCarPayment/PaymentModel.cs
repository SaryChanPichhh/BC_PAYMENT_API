namespace BC.PAYMENT.CORE.Entities.Transaction.ProvincialPayment.StockCarPayment;

public class PaymentModel
{
    public string? Description { get; set; }
    public double CreditAmount { get; set; }
    public double PaymentAmount { get; set; }
    public double Total => CreditAmount + PaymentAmount;
}

public class CollectionPaymentModel
{
    public double AmountExpense { get; set; }
    public double AmountTransfer { get; set; }
    public double AmountBiased { get; set; }
    public double Total => AmountExpense + AmountTransfer + AmountBiased;
}