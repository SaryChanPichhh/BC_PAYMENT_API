using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Enums;

namespace BC.PAYMENT.CORE.Entities.Invoice
{
    public class NewInvoiceModel : Customer
    {
        public string? InvoiceCode { get; set; }
        public double InvoiceAmount { get; set; }
        public InvoiceTypes InvoiceTypes { get; set; }
        public string? DbCode { get; set; }
        public string? CreatedBy { get; set; }
        public string? EntriesCode { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
