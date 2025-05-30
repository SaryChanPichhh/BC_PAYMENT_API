
namespace BC.PAYMENT.CORE.DTO.Invoice
{
    public class CreateInvoiceDto
    {
        public DateTime InvoiceDate { get; set; }
        public DateTime InvoiceDue { get; set; }
        public string InvoiceReference { get; set; }
        public string CustomerCode { get; set; }
        public decimal Discount { get; set; }
        public decimal Taxes { get; set; }
        public string Description { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate => DateTime.Now;
        public string CreatedBy { get; set; }
        public string DbCode { get; set; }
        public int RequestRepairId { get; set; }
    }
}
