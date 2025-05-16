using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BC.PAYMENT.CORE.DTO.Filter
{
    public record BaseFilterDTO
    {
        [JsonIgnore]
        public string? DbCode { get; set; }

        [Required]
        public int Page{ get; set; } = 1;

        [Required] public int PageSize { get; set; } = 10;
    }
}
