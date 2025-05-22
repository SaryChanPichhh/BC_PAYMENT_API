using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.CORE.DTO.Transaction.ProvincialPayment.ReviewReport
{
    public class StockCarRequestDto
    {
        public int RequestId { get; set; }
        public string? Employee { get; set; }
        public string? Title { get; set; }
        public int Action { get; set; }
        public DateTime Date { get; set; }
    }
}
