namespace BC.ACCOUNTING.CORE.Entities
{
    public class Aging
    {
        public string AccountPeriod { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string ACC_KHMER { get; set; }
        public string AccountType { get; set; }
        public DateTime TransactionDate { get; set; }
        public string JournalNumber { get; set; }
        public int JournalLine { get; set; }
        public DateTime ByDate { get; set; }

        public decimal Amount_Current { get; set; }
        public decimal Amount_0_30 { get; set; }
        public decimal Amount_1_30 { get; set; }
        public decimal Amount_31_60 { get; set; }
        public decimal Amount_61_90 { get; set; }
        public decimal Amount_91_120 { get; set; }
        public decimal Amount_Over_30 { get; set; }
        public decimal Amount_Over_60 { get; set; }
        public decimal Amount_Over_90 { get; set; }
        public decimal Amount_Over_120 { get; set; }
        public decimal Amount_Total { get; set; }

        public string JournalType { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public DateTime EntryDate { get; set; }
        public string EntryPeriod { get; set; }
        public string DueDate { get; set; }
        public decimal DueDateAmount { get; set; }

        public string AssetCode { get; set; }
        public string AssetUpdate { get; set; }

        public string ConversionCode { get; set; }
        public string ConversionSign { get; set; }
        public decimal? ConversionRate { get; set; }

        public decimal OtherAmount_Current { get; set; }
        public decimal OtherAmount_0_30 { get; set; }
        public decimal OtherAmount_1_30 { get; set; }
        public decimal OtherAmount_31_60 { get; set; }
        public decimal OtherAmount_61_90 { get; set; }
        public decimal OtherAmount_91_120 { get; set; }
        public decimal OtherAmount_Over_30 { get; set; }
        public decimal OtherAmount_Over_60 { get; set; }
        public decimal OtherAmount_Over_90 { get; set; }
        public decimal OtherAmount_Over_120 { get; set; }
        public decimal OtherAmount_Total { get; set; }

        public string AnalysisT0 { get; set; }
        public string AnalysisT0Description { get; set; }
        public string AnalysisT0Lookup { get; set; }
        public string AnalysisT0Comments { get; set; }

        public string AnalysisT1 { get; set; }
        public string AnalysisT1Description { get; set; }
        public string AnalysisT1Lookup { get; set; }
        public string AnalysisT1Comments { get; set; }

        public string AnalysisT2 { get; set; }
        public string AnalysisT2Description { get; set; }
        public string AnalysisT2Lookup { get; set; }
        public string AnalysisT2Comments { get; set; }

        public string AnalysisT3 { get; set; }
        public string AnalysisT3Description { get; set; }
        public string AnalysisT3Lookup { get; set; }
        public string AnalysisT3Comments { get; set; }

        public string AnalysisT4 { get; set; }
        public string AnalysisT4Description { get; set; }
        public string AnalysisT4Lookup { get; set; }
        public string AnalysisT4Comments { get; set; }

        public string AnalysisT5 { get; set; }
        public string AnalysisT5Description { get; set; }
        public string AnalysisT5Lookup { get; set; }
        public string AnalysisT5Comments { get; set; }

        public string AnalysisT6 { get; set; }
        public string AnalysisT6Description { get; set; }
        public string AnalysisT6Lookup { get; set; }
        public string AnalysisT6Comments { get; set; }

        public string AnalysisT7 { get; set; }
        public string AnalysisT7Description { get; set; }
        public string AnalysisT7Lookup { get; set; }
        public string AnalysisT7Comments { get; set; }

        public string AnalysisT8 { get; set; }
        public string AnalysisT8Description { get; set; }
        public string AnalysisT8Lookup { get; set; }
        public string AnalysisT8Comments { get; set; }

        public string AnalysisT9 { get; set; }
        public string AnalysisT9Description { get; set; }
        public string AnalysisT9Lookup { get; set; }
        public string AnalysisT9Comments { get; set; }

        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public string Description5 { get; set; }
        public string Description6 { get; set; }

        public string HeldReference { get; set; }
        public string AllocationStatus { get; set; }
    }

}
