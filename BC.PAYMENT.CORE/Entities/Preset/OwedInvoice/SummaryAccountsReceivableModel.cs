
using BC.PAYMENT.CORE.Entities.General;
namespace BC.PAYMENT.CORE.Entities.Preset.OwedInvoice
{
    public class SummaryAccountsReceivableModel 
    {
        public string Code { get; set; }
        public string TransactionCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Area { get; set; }
        public string Market { get; set; }
        public string Store { get; set; }
        public decimal InvoiceValue { get; set; }
        public string AnalysisT0 { get; set; }
        public string Username { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Period => TransactionDate.TimeAgo();
        public override string ToString()
        {
            return Period;
        }
    }
}
