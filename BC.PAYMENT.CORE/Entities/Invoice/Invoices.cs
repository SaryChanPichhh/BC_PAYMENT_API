using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.CORE.Entities.Invoice
{
    public class Invoices
    {
        public int InvoiceId { get; set; }
        public string TransRef { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Employee { get; set; }
        public string Store { get; set; }
        public double InvoiceValue { get; set; }
        public string Market { get; set; }
        public bool Status { get; set; }
    }
}
