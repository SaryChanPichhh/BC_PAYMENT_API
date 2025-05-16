
namespace BC.PAYMENT.CORE.DTO.Prepare.EmployeeSchedule
{
    public class PublicHolidayDto
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Remark { get; set; }
        public string? UserName { get; set; }
    }
}
