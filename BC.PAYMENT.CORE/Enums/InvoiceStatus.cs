namespace BC.PAYMENT.CORE.Enums;

public enum InvoiceStatus
{
    [Description("វិក្ក័យប័ត្រថ្មី")] NewInvoice,
    [Description("វិក្ក័យប័ត្រដូរ")] ChangeInvoice,
    [Description("វិក្ក័យប័ត្រចាស់")] OldInvoice
}