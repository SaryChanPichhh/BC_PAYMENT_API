
namespace BC.PAYMENT.CORE.Entities.Prepare.Account
{
    public class AccountReceivablePresetModel
    {
        public string? DbCode { get; set; }
        public string? AccountCode { get; set; }
        public string? Description { get; set; }
        public string? BcDatatype => "ACCOUNT RECEIVABLE";
        public string? CreditDebitType { get; set; }
        public string? JournalType => Field1;
        public string? Field1 { get; set; }
        public string? Field2 { get; set; }
        public string? Field3 { get; set; }
        public string? Field4 { get; set; }
        public string? Field5 { get; set; }
        public string? Field6 { get; set; }
        public string? Field7 { get; set; }
        public string? Field8 { get; set; }
        public string? Field9 { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedDate { get; set; }
    }
}
