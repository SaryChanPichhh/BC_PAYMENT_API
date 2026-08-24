namespace BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.DividingInvoices.Invoice
{
    public class NewInvoiceDto
    {
    }

    public class NewInvoiceFilterDto
    {
        [Required(ErrorMessage = "Invoice is required.")]
        public string? StartInvoiceType { get; set; }
        [Required(ErrorMessage = "Invoice is required.")]
        public string? EndInvoiceType { get; set; }
        [Required(ErrorMessage = "From Date is required.")]
        public string? FromDate { get; set; }
        [Required(ErrorMessage = "To Date is required.")]
        public string? ToDate { get; set; }
        public string? EntriesCode { get; set; }

    }
}
