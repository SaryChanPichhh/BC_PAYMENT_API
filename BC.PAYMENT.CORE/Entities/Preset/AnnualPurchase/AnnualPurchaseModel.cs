namespace BC.PAYMENT.CORE.Entities.Preset.AnnualPurchase
{
    public class AnnualPurchaseModel : Customer
    {
        public double Amount { get; set; }
        public int Year { get; set; }
    }
}
