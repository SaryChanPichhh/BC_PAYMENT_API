using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BC.PAYMENT.CORE.DTO.Invoice
{
    public record SaveInvoiceDTO
    {
        [JsonIgnore]
        public string? DbCode { get; set; }

        [Required]
        public string TransactionCode { get; set; }

        [Required]
        public string CustomerCode { get; set; }

        [Required]
        public string CustomerName { get; set; }

        [Required]
        public double InvoiceValue { get; set; }

        [JsonIgnore]
        public string? CreatedBy { get; set; }
         
        [JsonIgnore]
        public string? EntryCode { get; set; }
    }
}
