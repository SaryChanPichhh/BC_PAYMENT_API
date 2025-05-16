using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.CORE.Entities.Invoice
{
    public class DividedInvoiceSummary
    {
        public string? DeliveryName { get; set; }
        public string? AreaNameKhmer { get; set; }
        public int New { get; set; }
        public int Change { get; set; }
        public int Old { get; set; }
        public int Total { get; set; }
    }
}
