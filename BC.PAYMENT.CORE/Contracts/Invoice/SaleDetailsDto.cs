

namespace BC.PAYMENT.CORE.DTO.Invoice
{
    public class SaleDetailsDto
    {

        public string RefType => "D";
        public string DetailId => "D";
        public string TransType { get; set; }
        public string TransRef { get; set; }
        public string TransLine { get; set; }
        public string TransCd => "D";
        public string Location { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }

        /// <summary>
        ///  MM/dd/yyyy
        /// </summary>
        public string DueDate { get; set; }

        public string Status { get; set; } = "80"; // under 80

        /// <summary>
        ///   Quantity
        /// </summary>
        public double Value1 { get; set; }

        /// <summary>
        ///    Quantity
        /// </summary>J
        public double Value2 => Value1;

        /// <summary>
        ///     UnitPrice
        /// </summary>
        public double Value3 { get; set; }

        /// <summary>
        ///     Total
        /// </summary>
        public double Value4 => Value2 * Value3;

        public double Value5 { get; set; } = 0.00;
        public double Value6 { get; set; } = 0.00;
        public double Value7 => Value4;
        public double Value8 { get; set; } = 0.00;
        public double Value9 { get; set; } = 0.00;
        public double Value10 { get; set; } = 0.00;
        public double Value11 { get; set; } = 0.00;
        public double Value12 { get; set; } = 0.00;
        public double Value13 => Value4;
        public double Value14 { get; set; } = 0.00;
        public double Value15 { get; set; } = 0.00;
        public double Value16 { get; set; } = 0.00;
        public double Value17 { get; set; } = 0.00;
        public double Value18 { get; set; } = 0.00;
        public double Value19 { get; set; } = 0.00;
        public double Value20 { get; set; } = 0.00;
        public int UnitSale => 1;
        public string OrdPeriod { get; set; }

        /// <summary>
        ///     MM/dd/yyyy
        /// </summary>
        public string DelDate { get; set; }

        /// <summary>
        ///     MM/dd/yyyy
        /// </summary>
        public string InvoiceDate { get; set; }

        public string InvoiceNo { get; set; }
        public string InvoicePeriod { get; set; }
        public string AccountCode { get; set; } = "";
        public string AnalM0 { get; set; }
        public string AnalM1 { get; set; }
        public string AnalM2 { get; set; }
        public string AnalM3 { get; set; }
        public string AnalM4 { get; set; }
        public string AnalM5 { get; set; }
        public string AnalM6 { get; set; }
        public string AnalM7 { get; set; }
        public string AnalM8 { get; set; }
        public string AnalM9 { get; set; }
        public string AssemblyInd => "";
        public string SplitVal => "1";
        public string CreditStatus { get; set; } = "0"; // change to '' 
        public string PriceBook => "";
        public int SaleQtyValue => 1;
        public int StkQtyValue => 2;
        public int TopValue => 13;
        public int DisplayValue1 => 13;
        public int DisplayValue2 => 0;
        public int FixedValue => 3;
        public string LineRef { get; set; } = ""; // Expired Date
        public int Physical { get; set; }
        public int OnHold { get; set; }
        public int Fees { get; set; }
        public string ItemCodeCopy { get; set; }
        public string UserCode { get; set; }
        public string UserInvoice { get; set; }
        public string AllowRef => "";

        /// <summary>
        ///     If Repair Item UpdateStock = M else S
        /// </summary>
        public string UpdateStock { get; set; }

        public string FixedStockValue2 => "5";
        public string FixedStockValue3 => "9";
    }


    public class SaleDetailDtoRespone
    {
        public string RefType { get; set; }
        public string DetailId { get; set; }
        public string TransType { get; set; }
        public string TransRef { get; set; }
        public string TransLine { get; set; }
        public string TransCd { get; set; }
        public string Location { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }

        /// <summary>
        ///  MM/dd/yyyy
        /// </summary>
        public string DueDate { get; set; }

        public string Status { get; set; } = "80"; // under 80

        /// <summary>
        ///   Quantity
        /// </summary>
        public double Value1 { get; set; }

        /// <summary>
        ///    Quantity
        /// </summary>J
        public double Value2 => Value1;

        /// <summary>
        ///     UnitPrice
        /// </summary>
        public double Value3 { get; set; }

        /// <summary>
        ///     Total
        /// </summary>
        public double Value4 => Value2 * Value3;

        public double Value5 { get; set; } = 0.00;
        public double Value6 { get; set; } = 0.00;
        public double Value7 => Value4;
        public double Value8 { get; set; } = 0.00;
        public double Value9 { get; set; } = 0.00;
        public double Value10 { get; set; } = 0.00;
        public double Value11 { get; set; } = 0.00;
        public double Value12 { get; set; } = 0.00;
        public double Value13 => Value4;
        public double Value14 { get; set; } = 0.00;
        public double Value15 { get; set; } = 0.00;
        public double Value16 { get; set; } = 0.00;
        public double Value17 { get; set; } = 0.00;
        public double Value18 { get; set; } = 0.00;
        public double Value19 { get; set; } = 0.00;
        public double Value20 { get; set; } = 0.00;
        public int UnitSale { get; set; }
        public string OrdPeriod { get; set; }

        /// <summary>
        ///     MM/dd/yyyy
        /// </summary>
        public string DelDate { get; set; }

        /// <summary>
        ///     MM/dd/yyyy
        /// </summary>
        public string InvoiceDate { get; set; }

        public string InvoiceNo { get; set; }
        public string InvoicePeriod { get; set; }
        public string AccountCode { get; set; } = "";
        public string AnalM0 { get; set; }
        public string AnalM1 { get; set; }
        public string AnalM2 { get; set; }
        public string AnalM3 { get; set; }
        public string AnalM4 { get; set; }
        public string AnalM5 { get; set; }
        public string AnalM6 { get; set; }
        public string AnalM7 { get; set; }
        public string AnalM8 { get; set; }
        public string AnalM9 { get; set; }
        public string AssemblyInd { get; set; }
        public string SplitVal { get; set; }
        public string CreditStatus { get; set; } = "0"; // change to '' 
        public string PriceBook { get; set; }
        public int SaleQtyValue { get; set; }
        public int StkQtyValue { get; set; }
        public int TopValue { get; set; }
        public int DisplayValue1 { get; set; }
        public int DisplayValue2 { get; set; }
        public int FixedValue { get; set; }
        public string LineRef { get; set; } = ""; // Expired Date
        public int Physical { get; set; }
        public int OnHold { get; set; }
        public int Fees { get; set; }
        public string ItemCodeCopy { get; set; }
        public string UserCode { get; set; }
        public string UserInvoice { get; set; }
        public string AllowRef { get; set; }

        /// <summary>
        ///     If Repair Item UpdateStock = M else S
        /// </summary>
        public string UpdateStock { get; set; }

        public string FixedStockValue2 { get; set; }
        public string FixedStockValue3 { get; set; }
    }

}
