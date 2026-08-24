namespace BC.PAYMENT.CORE.DTO.Transaction.DailyPayment.AccountReceivable
{
    public class ConfirmAccountReceivableDto
    {
        public string? ConfirmBalanceOwner { get; set; }
        public string? Participants { get; set; }
        public string? Description { get; set; }
    }
    public class ConfirmAccountReceivableUpdateDto : ConfirmAccountReceivableDto
    {
        public int Id { get; set; }
    }

    public class ConfirmAccountReceivableDetailDto
    {
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? TransactionCode { get; set; }
        public string? InvoiceValue { get; set; }
        public int AccountReceivableId { get; set; }
    }

    public class ConfirmAccountReceivableDetailPostDto
    {

    }
    public class ConfirmAccountReceivableDetailPutDto
    {
        public double Balance { get; set; }
        public string? Description { get; set; }
        public string? IsCustomerAgreed { get; set; }
        public string? IsMet { get; set; }
        public string? InvoicedCode { get; set; }
        public int ConfirmBalanceId { get; set; }

    }
}
