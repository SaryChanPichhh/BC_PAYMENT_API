using BC.PAYMENT.CORE.DTO.CommondityExchange.DailyRefundItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.CORE.DTO.CommondityExchange.RepairItem
{
    public class RepairGoodsDto
    {
        public int ReceivedId { get; set; }
        public string CustomerCode { get; set; }
        public string ItemCode { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
    }

    public class RepairGoodsRespondDto : RefundItemDto
    {
        public int DetailId { get; set; }
        public string StatusRepair { get; set; }
        public DateTime Date { get; set; }
        public string Seller { get; set; }
        public string Description { get; set; }
        public string DbCode { get; set; }
        public string CreateBy { get; set; }

    }
}
