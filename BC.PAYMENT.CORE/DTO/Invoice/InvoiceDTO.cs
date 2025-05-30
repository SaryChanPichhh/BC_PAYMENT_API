using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BC.PAYMENT.CORE.Entities.General;

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

    public class InvoiceDetailDto : Customer
    {
        public string InvoiceNumber { get; set; }
        public string TransactionCode { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime InvoiceDue { get; set; }
        public string Phone { get; set; }
        public string SalesRepresentative { get; set; }
        public string User { get; set; } 
    }
}
