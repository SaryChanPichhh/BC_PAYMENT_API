
using BC.PAYMENT.CORE.Entities.General;

namespace BC.PAYMENT.CORE.DTO.CommondityExchange.RepairItem
{
    public class ItemRepairInprogressDto : Customer
    {
        public int Id { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionType { get; set; }
        public int ReceivedId { get; set; }
        public DateTime RequestDate { get; set; }
        public string Barcode { get; set; }
        public string ItemCode { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }

        public string StatusText =>
            Status.Trim() switch
            {
                "Pending" => "កំពុងរង់ចាំ",
                "Yes" => "កំពុងជួសជុល",
                _ => "បានបញ្ចប់ការជួសជុល"
            };
        public DateTime Date { get; set; }
        public string Seller { get; set; }
    }
}
