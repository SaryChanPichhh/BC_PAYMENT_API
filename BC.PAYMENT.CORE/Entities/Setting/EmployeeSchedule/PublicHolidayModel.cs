namespace BC.PAYMENT.CORE.Entities.Prepare.EmployeeSchedule;

public class PublicHolidayModel
{
    public string? Code { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Remark { get; set; }
    public string? Status { get; set; }
    public int Year { get; set; }
    public string? UserName { get; set; }
}