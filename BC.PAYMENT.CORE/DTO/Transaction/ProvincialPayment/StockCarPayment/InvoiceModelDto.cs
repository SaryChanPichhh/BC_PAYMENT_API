

namespace BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.StockCarPayment
{
    public class InvoiceModelDto
    {
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public double InvoiceValue { get; set; }
        public int InvoiceId { get; set; }
    }

    public class InvoiceModelPostDto
    {
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public double InvoiceValue { get; set; }
        public string? TransRef { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? DbCode { get; set; }
        public int Period { get; set; }
        public string? CreateBy { get; set; }
        public int TemplateId { get; set; }
    }
    public class NewInvoiceGetDto
    {
        public string? FromSaleCode { get; set; }
        public string? ToSaleCode { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }
    public class OldInvoiceResponeDto
    {
        public string? CustomerCode { get; set; }
        public string? CustomerName { get; set; }
        public double InvoiceValue { get; set; }
        public string? Code { get; set; }
        public string? Employee { get; set; }
        public DateTime TransactionDate { get; set; }

        public OldInvoiceResponeDto()
        {
            
        }
        public OldInvoiceResponeDto(DateTime transactionDate, string? customerCode, string? customerName, double invoiceValue, string? employee,string?code)
        {
            TransactionDate = transactionDate;
            CustomerCode = customerCode;
            CustomerName = customerName;
            InvoiceValue = invoiceValue;
            Employee = employee;
            Code = code;
        }

    }
    
    
  
}
