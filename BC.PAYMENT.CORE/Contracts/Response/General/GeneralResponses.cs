namespace BC.PAYMENT.CORE.Contracts.Response.General;

public class AccountCodeResponse
{
    public string ACC_CODE { get; set; } = null!;
    public string ACC_COM1 { get; set; } = null!;
}

public class AnalysisRangeDetailResponse
{
    public string ANAD_CODE { get; set; } = null!;
    public string ANAD_COM { get; set; } = null!;
}

public class AnalysisAllDetailResponse
{
    public string ANAD_CODE { get; set; } = null!;
    public string ANAD_DESC { get; set; } = null!;
    public string ANAD_COM { get; set; } = null!;
}

public class AnalysisTypeResponse
{
    public string ANAM_CODE { get; set; } = null!;
    public string ANAM_DESC { get; set; } = null!;
}
