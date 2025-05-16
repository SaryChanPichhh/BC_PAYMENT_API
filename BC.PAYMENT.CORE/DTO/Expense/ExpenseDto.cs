using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.CORE.DTO.Expense
{
    public class ExpenseDto
    {
        public string? DividedInvoiceId { get; set; }
        public string? Description { get; set; }
        public double UnitPrice { get; set; }
        public double ExchangeRate { get; set; }
        public string? PaymentInvoiceHeaderId { get; set; }
    }

    public class ExpensePostDto
    {
        public string? DividedInvoiceId { get; set; }
        public string? Description1 { get; set; }
        public double UnitPrice1 { get; set; }
        public string? Description2 { get; set; }
        public double UnitPrice2 { get; set; }
        public string? Description3 { get; set; }
        public double UnitPrice3 { get; set; }
        public double ExchangeRate { get; set; }
        public double Total { get; set; }
        public string? PaymentInvoiceHeaderId { get; set; }
    }
}
