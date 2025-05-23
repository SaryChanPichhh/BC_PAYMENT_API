
using BC.PAYMENT.CORE.DTO.Filter;

namespace BC.PAYMENT.CORE.DTO.Transaction.Inventory.VerificationStock
{
    public class VerificationStockDto
    {
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Location { get; set; }
        public int? UnitStock { get; set; }
        public int? Physical { get; set; }
        public int? OnOrder { get; set; }
        public int? SubTotal { get; set; }
        public int Quantity { get; set; }
        public int Total { get; set; }
    }

    public class VerificationRFIDDto
    {
        public VerificationStockPostDto SubmittedCodeAndLocation { get; set; } 
        public List<VerificationStockDto> StockForVerification { get; set; } 
    }

    public class VerificationStockReportDto : VerificationStockDto
    {
        public int Id { get; set; }
        public string CreateBy { get; set; } 
        public DateTime CreateDate { get; set; } 
        public int Period { get; set; } 

    }
    public class VerificationStockPostDto
    {
        public string SubmitCode { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string DbCode { get; set; } = string.Empty;
    }
    public class AdjustmentInventoryDto 
    {
        public string Location { get; set; }
        public string ItemCode { get; set; }
        public int Total { get; set; }
        public string Status { get; set; }
        public int AdjustQty => Status switch
        {
            "ត្រឹមត្រូវ" => 0,
            "ខ្វះ" => Total * 1,
            "លើស" => Total * -1,
            _ => 0
        };
        public string StatusType => Status switch
        {
            "ត្រឹមត្រូវ" => "",
            "ខ្វះ" => "ADJ-",
            "លើស" => "ADJ+",
            _ => ""
        };

    }
    public class PaginatedVerificationStockByPeriodDto : ByPeriodDto
    {
        public string Location { get; set; } = string.Empty;
    }  
    public class PaginatedVerificationStockByDateDto : ByDateDto
    {
        public string Location { get; set; } = string.Empty;
    }
}
