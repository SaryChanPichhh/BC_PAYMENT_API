using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BC.PAYMENT.CORE.DTO.Invoice
{
    public record IssueInvoiceDTO
    {
        [JsonIgnore]
        public string? DbCode { get; set; }
        [Required]
        public int InvoiceId { get; set; }
        [Required]
        public required string DeliveryId { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public string? CreatedBy { get; set; }
    }
}
