using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC.PAYMENT.CORE.Entities.General;

namespace BC.PAYMENT.CORE.Entities.Invoice
{
    public class Invoices : Customer
    {
        public int InvoiceId { get; set; }
        public string? TransRef { get; set; }
        public string? Employee { get; set; }
        public double InvoiceValue { get; set; }
        public bool Status { get; set; }
    }
}
