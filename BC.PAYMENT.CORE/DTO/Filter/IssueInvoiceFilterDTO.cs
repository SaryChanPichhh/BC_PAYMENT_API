using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.CORE.DTO.Filter
{
    public record IssueInvoiceFilterDTO:BaseFilterDTO
    {
        [Required]
        public string AreaId { get; set; }
    }
}
