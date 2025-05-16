using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.PAYMENT.CORE.Entities.General
{
    public class Market
    {
        public string? MarketID { get; set; }
        public string? MarketName { get; set; }
        public string? MarketNameKhmer { get; set; }
        public string ImagePath { get; set; }

        public class MarketImage
        {
            public byte[] Image { get; set; }
        }
    }
}
