namespace BC.PAYMENT.CORE.DTO.AR;

public class AgingDto : ReportDTO
{
    public string DB_CODE { get; set; } // nvarchar(5)
    public DateTime BY_DATE { get; set; } // smalldatetime
    public string FROM_ACC { get; set; } // nvarchar(15)
    public string TO_ACC { get; set; } // nvarchar(15)
    public string ACC_TYPE { get; set; } // char(1)
    public string T { get; set; } // char(2)
    public string FROM_ANAL { get; set; } // nvarchar(15)
    public string TO_ANAL { get; set; } // nvarchar(15)
    public string T0 { get; set; } = "%"; // nvarchar(15)
    public string T1 { get; set; } = "%"; // nvarchar(15)
    public string T2 { get; set; } = "%"; // nvarchar(15)
    public string T3 { get; set; } = "%"; // nvarchar(15)
    public string T4 { get; set; } = "%"; // nvarchar(15)
    public string T5 { get; set; } = "%"; // nvarchar(15)
    public string T6 { get; set; } = "%"; // nvarchar(15)
    public string T7 { get; set; } = "%"; // nvarchar(15)
    public string T8 { get; set; } = "%"; // nvarchar(15)
    public string T9 { get; set; } = "%"; // nvarchar(15)
}