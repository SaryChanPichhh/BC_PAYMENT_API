namespace BC.PAYMENT.CORE.DTO.ClosingInventoryAndInvoice.ClosingInventory
{
    public class ClosingInventoryDto
    {
        public class ClosingInventoryResponseDto
        {
            public string DbCode { get; set; }
            public DateTime TransactionDate { get; set; }
            public string ItemCode { get; set; }
            public string Location { get; set; }
            public int? OpeningBalance { get; set; }
            public int? PurchaseOrder { get; set; }
            public int? Order { get; set; }
            public int? Sale { get; set; }
            public int? Transfer { get; set; }
            public int? CreditNote { get; set; }
            public int? InventoryAdjustment { get; set; }
            public int? Print { get; set; }
            public int? ClosingBalance => (OpeningBalance ?? 0) + (PurchaseOrder ?? 0) + (CreditNote ?? 0) -
                (Math.Abs(Sale ?? 0) + Math.Abs(Transfer ?? 0)) + (InventoryAdjustment ?? 0);
        }
        public class ClosingInventoryPostDto : ClosingInventoryResponseDto
        {
            public List<ClosingInventoryResponseDto> ClosingInventoryDtos { get; set; }
            public bool Status { get; set; }
        }

        public class MonthlyClosingPostDto : ClosingInventoryResponseDto
        {
            
        }
        public class ClosingInventoryPaginatedDto : InventoryExpiredDto
        {
            public string ClosingDate { get; set; }
        }
       
    }
}
