namespace BC.PAYMENT.CORE.Entities.General;

public class EmployeeModel
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string AreaName { get; set; }
    public string DbCode { get; set; }
}

public class TemplateModel
{
    public int Id { get; set; }
    public string Description { get; set; }
    public DateTime ClosingDate { get; set; }
}