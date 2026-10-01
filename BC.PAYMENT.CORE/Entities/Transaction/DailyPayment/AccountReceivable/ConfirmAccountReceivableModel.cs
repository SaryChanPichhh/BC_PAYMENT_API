namespace BC.PAYMENT.CORE.Entities.Transaction.DailyPayment.AccountReceivable;

public class ConfirmAccountReceivableModel
{
    public class ConfirmBalance
    {
        public int Id { get; set; }
        public string? DbCode { get; set; }
        public string? ConfirmBalanceOwner { get; set; }
        public string? Participants { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class ConfirmBalanceDetails
    {
        public int ConfirmBalanceDetailsId { get; set; }
        public int ConfirmBalanceId { get; set; }
        public string? DbCode { get; set; }
        public DateTime InvoicedDate { get; set; }
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public string? InvoicedCode { get; set; }
        public double InvoicedAmount { get; set; }
        public string? Market { get; set; }
        public string? Store { get; set; }
        public string? Area { get; set; }
        public double Balance { get; set; }
        public string? IsMet { get; set; }
        public string IsMetStatus => IsMet == "N" ? "បានជួប" : "មិនបានជួប";
        public string? Description { get; set; }
        public string? IsCustomerAgreed { get; set; }
        public string CustomerAgreed => IsCustomerAgreed == "N" ? "មិនទទួលស្កាល់" : "ទទួលស្កាល់";
        public string? Status { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}