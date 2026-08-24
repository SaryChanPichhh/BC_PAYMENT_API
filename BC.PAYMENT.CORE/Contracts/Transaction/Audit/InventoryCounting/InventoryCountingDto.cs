namespace BC.PAYMENT.CORE.DTO.Transaction.Audit.InventoryCounting
{
    public class InventoryCountingDto
    {
        public string? WarehouseCode { get; set; }
        public string? Warehouse { get; set; }
        public string? StockController { get; set; }
        public string? Participation { get; set; }
        public string? Period { get; set; }
        public string? CountingDate { get; set; }
    }

    public class InventoryCountingUpdateDto : InventoryCountingDto
    {
        [Required(ErrorMessage = "StockId is required")]
        public string StockId { get; set; } = string.Empty;
    }
}
