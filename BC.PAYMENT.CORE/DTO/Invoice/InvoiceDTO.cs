using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BC.PAYMENT.CORE.DTO.Invoice
{
    public class InvoiceDTO
    {
        [JsonIgnore]
        public string? DbCode { get; set; }

        [Required]
        public required string TransactionCode { get; set; }

        [JsonIgnore]
        public string? EntryCode { get; set; }

        [JsonIgnore]
        public string? CreatedBy { get; set; }
    }
}
