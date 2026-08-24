namespace BC.PAYMENT.CORE.DTO.Invoice
{
    public class SaleHeaderDto
    {

        /// <summary>
        /// O => Sale Order
        /// I => Print
        /// </summary>
        public string RecType { get; set; }

        public string Transaction { get; set; }
        public string HeaderId => "H";
        public string CustomerCode { get; set; }
        public string DeliveryAdd => CustomerCode;

        /// <summary>
        ///     Format type yyyy-MM-dd HH:mm:ss
        /// </summary>
        public string TransactionDate { get; set; }

        public string Status { get; set; } = "10";
        public string TransactionCd => "D";
        public string TransactionCode { get; set; }
        public string OrderNo { get; set; }

        /// <summary>
        /// Format type MM-dd-yyyy
        /// </summary>
        public string OrderDate { get; set; }

        public string PrnDate => @"";
        public string DelDate => @"";

        /// <summary>
        ///     Format type M/d/yyyy
        /// </summary>
        public string InvoiceDate { get; set; }

        public string InvoicePeriod { get; set; }
        public string CustomerRef => @"";
        public string DeliveryRef => @"";
        public string Comments { get; set; }
        public double TransactionValue { get; set; }
        public string PayDate => @"";

        /// <summary>
        ///     User Code And AnalysisC0
        /// </summary>
        public string AnalM0 { get; set; }

        /// <summary>
        ///     Empty
        /// </summary>
        public string AnalM1 { get; set; }

        /// <summary>
        ///     Empty
        /// </summary>
        public string AnalM2 { get; set; }

        /// <summary>
        ///     AnalysisC6
        /// </summary>
        public string AnalM3 { get; set; }

        /// <summary>
        ///     Empty
        /// </summary>
        public string AnalM4 { get; set; }

        /// <summary>
        ///     Empty
        /// </summary>
        public string AnalM5 { get; set; }

        public string AnalM6 { get; set; }
        public string AnalM7 { get; set; }
        public string AnalM8 { get; set; }
        public string AnalM9 { get; set; }
        public string QuoteConvert => "";
        public string QuotePrint => "";
        public string QuoteExpiry => "";
        public string QuotePeriod => "0";
        public string QuotationRef => "";
        public string DateQuoted => "";
        public string VoidStatus => "N";
        public string UserCode { get; set; }
    }
}
