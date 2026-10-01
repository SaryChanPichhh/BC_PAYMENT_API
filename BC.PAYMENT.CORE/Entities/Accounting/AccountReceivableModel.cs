namespace BC.PAYMENT.CORE.Entities.Accounting;

public class AccountReceivableModel
{
    public class AccountReceivablePatternModel
    {
        public string? DbCode { get; set; }
        public string? AccountCode { get; set; }
        public string? Description { get; set; }
        public string? BcDatatype { get; set; }
        public CreditDebitType CreditDebitType { get; set; }
        public string JournalType => Field1;
        public string? Field1 { get; set; }
        public string? Field2 { get; set; }
        public string? Field3 { get; set; }
        public string? Field4 { get; set; }
        public string? Field5 { get; set; }
        public string? Field6 { get; set; }
        public string? Field7 { get; set; }
        public string? Field8 { get; set; }
        public string? Field9 { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class AccountReceivableParameter
    {
        public AccountReceivableParameter(string dbCode, string accCode, int accPeriod2, double amount6,
            string reference9, int entryPrd12, string allocPeriod17, string allocUser18, string analT026,
            string analT127, string analT228, string analT329, string analT430, string analT531, string analT632,
            string analT733, string analT834, string analT935, int holdRef43, string holdUserCode44, string userCrea45,
            string userUpdt46, DateTime dateUpdt47)
        {
            DbCode = dbCode;
            ACC_CODE = accCode;
            ACC_PERIOD_2 = accPeriod2;
            AMOUNT_6 = amount6;
            REFERENCE_9 = reference9;
            ENTRY_PRD_12 = entryPrd12;
            ALLOC_PERIOD_17 = allocPeriod17;
            ALLOC_USER_18 = allocUser18;
            ANAL_T0_26 = analT026;
            ANAL_T1_27 = analT127;
            ANAL_T2_28 = analT228;
            ANAL_T3_29 = analT329;
            ANAL_T4_30 = analT430;
            ANAL_T5_31 = analT531;
            ANAL_T6_32 = analT632;
            ANAL_T7_33 = analT733;
            ANAL_T8_34 = analT834;
            ANAL_T9_35 = analT935;
            HOLD_REF_43 = holdRef43;
            HOLD_USER_CODE_44 = holdUserCode44;
            USER_CREA_45 = userCrea45;
            USER_UPDT_46 = userUpdt46;
            DATE_UPDT_47 = dateUpdt47;
        }

        public string DbCode { get; set; }
        public string ACC_CODE { get; set; }
        public int ACC_PERIOD_2 { get; set; }
        public DateTime TRANS_DATE_3 => DateTime.Today;
        public int JRNAL_NO_4 => 0;
        public string JRNAL_LINE_5 => "";
        public double AMOUNT_6 { get; set; }
        public string D_C_7 => "";
        private string JRNAL_TYPE_8 { get; set; }
        public string REFERENCE_9 { get; set; }
        public string DESCRIPTN_10 => $"CASH IN FROM {REFERENCE_9} ON {DateTime.Today:dd.MM.yyyy}";
        public DateTime ENTRY_DATE_11 => DateTime.Now;
        public int ENTRY_PRD_12 { get; set; }
        public string DUE_DATE_13 => DateTime.Today.ToString("MM/dd/yyyy");
        public string ALLOCATION_14 => "A";
        public string ALLOC_REF_15 => "0";
        public string ALLOC_DATE_16 => DateTime.Today.ToString("yyyy-MM-dd");
        public string ALLOC_PERIOD_17 { get; set; }
        public string ALLOC_USER_18 { get; set; }
        public string ASSET_CODE_19 => "";
        public string ASSET_UPDT_20 => "N";
        public string CONV_CODE_21 => "";
        public string CONV_SIGN_22 => "";
        public double CONV_RATE_23 => 0.00000;
        public double OTHER_AMT_24 => 0.00000;
        public string LOSS_GAIN_25 => "";
        public string ANAL_T0_26 { get; set; }
        public string ANAL_T1_27 { get; set; }
        public string ANAL_T2_28 { get; set; }
        public string ANAL_T3_29 { get; set; }
        public string ANAL_T4_30 { get; set; }
        public string ANAL_T5_31 { get; set; }
        public string ANAL_T6_32 { get; set; }
        public string ANAL_T7_33 { get; set; }
        public string ANAL_T8_34 { get; set; }
        public string ANAL_T9_35 { get; set; }
        public string TRAN_DESC1_36 => "";
        public string TRAN_DESC2_37 => "";
        public string TRAN_DESC3_38 => "";
        public string TRAN_DESC4_39 => "";
        public string TRAN_DESC5_40 => "";
        public string TRAN_DESC6_41 => "";
        public string ALLOC_IN_PROG_42 => "";
        public int HOLD_REF_43 { get; set; }
        public string HOLD_USER_CODE_44 { get; set; }
        public string USER_CREA_45 { get; set; }
        public string USER_UPDT_46 { get; set; }
        public DateTime DATE_UPDT_47 { get; set; }
    }
}