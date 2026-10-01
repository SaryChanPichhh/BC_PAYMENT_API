using System.ComponentModel;

namespace BC.PAYMENT.REPORTS.Models;

public class FlatInvoiceRow
{
    [Description("ល.រ")] public string RowNumber { get; set; } // For numbering visible rows (ល.រ)
    [Description("លេខកូដទំនិញ")] public string ItemCode { get; set; } // Item Code (once per group)
    [Description("ឈ្មោះទំនិញ")] public string ItemDesc { get; set; } // Item Name/Description
    [Description("ចំនួន")] public decimal Qty { get; set; } // From UnitConvert
    [Description("ឯកតាស្តុក")] public string UnitStock { get; set; } // From UnitConvert
    [Description("តម្លៃ")] public decimal Price { get; set; } // From UnitConvert
    [Description("បញ្ចុះតម្លៃ​​")] public decimal Discount { get; set; } // From UnitConvert
    [Description("សរុប")] public decimal Total { get; set; }
}