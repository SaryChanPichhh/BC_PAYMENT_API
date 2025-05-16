
namespace BC.PAYMENT.CORE.DTO.Prepare.Account
{
    public class AccountReceivableDto
    {
        public string? AccountCode { get; set; }
        public string? Description { get; set; }
        public string? CreditDebitType { get; set; }
        public string? Field1 { get; set; }
        public string? Field2 { get; set; }
        public string? Field3 { get; set; }
        public string? Field4 { get; set; }
        public string? Field5 { get; set; }
        public string? Field6 { get; set; }
        public string? Field7 { get; set; }
        public string? Field8 { get; set; }
        public string? Field9 { get; set; }
    }

    public class AccountReceivableDeleteDto
    {
        public string? DbCode { get; set; }
        public string? AccountCode { get; set; }
        public string? CreditDebitType { get; set; }

    }
}
