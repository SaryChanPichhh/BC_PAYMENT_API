namespace BC.PAYMENT.CORE.Entities.Preset.DailySaleAnalysis
{
    public class DailySaleAnalysisModel
    {
        public string No { get; set; }
        public string DB_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string DESCRIPTN { get; set; }
        public decimal B16_AMOUNT { get; set; }
        public decimal BT7_AMOUNT { get; set; }
        public decimal KC7_AMOUNT { get; set; }
        public decimal SR7_AMOUNT { get; set; }
        public decimal SP7_AMOUNT { get; set; }
        public decimal KP7_AMOUNT { get; set; }
        public decimal SV7_AMOUNT { get; set; }
        public decimal TOTAL { get; set; }
        public int B16_QTY { get; set; }
        public int BT7_QTY { get; set; }
        public int KC7_QTY { get; set; }
        public int SR7_QTY { get; set; }
        public int SP7_QTY { get; set; }
        public int KP7_QTY { get; set; }
        public int SV7_QTY { get; set; }
        public int TOTAL_QTY { get; set; }
    }
}
