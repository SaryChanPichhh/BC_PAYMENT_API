namespace BC.PAYMENT.CORE.Entities;

public class DtConfirmAccountReceivableDetail
{
    public int Id { get; set; }
    public int HeaderId { get; set; }
    public string DbCode { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string InvoiceCode { get; set; } = string.Empty;
    public double InvoiceAmount { get; set; }
    public double Balance { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CustomerStatus { get; set; } = string.Empty;
    public string IsAgree { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime UpdatedDate { get; set; }
}