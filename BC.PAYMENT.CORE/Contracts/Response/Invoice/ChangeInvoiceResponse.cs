using System;
using BC.PAYMENT.CORE.Entities.General;
using BC.PAYMENT.CORE.Enums;

namespace BC.PAYMENT.CORE.Contracts.Response.Invoice
{
    public class ChangeInvoiceResponse : Entities.General.Customer
    {
        public int InvoiceId { get; set; }
        public string? InvoiceCode { get; set; }
        public double InvoiceAmount { get; set; }
        public InvoiceStatus InvoiceType { get; set; }
        public string? DbCode { get; set; }
        public string? CreatedBy { get; set; }
        public string? EntriesCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDivided { get; set; }
    }
}
