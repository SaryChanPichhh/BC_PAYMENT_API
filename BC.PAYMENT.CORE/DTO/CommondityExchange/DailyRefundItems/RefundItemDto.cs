using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC.PAYMENT.CORE.Entities.General;

namespace BC.PAYMENT.CORE.DTO.CommondityExchange.DailyRefundItems
{
    public class RefundItemDto : Customer
    {
        public int Id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }
    public class RefundItemDetailDto : RefundItemDto
    {
        public int MasterId { get; set; }
        public string ChangeType { get; set; }
        public string? Image { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }    
        public DateTime RequestDate { get; set; }
        public string InvoiceNumber { get; set; }
    }
}
