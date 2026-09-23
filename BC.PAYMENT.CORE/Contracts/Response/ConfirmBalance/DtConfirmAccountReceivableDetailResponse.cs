namespace BC.PAYMENT.CORE.Contracts.Response.ConfirmBalance;

public class DtConfirmAccountReceivableDetailResponse 
{
    public int ConfirmBalanceDetailsId { get; set; }
    public string CustomerCode { get; set; }= string.Empty;
    public string CustomerName { get; set; }= string.Empty;
    public string Market { get; set; }= string.Empty;
    public string Store { get; set; }= string.Empty;
    public string Area { get; set; }= string.Empty;
    public string InvoicedCode { get; set; }= string.Empty;
    public double InvoicedAmount { get; set; }
    public double Balance { get; set; }
    public string Description { get; set; }= string.Empty;
    public string IsMet { get; set; }= string.Empty;
    public string IsMetStatus => IsMet == "N" ? "បានជួប" : "មិនបានជួប";
    public string IsCustomerAgreed { get; set; }= string.Empty;
    public string CustomerAgreed => IsCustomerAgreed == "N" ? "មិនទទួលស្កាល់" : "ទទួលស្កាល់";
    public string Status { get; set; }= string.Empty;
    public DateTime UpdatedDate { get; set; }
    public string UpdatedBy { get; set; }= string.Empty;
}