using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BC.PAYMENT.CORE.DTO.Payment
{
    public class PaymentInvoiceHeaderDTO
    {
        [JsonIgnore]
        public int Id { get; set; }

        [Required]
        public string DbCode { get; set; }

        [Required]
        public string DeliveryId { get; set; }

        [Required]
        public int Period { get; set; }

        [Required]
        public DateTime InvoiceDividendDate { get; set; }

        [Required]
        public string EntriesCode { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public string CreatedDate { get; set; }

        [JsonIgnore]
        public string Status => "1";
    }
}
