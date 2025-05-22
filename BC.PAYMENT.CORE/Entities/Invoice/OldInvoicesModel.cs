using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.CORE.Entities.Invoice
{
    public class OldInvoicesModel : Invoices
    {
        public string? Username { get; set; }
        public string? AnalysisT0 { get; set; }
        public new string? Employee => string.IsNullOrEmpty(Username) ? AnalysisT0 : Username;
        public string? TransactionCode { get; set; }
        public string? DbCode { get; set; }
        public string? CreateBy { get; set; }
    }
}
