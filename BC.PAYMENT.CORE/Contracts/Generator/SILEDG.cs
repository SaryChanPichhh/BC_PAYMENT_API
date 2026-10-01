namespace BC.PAYMENT.CORE.DTO.Generator;

public class SILEDG
{
    public string ACC_CODE { get; set; }
    public int ACC_PERIOD { get; set; }
    public DateTime TRANS_DATE { get; set; }
    public int JRNAL_NO { get; set; }
    public int JRNAL_LINE { get; set; }
    public double AMOUNT { get; set; }
    public string D_C { get; set; }
    public string JRNAL_TYPE { get; set; }
    public string REFERENCE { get; set; }
    public string DESCRIPTN { get; set; }
    public DateTime ENTRY_DATE { get; set; }
    public int ENTRY_PRD { get; set; }
    public string DUE_DATE { get; set; }
    public string ALLOCATION { get; set; }
    public int ALLOC_REF { get; set; }
    public string ALLOC_DATE { get; set; }
    public int ALLOC_PERIOD { get; set; }
    public string ALLOC_USER { get; set; }
    public string ASSET_CODE { get; set; }
    public string ASSET_UPDT { get; set; }
    public string CONV_CODE { get; set; }
    public string CONV_SIGN { get; set; }
    public decimal CONV_RATE { get; set; }
    public decimal OTHER_AMT { get; set; }
    public string LOSS_GAIN { get; set; }
    public string ANAL_T0 { get; set; }
    public string ANAL_T1 { get; set; }
    public string ANAL_T2 { get; set; }
    public string ANAL_T3 { get; set; }
    public string ANAL_T4 { get; set; }
    public string ANAL_T5 { get; set; }
    public string ANAL_T6 { get; set; }
    public string ANAL_T7 { get; set; }
    public string ANAL_T8 { get; set; }
    public string ANAL_T9 { get; set; }
    public string TRAN_DESC1 { get; set; }
    public string TRAN_DESC2 { get; set; }
    public string TRAN_DESC3 { get; set; }
    public string TRAN_DESC4 { get; set; }
    public string TRAN_DESC5 { get; set; }
    public string TRAN_DESC6 { get; set; }
    public string ALLOC_IN_PROG { get; set; }
    public int HOLD_REF { get; set; }
    public string HOLD_USER_CODE { get; set; }
    public string USER_CREA { get; set; }
    public string USER_UPDT { get; set; }
    public DateTime DATE_UPDT { get; set; }

    public SILEDG(string accCode, int accPeriod, DateTime transDate, int jrnalNo, int jrnalLine, double amount,
        string dC, string jrnalType, string reference, string descriptn, DateTime entryDate, int entryPrd,
        string dueDate, string allocation, int allocRef, string allocDate, int allocPeriod, string allocUser,
        string assetCode, string assetUpdt, string convCode, string convSign, decimal convRate, decimal otherAmt,
        string lossGain, string analT0, string analT1, string analT2, string analT3, string analT4, string analT5,
        string analT6, string analT7, string analT8, string analT9, string tranDesc1, string tranDesc2,
        string tranDesc3, string tranDesc4, string tranDesc5, string tranDesc6, string allocInProgress, int holdRef,
        string holdUserCode, string userCrea, string userUpdt, DateTime dateUpdt)
    {
        ACC_CODE = accCode;
        ACC_PERIOD = accPeriod;
        TRANS_DATE = transDate;
        JRNAL_NO = jrnalNo;
        JRNAL_LINE = jrnalLine;
        AMOUNT = amount;
        D_C = dC;
        JRNAL_TYPE = jrnalType;
        REFERENCE = reference;
        DESCRIPTN = descriptn;
        ENTRY_DATE = entryDate;
        ENTRY_PRD = entryPrd;
        DUE_DATE = dueDate;
        ALLOCATION = allocation;
        ALLOC_REF = allocRef;
        ALLOC_DATE = allocDate;
        ALLOC_PERIOD = allocPeriod;
        ALLOC_USER = allocUser;
        ASSET_CODE = assetCode;
        ASSET_UPDT = assetUpdt;
        CONV_CODE = convCode;
        CONV_SIGN = convSign;
        CONV_RATE = convRate;
        OTHER_AMT = otherAmt;
        LOSS_GAIN = lossGain;
        ANAL_T0 = analT0;
        ANAL_T1 = analT1;
        ANAL_T2 = analT2;
        ANAL_T3 = analT3;
        ANAL_T4 = analT4;
        ANAL_T5 = analT5;
        ANAL_T6 = analT6;
        ANAL_T7 = analT7;
        ANAL_T8 = analT8;
        ANAL_T9 = analT9;
        TRAN_DESC1 = tranDesc1;
        TRAN_DESC2 = tranDesc2;
        TRAN_DESC3 = tranDesc3;
        TRAN_DESC4 = tranDesc4;
        TRAN_DESC5 = tranDesc5;
        TRAN_DESC6 = tranDesc6;
        ALLOC_IN_PROG = allocInProgress;
        HOLD_REF = holdRef;
        HOLD_USER_CODE = holdUserCode;
        USER_CREA = userCrea;
        USER_UPDT = userUpdt;
        DATE_UPDT = dateUpdt;
    }
}