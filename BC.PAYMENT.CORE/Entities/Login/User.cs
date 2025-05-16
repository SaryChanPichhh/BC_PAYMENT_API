using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BC.PAYMENT.CORE.DTO.General;

namespace BC.PAYMENT.CORE.Entities.Login
{
    public class User
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? DbCode { get; set; }
        public List<BranchDTO> Branches { get; set; }

        [JsonIgnore]
        public string? UserPass { get; set; }
        //public bool? UserStatus { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Name { get; set; }
        public string CompanyCode { get; set; }
        public string AppCode { get; set; }
        public DateTime CurrentDate { get; set; }
        public int RowNumber { get; set; }
        public string InvoiceEntryCode { get; set; }

    }
}
