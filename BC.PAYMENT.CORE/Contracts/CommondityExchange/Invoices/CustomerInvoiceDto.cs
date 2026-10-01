namespace BC.PAYMENT.CORE.DTO.CommondityExchange.Invoices;

public class CustomerInvoiceDto : Customer
{
    public string Phone { get; set; }
    public string Transaction { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => LastName + " " + FirstName;
    public string UserCode { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
}