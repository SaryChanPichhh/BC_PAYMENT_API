namespace BC.PAYMENT.CORE.DTO.CommondityExchange.Invoices
{
    public class ItemDto :Customer
    {
        public int Id { get; set; }
        public int MasterId { get; set; }
        public int RequestExchangeDetailId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ChangeType { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double Total => Quantity * UnitPrice;
        public byte[] Image { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string Seller { get; set; }
        public string TransactionCode { get; set; }
        public string OldItemCode { get; set; }
        public double ItemPrice { get; set; }
        public string Warehouse { get; set; }
        public string ExpiredDate { get; set; }
        public DateTime RequestDate { get; set; }
        public string InvoiceNumber { get; set; }
    }

    public class ItemExchangeInvoiceDto 
    {
        public string ItemCode { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }

    public class ItemRepairInvoiceDto : ItemExchangeInvoiceDto
    {
        public string Description { get; set; }

    }
}
