using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BC.PAYMENT.CORE.DTO.Accounting
{
    public class AnalysisCodeDTO
    {
        [Required]
        public string AnamCode { get; set; }

        [JsonIgnore]
        public string? DbCode { get; set; }
    }
}
